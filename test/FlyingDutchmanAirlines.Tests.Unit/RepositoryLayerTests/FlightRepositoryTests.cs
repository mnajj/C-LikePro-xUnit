using Bogus;
using FluentAssertions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer.Views;
using FlyingDutchmanAirlines.Tests.Unit.RepositoryLayerTests.Stubs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.RepositoryLayerTests;

[Collection("SerializedCollection")]
public sealed class FlightRepositoryTests
{
	private readonly FlightRepository _sut;
	private readonly FlyingDutchmanAirlinesContext _context;

	public FlightRepositoryTests()
	{
		_context = new FlyingDutchmanAirlinesContextStub(
			new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
				.UseInMemoryDatabase("FlyingDutchman").Options);
		_sut = new FlightRepository(_context);
		_sut.Should().NotBeNull();
	}

	[Fact]
	public async Task GetFlightByFlightNumber_ShouldReturnExistingFlight_WhenDataIsValidAndFlightExists()
	{
		// Arrange
		var flight = new Flight()
		{
			FlightNumber = 1,
			Origin = 1,
			Destination = 1
		};
		await _context.Flights.AddAsync(flight);
		await _context.SaveChangesAsync();
		
		// Act
		var result = await _sut.GetFlightByFlightNumber(1);

		// Assert
		result.Should().BeEquivalentTo(flight);
	}

	[Fact]
public async Task GetFlightByFlightNumber_ShouldThrowDatabaseException_WhenDatabaseFails()
	{
		// Act
		var result = async () => await _sut.GetFlightByFlightNumber(1);
		// Assert
		await result.Should().ThrowAsync<CouldNotAddFlightToDatabaseException>();
	}
	
	[Fact]
	public async Task GetFlightByFlightNumber_ShouldThrowFlightNotFoundException_WhenFlightNumberInvalid()
	{
		// Act
		var result = async () => await _sut.GetFlightByFlightNumber(-1);
		// Assert
		await result.Should().ThrowAsync<FlightNotFoundException>();
	}

	//TODO
	[Fact]
	public async Task GetFlights_ShouldReturnAllFlights_WhenFlightsExists()
	{
		// Arrange
		var flights = new List<Flight>()
		{
			new()
		{
			FlightNumber = 1,
			Origin = 1,
			Destination = 2
		}, new()
			{
				FlightNumber = 10,
				Origin = 3,
				Destination = 4
			}
		};
		_context.Flights.AddRange(flights);
		await _context.SaveChangesAsync();
		// Act
		var result = _sut.GetFlights().ToList();
		// Assert
		result.Should().NotBeNull()
			.And.BeEquivalentTo(flights);
	}
}