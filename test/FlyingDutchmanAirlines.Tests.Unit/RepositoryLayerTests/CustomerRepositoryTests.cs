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
public class CustomerRepositoryTests
{
	private readonly Faker _faker = new ();
	private readonly CustomerRepository _sut;
	private readonly FlyingDutchmanAirlinesContextStub _context;

	public CustomerRepositoryTests()
	{
		 _context = new FlyingDutchmanAirlinesContextStub(
			new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
				.UseInMemoryDatabase("FlyingDutchman").Options);
		_sut = new CustomerRepository(_context);
		_sut.Should().NotBeNull();
	}

	[Fact]
	public async Task CreateUser_ShouldCreateCustomer_WhenDataIsValid()
	{
		var created = await _sut.CreateCustomer(_faker.Name.FullName());
		created.Should().BeTrue();
	}

	[Fact]
	public async Task CreateCustomer_ShouldReturnFalse_WhenNameIsNullOrEmpty()
	{
		// Act
		var created = await _sut.CreateCustomer(null!);
		// Assert
		created.Should().BeFalse();
	}

	[Theory]
	[InlineData('@')]
	[InlineData('$')]
	[InlineData('*')]
	public async Task CreateCustomer_ShouldReturnFalse_WhenDataIsInvalid(char specialChar)
	{
		// Act
		var result = await _sut.CreateCustomer(_faker.Name.FullName() + specialChar);
		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public async Task CreateCustomer_ShouldThrowException_WhenContextIsNull()
	{
		// Arrange
		var sut = new CustomerRepository(null!);
		
		// Act
		var result = await sut.CreateCustomer(_faker.Name.FullName());
		
		// Assert
		sut.Should().NotBeNull();
		result.Should().BeFalse();
	}

	[Fact]
	public async Task GetCustomerByName_ShouldReturnCustomer_WhenDataIsValid()
	{
		// Arrange
		var name = _faker.Name.FullName();
		var customer = new Customer(name);
		await _context.Customers.AddAsync(customer);
		await _context.SaveChangesAsync();
		// Act
		var result = await _sut.GetCustomerByName(name);
		// Assert
		result.Should().BeEquivalentTo(customer);
	}

	[Theory]
	[InlineData('@')]
	[InlineData('$')]
	[InlineData('*')]
	public async Task GetCustomerByName_ShouldThrowNotFoundException_WhenNameIsInvalid(char invalid)
	{
		// Arrange
		await _context.Customers.AddAsync(new(_faker.Name.FullName()));
		await _context.SaveChangesAsync();
		
		// Act
		var result = async () => await _sut.GetCustomerByName(_faker.Name.FullName() + invalid);
		
		// Assert
		await result.Should().ThrowAsync<CustomerNotFoundException>();
	}
}