using FeederJsonParser.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FeederJsonParser.Handler;
using FeederJsonParser.Database;

namespace FeederJsonParser;

internal class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Starting");

		var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.AddEnvironmentVariables()
			.Build();

		var appSettings = new AppSettings();
		configuration.Bind(appSettings);

		var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.SetMinimumLevel(appSettings.FEEDER_JSON_PARSER_TRACE ? LogLevel.Trace : LogLevel.Debug);
			builder.AddSimpleConsole(options =>
			{
				options.IncludeScopes = false;
				options.SingleLine = true;
				options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
			});
		});

		var logger = loggerFactory.CreateLogger<ProgramRunner>();

		await using var db = new AviationContext(appSettings.FEEDER_JSON_PARSER_SQLITE_LOCATION);

		var runner = new ProgramRunner(logger, appSettings, new List<IAircraftHandler>
			{
				new DefaultAircraftHandler(logger, db)
			});

		await runner.RunForrestRun();
	}
}