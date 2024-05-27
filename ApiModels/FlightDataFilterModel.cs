namespace FlightInformation;

public class FlightDataFilterModel
{
	public Guid? SenderId { get; set; }
	public string? AircraftHex { get; set; }
	public string? AircraftType { get; set; }
	public string? AircraftRegistration { get; set; }
	public string? FlightNumber { get; set; }
	public DateTime? Day { get; set; }
}