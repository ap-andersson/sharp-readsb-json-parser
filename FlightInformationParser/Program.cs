using FlightInformationParser.Database;
using FlightInformationParser.Dto;
using FlightInformationParser.Handler;
using FlightInformationParser.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FlightInformationParser;

internal class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Starting");

		var builder = Host.CreateApplicationBuilder(args);
		
		builder.Configuration.AddJsonFile("appsettings.local.json", true);

		builder.Logging.AddSimpleConsole(options =>
		{
			options.IncludeScopes = false;
			options.SingleLine = true;
			options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
		});

		builder.Services.AddHostedService<SyncDataService>();
		builder.Services.AddHostedService<ParserService>();
		builder.Services.AddOptions<AppSettings>().Bind(builder.Configuration.GetSection(AppSettings.ConfigAreaName));

		builder.Services.AddDbContext<AviationContext>(optionsBuilder =>
		{
			optionsBuilder.EnableDetailedErrors();
			optionsBuilder.EnableSensitiveDataLogging();
		});

		builder.Services.AddScoped<IAircraftHandler, DefaultAircraftHandler>();

		var host = builder.Build();

		await host.RunAsync();
	}
}