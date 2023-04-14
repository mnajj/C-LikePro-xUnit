using FluentAssertions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.ServiceLayer.Views;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.ServiceLayerTests;

public sealed class FlightServiceTests
{
	private readonly FlightService _sut;
	private readonly FlightRepository _flightRepository = Substitute.For<FlightRepository>();
	private readonly AirportRepository _airportRepository = Substitute.For<AirportRepository>();

	public FlightServiceTests()
	{
		_sut = new FlightService(_flightRepository, _airportRepository);
		
		// Arrange
		var flight = new Flight
		{
			FlightNumber = 148,
			Origin = 31,
			Destination = 92
		};
		var mockResult = new Queue<Flight>(1);
		mockResult.Enqueue(flight);
		_flightRepository.GetFlights().Returns(mockResult);
		_flightRepository.GetFlightByFlightNumber(flight.FlightNumber).Returns(flight);
		var origin = new Airport
		{
			AirportId = 31,
			City = "Mexico City",
			Iata = "MEX"
		};
		var destination = new Airport
		{
			AirportId = 92,
			City = "Ulaanbaataar",
			Iata = "UBN"
		};
		_airportRepository.GetAirportById(31).Returns(origin);
		_airportRepository.GetAirportById(92).Returns(destination);	

	}

	[Fact]
	public async Task GetFlights_ShouldReturnFlightViews_WhenFlightsExists()
	{
		// Act
		// Assert
		await foreach (var flightView in _sut.GetFlights())
		{
			flightView.FlightNumber.Should().Be("148");
			flightView.Origin.City.Should().Be("Mexico City");
			flightView.Origin.Code.Should().Be("MEX");
			flightView.Destination.City.Should().Be("Ulaanbaataar");
			flightView.Destination.Code.Should().Be("UBN");
		}
	}

	[Fact]
	public async Task GetFlights_ShouldThrowFlightNotFoundException_WhenAirportNotFound()
	{
		// Arrange
		_airportRepository.GetAirportById(31).ThrowsAsync<AirportNotFoundException>();
		
		// Act
		var result = async () =>
		{
			await foreach (var flightView in _sut.GetFlights())
			{
				;
			}
		};
		// Assert
		await result.Should().ThrowAsync<FlightNotFoundException>();
	}
	
	[Fact]
	public async Task GetFlights_ShouldThrowOtherThanFlightNotFoundException_WhenSomethingWentWrong()
	{
		// Arrange
		_airportRepository.GetAirportById(31).ThrowsAsync<ApplicationException>();
		
		// Act
		var result = async () =>
		{
			await foreach (var flightView in _sut.GetFlights())
			{
				;
			}
		};
		// Assert
		await result.Should().ThrowAsync<ArgumentException>();
	}

	[Fact]
	public async Task GetFlightByFlightNumber_ShouldReturnFlight_WhenFlightExistsAndDataIsValid()
	{
		// Arrange
		var flightNumber = 148;
		// Act
		var result = await _sut.GetFlightByFlightNumber(flightNumber);
		
		// Assert
		result.Should().NotBeNull();
		result.FlightNumber.Should().Be("148");
		result.Origin.City.Should().Be("Mexico City");
		result.Origin.Code.Should().Be("MEX");
		result.Destination.City.Should().Be("Ulaanbaataar");
		result.Destination.Code.Should().Be("UBN");
	}

	[Fact]
	public async Task GetFlightByFlightNumber_ShouldThrowFlightNotFoundException_WhenFlightNotFound()
	{
		// Arrange
		_flightRepository.GetFlightByFlightNumber(-1).ThrowsAsync<FlightNotFoundException>();
		
		// Act
		var result = async () => await _sut.GetFlightByFlightNumber(-1);

		// Assert
		await result.Should().ThrowAsync<FlightNotFoundException>();
	}
	[Fact]
	public async Task GetFlightByFlightNumber_ShouldThrowArgumentException_WhenOtherFlightNotFound()
	{
		// Arrange
		_flightRepository.GetFlightByFlightNumber(-1).ThrowsAsync<ApplicationException>();
		
		// Act
		var result = async () => await _sut.GetFlightByFlightNumber(-1);

		// Assert
		await result.Should().ThrowAsync<ArgumentException>();
	}	
	[Fact]
	public async Task GetFlightByFlightNumber_ShouldThrowFlightNotFoundException_WhenAirportNotFound()
	{
		// Arrange
		var flight = new Flight
		{
			FlightNumber = 999,
			Origin = 666,
			Destination = 555
		};
		_flightRepository.GetFlightByFlightNumber(flight.FlightNumber).Returns(flight);	
		_airportRepository.GetAirportById(666).ThrowsAsync<AirportNotFoundException>();
		
		// Act
		var result = async () => await _sut.GetFlightByFlightNumber(flight.FlightNumber);

		// Assert
		await result.Should().ThrowAsync<FlightNotFoundException>();
	}	
}