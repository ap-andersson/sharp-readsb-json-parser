using Microsoft.EntityFrameworkCore;

namespace FlightInformationApi.Database;

public class FlightDataContext : DbContext
{
	public DbSet<FlightDataRecord> FlightData { get; set; }

	public FlightDataContext(DbContextOptions<FlightDataContext> dbContextOptions) : base(dbContextOptions)
	{
		Database.EnsureCreated();
	}
}