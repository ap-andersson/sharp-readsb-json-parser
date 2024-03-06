using FeederJsonParser.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FeederJsonParser.Service;

internal class SyncDataService : IHostedService, IDisposable
{
	private readonly ILogger<SyncDataService> _logger;
	private readonly AviationContext _db;
	private Timer? _timer;

	public SyncDataService(ILogger<SyncDataService> logger, AviationContext db)
	{
		_logger = logger;
		_db = db;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("SyncDataService running.");

		_timer = new Timer(SyncUpdatedRecords, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

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

	private void SyncUpdatedRecords(object? state)
	{
		_logger.LogDebug("SyncDataService is working.");
	}
}