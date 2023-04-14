using Bogus;
using FluentAssertions;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.ServiceLayer.Views;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.ServiceLayerTests.Views;

public sealed class FlightViewTests
{
	[Fact]
	public void FlightView_ShouldCreateNewView_WhenDataIsValid()
	{
		// Arrange
		string flightNumber = "0";
		string originCity = "Amsterdam";
		string originCityCode = "AMS";
		string destinationCity = "Moscow";
		string destinationCityCode = "SVO";
		
		// Act
		var view = new FlightView(flightNumber, (originCity, originCityCode),
			(destinationCity, destinationCityCode));
		
		// Assert
		view.Should().NotBeNull();
		view.FlightNumber.Should().Be(flightNumber);
		view.Origin.City.Should().Be(originCity);
		view.Origin.Code.Should().Be(originCityCode);
		view.Destination.City.Should().Be(destinationCity);
		view.Destination.Code.Should().Be(destinationCityCode);
		}

	[Fact]
	public void FlightView_ShouldCreateNewView_WhenFlightNumberIsNull()
	{
		// Arrange
		string originCity = "Athens";
		string originCityCode = "ATH";
		string destinationCity = "Dubai";
		string destinationCityCode = "DXB";
		
		// Act
		var view = new FlightView(null, (originCity, originCityCode),
			(destinationCity, destinationCityCode));
		
		// Assert
		view.Should().NotBeNull();
		view.FlightNumber.Should().Be("No flight number found");                
		view.Origin.City.Should().Be(originCity);
		view.Origin.Code.Should().Be(originCityCode);
		view.Destination.City.Should().Be(destinationCity);
		view.Destination.Code.Should().Be(destinationCityCode);       
	}

	[Fact]
	public void FlightView_ShouldCreateNewView_WhenCityIsEmpty()
	{
		// Arrange
		string destinationCity = string.Empty;
		string destinationCityCode = "SYD";
		
		// Act
		AirportInfo airportInfo = new AirportInfo((destinationCity, destinationCityCode));
		
		// Assert
		airportInfo.Should().NotBeNull();
		airportInfo.City.Should().Be("No city found");
		airportInfo.Code.Should().Be( destinationCityCode);
	}
	
	[Fact]
	public void FlightView_ShouldCreateNewView_WhenCodeIsEmpty()
	{
		// Arrange
		string destinationCity = "Ushuaia";
		string destinationCityCode = string.Empty;
		
		// Act
		AirportInfo airportInfo = new AirportInfo((destinationCity, destinationCityCode));
		
		// Assert
		airportInfo.Should().NotBeNull();
		airportInfo.City.Should().Be(destinationCity);
		airportInfo.Code.Should().Be("No code found");
	}
}