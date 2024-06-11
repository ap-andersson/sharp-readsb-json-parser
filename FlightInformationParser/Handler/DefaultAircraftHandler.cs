using FlightInformationParser.Database;
using FlightInformationParser.Dto;
using FlightInformationParser.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightInformationParser.Handler;

internal class DefaultAircraftHandler : IAircraftHandler
{
	private readonly ILogger<ParserService> _logger;
	private readonly AviationContext _db;

	public DefaultAircraftHandler(ILogger<ParserService> logger, AviationContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task HandleAircraft(Aircraft aircraft)
	{
		_logger.LogTrace("Received aircraft: " + aircraft);

		Sanitize(aircraft);

		if(string.IsNullOrWhiteSpace(aircraft.Flight)) return;
		if(string.IsNullOrWhiteSpace(aircraft.AircraftRegistration)) return;

		// Find any matching existing rows
		var existingRows = await _db.FlightInfos.AsQueryable()
			.Where(x =>
				x.AircraftHex == aircraft.Hex
				&& x.FlightNumber == aircraft.Flight
				&& x.EndTime > DateTime.Now.AddHours(-1)) // Same aircraft with same flight within one hour should update the row
			.ToListAsync();

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

			await _db.FlightInfos
				.Where(x => x.Id == row.Id)
				.ExecuteUpdateAsync(calls => calls.SetProperty(r => r.Updated, true));

			await _db.FlightInfos
				.Where(x => x.Id == row.Id)
				.ExecuteUpdateAsync(calls => calls.SetProperty(r => r.EndTime, DateTime.Now));

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
			EndTime = DateTime.Now,
			Updated = true,
		};

		await _db.AddAsync(newRow);
		await _db.SaveChangesAsync();
		//_logger.LogTrace("Created new row. Aircraft: " + aircraft);
	}

	private void Sanitize(Aircraft aircraft)
	{
		aircraft.AircraftRegistration = aircraft.AircraftRegistration?.Trim().ToUpper() ?? null;
		aircraft.AircraftType = aircraft.AircraftType.Trim().ToUpper();
		aircraft.Flight = aircraft.Flight?.Trim().ToUpper() ?? null;
		aircraft.Hex = aircraft.Hex.Trim().ToUpper();
	}
}
