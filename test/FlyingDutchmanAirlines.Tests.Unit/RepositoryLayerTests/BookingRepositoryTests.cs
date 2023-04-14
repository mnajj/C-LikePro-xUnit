using Bogus;
using FluentAssertions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.Tests.Unit.RepositoryLayerTests.Stubs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.RepositoryLayerTests;

[Collection("SerializedCollection")]
public class BookingRepositoryTests
{
	private readonly BookingRepository _sut;
	private readonly FlyingDutchmanAirlinesContext _context;

	public BookingRepositoryTests()
	{
		_context = new FlyingDutchmanAirlinesContextStub(
			new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
				.UseInMemoryDatabase("FlyingDutchman").Options);
		_sut = new BookingRepository(_context);
		_sut.Should().NotBeNull();
	}


	[Theory]
	[InlineData(5, -1)]
	[InlineData(-5, 1)]
	[InlineData(-1, -1)]
	public async Task CreateBooking_ShouldThrowArgumentException_WhenArgumentsIsInvalid(int customerId, int flightNumber)
	{
		// Act
		var result =  async () => await _sut.CreateBooking(customerId, flightNumber);
		
		// Assert
		await result.Should().ThrowAsync<ArgumentException>();
	}

	[Fact]
	public async Task CreateBooking_ShouldThrowException_WhenCustomerNotExistInDb()
	{
		// Act
		var result = async () => await _sut.CreateBooking(0, 1);
		// Assert
		await result.Should().ThrowAsync<CouldNotAddBookingToDatabaseException>();
	}

	[Fact]
	public async Task CreateBooking_ShouldCreateBooking_WhenCustomerExistsAndDataIsValid()
	{
		// Arrange
		var booking = new Booking
		{
			CustomerId = 1,
			FlightNumber = 1,
			BookingId = 1
		};
		// Act
		 await _sut.CreateBooking(1, 1);
		// Assert
		var result = await _context.Bookings.FirstOrDefaultAsync();
		result.Should().BeEquivalentTo(booking);
		result.Should().NotBeNull();
		result?.CustomerId.Should().Be(1);
		result?.BookingId.Should().Be(1);
	}
}
