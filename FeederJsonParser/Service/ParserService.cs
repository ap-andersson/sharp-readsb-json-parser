using System.Net.Sockets;
using System.Text.Json;
using FeederJsonParser.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeederJsonParser.Service;

internal class ParserService : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly IAircraftHandler _aircraftHandler;
    private readonly CancellationTokenSource _cts;
    //private readonly List<IAircraftHandler> _aircraftHandlers;

    public ParserService(ILogger<ParserService> logger, IOptions<AppSettings> settings, /*List<IAircraftHandler> aircraftHandlers*/ IAircraftHandler aircraftHandler)
    {
        _logger = logger;
        _settings = settings;
        _aircraftHandler = aircraftHandler;
        _cts = new CancellationTokenSource();

        //_aircraftHandlers = aircraftHandlers;
    }

    public async Task RunForrestRun()
    {
        _logger.LogDebug("Getting started");

        _logger.LogDebug($"Will attempt to connect to feeder Json endpoint: {_settings.Value.URL}:{_settings.Value.Port}");

        var ct = _cts.Token;

        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cts.Cancel();
        };

        using TcpClient client = new();
        await client.ConnectAsync(_settings.Value.URL, _settings.Value.Port, ct);

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

                // I do not want to await at this time
                //_aircraftHandlers.ForEach(handler => handler.HandleAircraft(aircraft));
                await _aircraftHandler.HandleAircraft(aircraft);

            }
            catch (OperationCanceledException)
            {
                break;
            }

        }

        _logger.LogDebug("Done, stopping");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await RunForrestRun();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Nothing here, move along
    }
}