using System.Net.Http.Headers;
using System.Net.Http.Json;
using FeederJsonParser.Database;
using FeederJsonParser.Dto;
using ApiModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeederJsonParser.Service;

internal class SyncDataService : IHostedService, IDisposable
{
	private readonly ILogger<SyncDataService> _logger;
	private readonly IOptions<AppSettings> _settings;
	private readonly IServiceScopeFactory _factory;
	private Timer? _timer;

	public SyncDataService(ILogger<SyncDataService> logger
		, IOptions<AppSettings> settings
		, IServiceScopeFactory factory
		)
	{
		_logger = logger;
		_settings = settings;
		_factory = factory;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("SyncDataService running.");

		if(!_settings.Value.SenderId.HasValue)
		{
			_logger.LogWarning("No sender ID sent, syncing disabled");
			return Task.CompletedTask;
		}

		// Run SyncUpdatedRecords every X seconds based on config
		_timer = new Timer(SyncUpdatedRecords, 
			null, 
			TimeSpan.Zero, 
			TimeSpan.FromSeconds(_settings.Value.SyncIntervalSeconds)
			);

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("SyncDataService is stopping.");

		_timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}

	private async void SyncUpdatedRecords(object? state)
	{
		_logger.LogDebug("SyncDataService is working.");
		try
		{
			await using var db = _factory.CreateScope()
				.ServiceProvider
				.GetRequiredService<AviationContext>();

			var toBeUpdated = await db.FlightInfos
				.AsQueryable()
				.Where(x => 
					x.Updated 
					&& (x.LastSynced == null || x.LastSynced < DateTime.Now.AddSeconds(-60))
					).ToListAsync();

			var model = MapToModel(toBeUpdated);

			_logger.LogDebug($"Will now post {model.FlightDataList!.Count} data points with sender id '{model.SenderId}'.");

			using var client = new HttpClient();

			client.BaseAddress = new Uri(_settings.Value.ApiUrl);

			var response = await client.PostAsJsonAsync("Ingest", model);

			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("Failed to sync data: " + response.ReasonPhrase);
				return;
			}

			var updatedIds = toBeUpdated.Select(x => x.Id).ToList();

			await db.FlightInfos
				.Where(x => updatedIds.Contains(x.Id))
				.ExecuteUpdateAsync(calls => calls.SetProperty(r => r.LastSynced, DateTime.Now));

			await db.FlightInfos
				.Where(x => updatedIds.Contains(x.Id))
				.ExecuteUpdateAsync(calls => calls.SetProperty(r => r.Updated, false));

			_logger.LogDebug("Sync data done");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unable to sync data");
		}
	}

	private FlightDataModel MapToModel(List<AircraftFlightTimespan> toBeUpdated)
	{
		return new FlightDataModel
		{
			SenderId = _settings.Value.SenderId!.Value,
			FlightDataList = toBeUpdated.Select(x => new FlightData
			{
				AircraftHex = x.AircraftHex,
				EndTime = x.EndTime,
				StartTime = x.StartTime,
				FlightNumber = x.FlightNumber,
				AircraftRegistration = x.AircraftRegistration,
				AircraftType = x.AircraftType,
			}).ToList()
		};
	}
}