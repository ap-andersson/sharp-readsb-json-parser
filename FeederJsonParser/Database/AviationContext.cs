using Microsoft.EntityFrameworkCore;

namespace FeederJsonParser.Database;

public class AviationContext : DbContext
{
	public DbSet<AircraftFlightTimespan> FlightInfos { get; set; }

	public string DbPath { get; }

	public AviationContext(string path)
	{
		DbPath = Path.Combine(path, "aviation.sqlite");
		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DbPath}");
}