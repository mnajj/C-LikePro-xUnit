using System.Data.SqlTypes;
using Bogus;
using FluentAssertions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FlyingDutchmanAirlines.Tests.Unit.ServiceLayerTests;

public sealed class BookingServiceTests
{
	private readonly Faker _faker = new ();
	private readonly BookingService _sut;
	private readonly BookingRepository _bookingRepository = Substitute.For<BookingRepository>();
	private readonly CustomerRepository _customerRepository = Substitute.For<CustomerRepository>();
	private readonly FlightRepository _flightRepository = Substitute.For<FlightRepository>();
	
	public BookingServiceTests()
	{
		_sut = new BookingService(_bookingRepository, _customerRepository, _flightRepository);
	}

	[Fact]
	public async Task CreateBooking_ShouldCreateBooking_WhenDataIsValidAndUserExists()
	{
		// Arrange
		var customer = new Customer(_faker.Name.FullName()) { CustomerId = _faker.Random.Int(1) };
		var flightNumber = _faker.Random.Int(1);
		
		_bookingRepository.CreateBooking(customer.CustomerId, flightNumber).Returns(Task.CompletedTask);
		_customerRepository.GetCustomerByName(customer.Name!).Returns(customer);
		_flightRepository.GetFlightByFlightNumber(flightNumber).Returns(new Flight());
		
		// Act
		(bool result, Exception? exception) = await _sut.CreateBooking(customer.Name!, flightNumber);
		
		// Assert
		result.Should().BeTrue();
		exception.Should().BeNull();
	}

	[Fact]
	public async Task CreateBooking_ShouldThrowNotFoundException_WhenDataIsValidAndUserDoesNotExist()
	{
		// Arrange
		var customer = new Customer(_faker.Name.FullName()) { CustomerId = _faker.Random.Int(1) };
		var flightNumber = _faker.Random.Int(1);

		_bookingRepository.CreateBooking(customer.CustomerId, flightNumber).Returns(Task.CompletedTask);
		_customerRepository.GetCustomerByName(customer.Name!).ThrowsAsync<CustomerNotFoundException>();
		
		// Act
		(bool result, Exception? exception) = await _sut.CreateBooking(customer.Name!, flightNumber);
		
		// Assert
		result.Should().BeFalse();
		exception
			.Should().NotBeNull()
			.And.BeOfType<CustomerNotFoundException>();
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData(null, -1)]
	[InlineData("Dummy", -1)]
	public async Task CreateBooking_ShouldThrowArgumentException_WhenDataIsInvalid(string customerName, int flightNumber)
	{
		// Act
		(bool result, Exception? exception) = await _sut.CreateBooking(customerName, flightNumber);
		
		// Assert
		exception.Should().NotBeNull();
		exception.Should().BeOfType(typeof(ArgumentException));
		result.Should().BeFalse();
	}

	[Fact]
	public async Task CreateBooking_ShouldThrowArgumentException_WhenRepositoryDataIsInvalid()
	{
		// Arrange
		var customer0 = new Customer(_faker.Name.FullName()) { CustomerId = 0 };
		var flightNumber = 1;
		_bookingRepository.CreateBooking(customer0.CustomerId, flightNumber).ThrowsAsync<ArgumentException>();
		_customerRepository.GetCustomerByName(customer0.Name!).Returns(customer0);
		_flightRepository.GetFlightByFlightNumber(flightNumber).Returns(new Flight());
		// Act
		(bool result0, Exception? exception0)  = await _sut.CreateBooking(customer0.Name!, 1);

		// Assert
		exception0
			.Should().NotBeNull()
			.And.BeOfType<ArgumentException>();
		result0.Should().BeFalse();
	}

