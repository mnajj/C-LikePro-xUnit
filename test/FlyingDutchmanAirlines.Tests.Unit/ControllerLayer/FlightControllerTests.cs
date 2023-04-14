using System.Net;
using FluentAssertions;
using FlyingDutchmanAirlines.ControllerLayer;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.ServiceLayer.Views;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.ControllerLayer;

public class FlightControllerTests
{
	private readonly FlightService _service = Substitute.For<FlightService>();
	private readonly FlightController _sut;

	public FlightControllerTests()
	{
		_sut = new FlightController(_service);
	}
	
	[Fact]
	public async Task GetFlights_ShouldReturnFlights_WhenFlightsExists()
	{
		// Arrange
		List<FlightView> views = new List<FlightView>(2) 
		{
			new ("1932", ("Groningen", "GRQ"), ("Phoenix", "PHX")),
			new("841", ("New York City", "JFK"), ("London", "LHR"))
		};
		_service.GetFlights().Returns(FlightViewAsyncGenerator(views));
		
		// Act
		var response = await _sut.GetFlights() as ObjectResult;
		
		// Assert
		response.Should().NotBeNull();
		response!.StatusCode.Should().Be((int)HttpStatusCode.OK);
		var content = response!.Value as Queue<FlightView>;
		content.Should().NotBeNull()
			.And.BeEquivalentTo(views);
	}
	
	private async IAsyncEnumerable<FlightView> FlightViewAsyncGenerator(IEnumerable<FlightView> views) 
	{
		foreach (FlightView flightView in views) 
		{
			yield return flightView;
		}
	}

	[Fact]
	public async Task getFlights_ShouldThrowFlightNotFoundException_WhenFlightDoesNotExists()
	{
		// Arrange
		_service.GetFlights().Throws<FlightNotFoundException>();

		// Act
		var result = await _sut.GetFlights() as ObjectResult;

		// Assert
		result.Should().NotBeNull();
		result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
		result!.Value.Should().Be("No flights were found in the database");
	}
	
	[Fact]
	public async Task getFlights_ShouldThrowInternalError_WhenSomethingWentWrong()
	{
		// Arrange
		_service.GetFlights().Throws<ApplicationException>();

		// Act
		var result = await _sut.GetFlights() as ObjectResult;

		// Assert
		result.Should().NotBeNull();
		result!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
		result!.Value.Should().Be("An error occurred");
	}	
	
	[Fact]
	public async Task GetFlightByFlightNumber_ShouldReturnFlightView_WhenFlightIsExists()
	{
		// Arrange
		var view = new FlightView("0",("Lagos", "LOS"),("Marrakesh", "RAK"));
		_service.GetFlightByFlightNumber(int.Parse(view.FlightNumber)).Returns(view);
		
		// Act
		var result = await _sut.GetFlightByNumber(int.Parse(view.FlightNumber)) as ObjectResult;
		
		// Assert
		result!.Should().NotBeNull();
		result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
		var content = (FlightView)result!.Value!;
		content.Should().Be(view);
	}	
	
	[Fact]
	public async Task GetFlightByFlightNumber_ShouldThrowFlightNotFoundException_WhenFlightDoesNotExists()
	{
		// Arrange
		_service.GetFlightByFlightNumber(1).Throws<FlightNotFoundException>();

		// Act
		var result = await _sut.GetFlightByNumber(1) as ObjectResult;

		// Assert
		result.Should().NotBeNull();
		result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
		result!.Value.Should().Be("The flights were found in the database");
	}
	
	[Fact]
	public async Task GetFlights_ShouldThrowBadRequestException_WhenDataIsInvalid()
	{
		// Arrange
		_service.GetFlightByFlightNumber(-1).Throws<ArgumentException>();

		// Act
		var result = await _sut.GetFlightByNumber(-1) as ObjectResult;

		// Assert
		result.Should().NotBeNull();
		result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		result!.Value.Should().Be("An error occurred");
	}		
}