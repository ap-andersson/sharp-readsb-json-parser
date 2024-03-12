using System.Globalization;
using FlightInformationApi.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlightInformationApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IngestController : ControllerBase
{
	private readonly ILogger<IngestController> _logger;
	private readonly IOptions<AppSettings> _appSettings;
	private readonly FlightDataContext _flightDataContext;

	private readonly IReadOnlyDictionary<Guid, string> _sendersDict;

	public IngestController(ILogger<IngestController> logger
		, IOptions<AppSettings> appSettings
		, FlightDataContext flightDataContext
		)
	{
		_logger = logger;
		_appSettings = appSettings;
		_flightDataContext = flightDataContext;

		_sendersDict = _appSettings.Value.Senders
			.GroupBy(x => x.Id)
			.ToDictionary(
				x => x.Key, 
				x => x.First().Name
				);
	}

	[HttpGet]
	[Route("Test")]
	public IActionResult HelloWorld()
	{
		_logger.LogDebug("Hello worlds?");

		var tjo = _flightDataContext.FlightData.AsQueryable().ToList();

		var connectionString = _appSettings.Value.MysqlConnectionString;

		return Content("Hello worlds");
	}

	[HttpPost(Name = "IngestFlightData")]
	[Route("")]
	public async Task<IActionResult> FlightData([FromBody] FlightDataModel dataModel)
	{
		if (!dataModel.SenderId.HasValue || !_sendersDict.ContainsKey(dataModel.SenderId.Value))
		{
			return StatusCode(StatusCodes.Status400BadRequest, "Missing valid sender id");
		}

		if (dataModel.FlightDataList == null || dataModel.FlightDataList.Count == 0)
		{
			return StatusCode(StatusCodes.Status200OK);
		}

		// More actual data validation
		// Sanity check times and that records contains required data...

		// Let's just get all relevant existing.
		// There is potential that this will load LOADS into memory
		// Long term and if this gets more clients ingesting this should probably limit
		// the date ranges and maybe cache... 

		var relevantAircraftHexes = dataModel.FlightDataList
			.Select(x => x.AircraftHex.ToUpper(CultureInfo.InvariantCulture))
			.Distinct()
			.ToHashSet();

		var relevantTimeSpan = new
		{
			Start = dataModel.FlightDataList.Min(x => x.StartTime), 
			Stop = dataModel.FlightDataList.Max(x => x.EndTime)
		};

		var existingDbRecords = _flightDataContext.FlightData.AsQueryable()
			.Where(x =>
				relevantAircraftHexes.Contains(x.AircraftHex)
				&& x.StartTime >= relevantTimeSpan.Start
				&& x.EndTime <= relevantTimeSpan.Stop
			)
			.GroupBy(x => x.AircraftHex)
			.ToDictionary(
				x => x.Key, 
				x => x.OrderByDescending(y => y.EndTime).ToList()
				);

		foreach (var data in dataModel.FlightDataList)
		{
			// If any of these are missing we skip for now
			if(string.IsNullOrEmpty(data.AircraftHex)) continue;
			if(string.IsNullOrEmpty(data.FlightNumber)) continue;
			if(string.IsNullOrEmpty(data.AircraftRegistration)) continue;

			// Ensure CAPITAL LETTERS
			data.AircraftHex = data.AircraftHex.ToUpper(CultureInfo.InvariantCulture);
			data.FlightNumber = data.FlightNumber.ToUpper(CultureInfo.InvariantCulture);
			data.AircraftRegistration = data.AircraftRegistration.ToUpper(CultureInfo.InvariantCulture);
			data.AircraftType = data.AircraftType?.ToUpper(CultureInfo.InvariantCulture);

			if (!existingDbRecords.ContainsKey(data.AircraftHex))
			{
				await CreateRecord(data);
				continue;
			}

			var existingRecords = existingDbRecords[data.AircraftHex];

			var existingRecord = existingRecords.FirstOrDefault(x =>
			{
				if (!x.FlightNumber.Equals(data.FlightNumber)) return false;

				var startTimeDiff = (x.StartTime - data.StartTime).TotalMinutes;
				var endTimeDiff = (data.EndTime - x.EndTime).TotalMinutes;

				if ((startTimeDiff < 0 || startTimeDiff > 60) && (endTimeDiff < 0 || endTimeDiff > 60))
				{
					return false; // If not seen within 60 minutes its considered a new flight... Maybe weird. 
				}

				return true;
			});

			if (existingRecord == null)
			{
				await CreateRecord(data);
				continue;
			}

			existingRecord.StartTime = data.StartTime < existingRecord.StartTime 
				? data.StartTime 
				: existingRecord.StartTime;

			existingRecord.EndTime = data.EndTime > existingRecord.EndTime 
				? data.EndTime 
				: existingRecord.EndTime;

			existingRecord.AircraftRegistration = string.IsNullOrWhiteSpace(existingRecord.AircraftRegistration)
				? data.AircraftRegistration
				: existingRecord.AircraftRegistration;

			existingRecord.AircraftType = string.IsNullOrWhiteSpace(existingRecord.AircraftType)
				? data.AircraftRegistration
				: existingRecord.AircraftType;

			_flightDataContext.Update(existingRecord);
		}
		
		await _flightDataContext.SaveChangesAsync();

		return Ok();
	}

	private async Task CreateRecord(FlightData data)
	{
		var record = new FlightDataRecord
		{
			AircraftHex = data.AircraftHex,
			AircraftRegistration = data.AircraftRegistration,
			AircraftType = data.AircraftType,
			FlightNumber = data.FlightNumber,
			StartTime = data.StartTime,
			EndTime = data.EndTime,
		};

		await _flightDataContext.AddAsync(record);
	}
}