	[Fact]
	public async Task CreateBooking_ShouldThrowCouldNotAddBookingException_WhenRepositoryFails()
	{
		// Arrange
		var customer1 = new Customer(_faker.Name.FullName()) { CustomerId = 1 };
		_bookingRepository.CreateBooking(customer1.CustomerId, 2).ThrowsAsync<CouldNotAddBookingToDatabaseException>();
		_customerRepository.GetCustomerByName(customer1.Name!).Returns(customer1);
		_flightRepository.GetFlightByFlightNumber(1).Returns(new Flight());
		// Act
		(bool result1, Exception? exception1) = await _sut.CreateBooking(customer1.Name!, 2);
		// Assert
		exception1
			.Should().NotBeNull()
			.And.BeOfType<CouldNotAddBookingToDatabaseException>();
		result1.Should().BeFalse();
	}

	[Fact]
	public async Task CreateBooking_ShouldThrowException_WhenFlightNotExists()
	{
		// Arrange
		var customer = new Customer(_faker.Name.FullName()){CustomerId = _faker.Random.Int(1)};
		var flightNumber = _faker.Random.Int(1);
		_customerRepository.GetCustomerByName(customer.Name!).Returns(customer);
		_flightRepository.GetFlightByFlightNumber(flightNumber).ThrowsAsync<FlightNotFoundException>();
		_bookingRepository.CreateBooking(customer.CustomerId, flightNumber).Returns(Task.CompletedTask);
		
		// Act
		(bool result, Exception? exception)  =await _sut.CreateBooking(customer.Name!, flightNumber);
		
		// Assert
		exception
			.Should().NotBeNull()
			.And.BeOfType<CouldNotAddBookingToDatabaseException>();
		result.Should().BeFalse();
	}

	[Fact]
	public async Task CreateBooking_ShouldThrowExceptionOtherThanNoFound_WhenSomethingWentWrong()
	{
		// Arrange
		var customer = new Customer(_faker.Name.FullName()) { CustomerId = _faker.Random.Int(1) };
		var flightNumber = _faker.Random.Int(1);

		_bookingRepository.CreateBooking(customer.CustomerId, flightNumber).Returns(Task.CompletedTask);
		_customerRepository.GetCustomerByName(customer.Name!).ThrowsAsync<ApplicationException>();
		
		// Act
		(bool result, Exception? exception) = await _sut.CreateBooking(customer.Name!, flightNumber);
		
		// Assert
		result.Should().BeFalse();
		exception
			.Should().NotBeNull()
			.And.BeOfType<Exception>();
	}
	
	[Fact]
	public async Task CreateBooking_ShouldThrowNotFoundException_WhenCreateCustomerReturnFalse()
	{
		// Arrange
		var customer = new Customer(_faker.Name.FullName()) { CustomerId = _faker.Random.Int(1) };
		var flightNumber = _faker.Random.Int(1);

		_bookingRepository.CreateBooking(customer.CustomerId, flightNumber).Returns(Task.CompletedTask);
		_customerRepository.GetCustomerByName(customer.Name!).ThrowsAsync<CustomerNotFoundException>();
		_customerRepository.CreateCustomer(customer.Name!).Returns(false);
		
		// Act
		(bool result, Exception? exception) = await _sut.CreateBooking(customer.Name!, flightNumber);
		
		// Assert
		result.Should().BeFalse();
		exception
			.Should().NotBeNull()
			.And.BeOfType<CustomerNotFoundException>();
	}
	
	[Fact]
	public async Task CreateBooking_ShouldThrowCouldNotAddBookingException_WhenCustomerNotInDatabase()
	{
		// Arrange
		var customer = new Customer(_faker.Name.FullName()) { CustomerId = _faker.Random.Int(1) };
		var flightNumber = _faker.Random.Int(1);

		_bookingRepository.CreateBooking(customer.CustomerId, flightNumber).ThrowsAsync<CouldNotAddBookingToDatabaseException>();
		_customerRepository.GetCustomerByName(customer.Name!).Returns(customer);
		_flightRepository.GetFlightByFlightNumber(flightNumber).Returns(new Flight());

		// Act
		(bool result, Exception? exception) = await _sut.CreateBooking(customer.Name!, flightNumber);
		
		// Assert
		result.Should().BeFalse();
		exception
			.Should().NotBeNull()
			.And.BeOfType<CouldNotAddBookingToDatabaseException>();
	}
}