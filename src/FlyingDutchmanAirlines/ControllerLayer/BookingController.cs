using System.Net;
using FlyingDutchmanAirlines.ControllerLayer.Contracts;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlyingDutchmanAirlines.ControllerLayer;

[Route("{controller}")]
public sealed class BookingController : Controller
{
	private readonly BookingService _service;

	public BookingController(BookingService service)
	{
		_service = service;
	}

	[HttpPost("{flightNumber}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> CreateBooking([FromBody] BookingRequest bookingRequest, int flightNumber)
	{
		if (!ModelState.IsValid || flightNumber.IsPositive())
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, 
				ModelState.Root.Errors.First().ErrorMessage);
		}

		(bool result, Exception? exception) = await _service.CreateBooking(
			$"{bookingRequest.FirstName} {bookingRequest.LastName}", flightNumber);
		if (result && exception is null)
		{
			return StatusCode((int)HttpStatusCode.Created);
		}
		return exception is CouldNotAddBookingToDatabaseException 
			? NotFound()
				: StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
	}
}