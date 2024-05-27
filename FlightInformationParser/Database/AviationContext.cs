using FlightInformationParser.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlightInformationParser.Database;

public class AviationContext : DbContext
{
	private readonly IOptions<AppSettings> _options;
	public DbSet<AircraftFlightTimespan> FlightInfos { get; set; }

	public string DbPath { get; }

	public AviationContext(IOptions<AppSettings> options)
	{
		_options = options;
		DbPath = Path.Combine(_options.Value.Sqlite_Location, "aviation.sqlite");
		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DbPath}");
}