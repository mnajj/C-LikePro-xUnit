using FluentAssertions;
using FlyingDutchmanAirlines.ControllerLayer.Contracts;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.ControllerLayer.Contracts;

public sealed class BookingRequestTests
{
	[Fact]
	public void BookingRequest_ShouldCreateBooking_WhenDataIsValid()
	{
		// Arrange
		// Act
		var bookingData = new BookingRequest {FirstName = "Marina", LastName = "Michaels"};
		
		// Assert
		bookingData.FirstName.Should().Be("Marina");
		bookingData.LastName.Should().Be("Michaels");
	}
	
	[Theory]
	[InlineData(null, "mohab")]
	[InlineData("mohab", null)]
	public void BookingRequest_ShouldThrowException_WhenDataIsNull(string first, string last)
	{
		// Arrange
		// Act
		var result = () => new BookingRequest { FirstName = first, LastName = last };
		
		// Assert
		result.Should().Throw<InvalidOperationException>();
	}	
	
	[Theory]
	[InlineData("", "mohab")]
	[InlineData("mohab", "")]
	public void BookingRequest_ShouldThrowException_WhenDataIsEmpty(string first, string last)
	{
		// Arrange
		// Act
		var result = () => new BookingRequest { FirstName = first, LastName = last };
		
		// Assert
		result.Should().Throw<InvalidOperationException>();
	}	
}