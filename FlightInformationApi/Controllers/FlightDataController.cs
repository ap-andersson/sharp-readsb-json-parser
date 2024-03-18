using ApiModels;
using FlightInformationApi.Database;
using FlightInformationApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlightInformationApi.Controllers;

/// <summary>
/// Controller used to ingest data from FeederJsonParser/Feeders
/// </summary>
[ApiController]
[Route("[controller]")]
public class FlightDataController : ControllerBase
{
	private readonly ILogger<FlightDataController> _logger;
	private readonly IOptions<AppSettings> _appSettings;
	private readonly FlightDataContext _db;
	private readonly Dictionary<Guid, string> _sendersDict;

	/// <inheritdoc />
	public FlightDataController(ILogger<FlightDataController> logger
		, IOptions<AppSettings> appSettings
		, FlightDataContext db
		)
	{
		_logger = logger;
		_appSettings = appSettings;
		_db = db;

		_sendersDict = _appSettings.Value.Senders
			.GroupBy(x => x.Id)
			.ToDictionary(
				x => x.Key,
				x => x.First().Name
			);
	}

	/// <summary>
	/// Filter from the available flight data
	/// </summary>
	/// <param name="filterModel">Filter model. Setting more than one property is added to the search as AND.</param>
	/// <returns>List of flight data</returns>
	[HttpGet]
	[Route("Filter")]
	[ProducesResponseType(typeof(List<FlightData>), StatusCodes.Status200OK)]
	public async Task<IActionResult> Filter([FromQuery] FlightDataFilterModel filterModel)
	{
		if (!filterModel.SenderId.HasValue || !_sendersDict.ContainsKey(filterModel.SenderId.Value))
		{
			return StatusCode(StatusCodes.Status400BadRequest, "Missing valid sender id");
		}

		if (string.IsNullOrEmpty(filterModel.AircraftHex)
		    && string.IsNullOrEmpty(filterModel.AircraftRegistration)
		    && string.IsNullOrEmpty(filterModel.FlightNumber)
		    && !filterModel.Day.HasValue)
		{
			return StatusCode(StatusCodes.Status400BadRequest, "At least one filter has to be selected");
		}

		var query = _db.FlightData.AsQueryable();

		if (filterModel.Day.HasValue)
		{
			query = query.Where(x => x.StartTime.Date == filterModel.Day.Value.Date
			                           || x.EndTime.Date == filterModel.Day.Value.Date);
		}

		if (!string.IsNullOrEmpty(filterModel.AircraftHex))
		{
			query = query.Where(x => x.AircraftHex == filterModel.AircraftHex.ToUpper()); ;
		}

		if (!string.IsNullOrEmpty(filterModel.AircraftRegistration))
		{
			query = query.Where(x => x.AircraftRegistration == filterModel.AircraftRegistration.ToUpper());
		}

		if (!string.IsNullOrEmpty(filterModel.FlightNumber))
		{
			query = query.Where(x => x.FlightNumber == filterModel.FlightNumber.ToUpper());
		}

		var result = await query.ToListAsync();

		return Ok(result);
	}

}
