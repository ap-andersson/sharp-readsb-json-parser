﻿using FeederJsonParser.Dto;
using Microsoft.Extensions.Logging;
using FeederJsonParser.Handler;
using FeederJsonParser.Database;
using FeederJsonParser.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeederJsonParser;

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