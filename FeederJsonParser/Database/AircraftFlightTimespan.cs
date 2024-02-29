using Microsoft.EntityFrameworkCore;

namespace FeederJsonParser.Database;

[PrimaryKey(nameof(Id))]
public class AircraftFlightTimespan
{
	public long Id { get; set; }
	public string AircraftHex { get; set; }
	public string AircraftType { get; set; }
	public string AircraftRegistration { get; set; }
	public string FlightNumber { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

}