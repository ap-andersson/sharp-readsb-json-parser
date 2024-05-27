namespace FlightInformation
{
	public class FlightDataModel
	{
		public Guid? SenderId { get; set; }
		public List<FlightData>? FlightDataList { get; set; }
	}

	public class FlightData
	{
		public string AircraftHex { get; set; }
		public string? AircraftType { get; set; }
		public string AircraftRegistration { get; set; }
		public string FlightNumber { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
