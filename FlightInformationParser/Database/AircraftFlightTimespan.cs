using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlightInformationParser.Database;

[PrimaryKey(nameof(Id))]
public class AircraftFlightTimespan
{
	public long Id { get; set; }

	[Column(TypeName = "varchar(8)")]
	public string AircraftHex { get; set; }

	[Column(TypeName = "varchar(4)")]
	public string AircraftType { get; set; }

	[Column(TypeName = "varchar(16)")]
	public string AircraftRegistration { get; set; }

	[Column(TypeName = "varchar(16)")]
	public string FlightNumber { get; set; }

	public DateTime StartTime { get; set; }

	public DateTime EndTime { get; set; }

	public bool Updated { get; set; }

	public DateTime? LastSynced { get; set; }

}