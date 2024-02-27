using FeederJsonParser.Dto;
using Microsoft.Extensions.Logging;

namespace FeederJsonParser.Handler;

internal class DefaultAircraftHandler : IAircraftHandler
{
	private readonly ILogger<ProgramRunner> _logger;

	public DefaultAircraftHandler(ILogger<ProgramRunner> logger)
	{
		_logger = logger;
	}

	public async Task HandleAircraft(Aircraft aircraft)
	{
		_logger.LogTrace("Received aircraft: " + aircraft);
	}
}
