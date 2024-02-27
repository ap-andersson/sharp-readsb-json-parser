using System.Net.Sockets;
using System.Text.Json;
using FeederJsonParser.Dto;
using Microsoft.Extensions.Logging;

namespace FeederJsonParser;

internal class ProgramRunner
{
	private readonly ILogger _logger;
	private readonly AppSettings _settings;
	private readonly List<IAircraftHandler> _aircraftHandlers;

	public ProgramRunner(ILogger logger, AppSettings settings, List<IAircraftHandler> aircraftHandlers)
	{
		_logger = logger;
		_settings = settings;
		_aircraftHandlers = aircraftHandlers;
	}

	public async Task RunForrestRun()
	{
		_logger.LogDebug("Getting started");

		_logger.LogDebug($"Will attempt to connect to feeder Json endpoint: {_settings.FEEDER_JSON_PARSER_URL}:{_settings.FEEDER_JSON_PARSER_PORT}");

		var cts = new CancellationTokenSource();
		var ct = cts.Token;

		Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
			e.Cancel = true;
			cts.Cancel();
		};

		using TcpClient client = new();
		await client.ConnectAsync(_settings.FEEDER_JSON_PARSER_URL, _settings.FEEDER_JSON_PARSER_PORT, ct);

		_logger.LogDebug("Connected");

		await using NetworkStream stream = client.GetStream();
		using var streamReader = new StreamReader(stream);

		while (!ct.IsCancellationRequested)
		{
			try
			{
				var line = await streamReader.ReadLineAsync(ct);

				if (line == null)
				{
					_logger.LogTrace("Received NULL line");
					continue;
				};

				var aircraft = JsonSerializer.Deserialize<Aircraft>(line);

				if (aircraft == null)
				{
					_logger.LogTrace("Failed to deserialize aircraft: " + line);
					continue;
				};

				_aircraftHandlers.ForEach(handler => handler.HandleAircraft(aircraft));

			}
			catch (OperationCanceledException)
			{
				break;
			}

		}

		_logger.LogDebug("Done, stopping");
	}
}