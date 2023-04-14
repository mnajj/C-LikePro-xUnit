using System.Net;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.ServiceLayer.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyingDutchmanAirlines.ControllerLayer;

[Route("{controller}")]
public sealed class FlightController : Controller
{
	private readonly FlightService _service;

	public FlightController(FlightService service)
	{
		_service = service;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetFlights()
	{
		try
		{
			var flights = new Queue<FlightView>();
			await foreach (var flight in _service.GetFlights()) flights.Enqueue(flight);
			return Ok(flights);
		}
		catch (FlightNotFoundException)
		{
			return NotFound("No flights were found in the database");
		}
		catch (Exception)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred");
		}
	}

	[HttpGet("{flightNumber}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetFlightByNumber(int flightNumber)
	{
		try
		{
			if (!flightNumber.IsPositive()) throw new Exception();
			return Ok(await _service.GetFlightByFlightNumber(flightNumber));
		}
		catch (FlightNotFoundException)
		{
			return NotFound("The flights were found in the database");
		}
		catch (Exception)
		{
			return BadRequest("An error occurred");
		}
	}
}