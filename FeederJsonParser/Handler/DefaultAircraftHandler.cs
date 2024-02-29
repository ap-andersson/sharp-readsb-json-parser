using FeederJsonParser.Database;
using FeederJsonParser.Dto;
using Microsoft.Extensions.Logging;

namespace FeederJsonParser.Handler;

internal class DefaultAircraftHandler : IAircraftHandler
{
	private readonly ILogger<ProgramRunner> _logger;
	private readonly AviationContext _db;

	public DefaultAircraftHandler(ILogger<ProgramRunner> logger, AviationContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task HandleAircraft(Aircraft aircraft)
	{
		_logger.LogTrace("Received aircraft: " + aircraft);

		Sanitize(aircraft);

		if(string.IsNullOrWhiteSpace(aircraft.Flight)) return;

		// Find any matching existing rows
		var existingRows = _db.FlightInfos.AsEnumerable()
			.Where(x => 
				x.AircraftHex.Equals(aircraft.Hex) 
				&& x.FlightNumber.Equals(aircraft.Flight)
				&& (DateTime.Now - x.EndTime).TotalMinutes < 10d)
			.ToList();

		// More than one row found, should not happen, skip
		if (existingRows.Count > 1)
		{
			_logger.LogWarning("Found more than 1 existing row (which should not happen). Will not do anything. Aircraft: " + aircraft);
			return;
		}

		// One matching row, lets update the end time
		if (existingRows.Count == 1)
		{
			var row = existingRows[0];

			row.EndTime = DateTime.Now;
			_db.Update(row);
			await _db.SaveChangesAsync();
			//_logger.LogTrace("Updated existing row. Aircraft: " + aircraft);
			return;
		}

		// Create new row
		var newRow = new AircraftFlightTimespan
		{
			AircraftHex = aircraft.Hex,
			AircraftRegistration = aircraft.AircraftRegistration,
			AircraftType = aircraft.AircraftType,
			FlightNumber = aircraft.Flight,
			StartTime = DateTime.Now,
			EndTime = DateTime.Now
		};

		await _db.AddAsync(newRow);
		await _db.SaveChangesAsync();
		//_logger.LogTrace("Created new row. Aircraft: " + aircraft);
	}

	private void Sanitize(Aircraft aircraft)
	{
		aircraft.AircraftRegistration = aircraft.AircraftRegistration.Trim().ToUpper();
		aircraft.AircraftType = aircraft.AircraftType.Trim().ToUpper();
		aircraft.Flight = aircraft.Flight?.Trim().ToUpper() ?? null;
		aircraft.Hex = aircraft.Hex.Trim().ToUpper();
	}
}
