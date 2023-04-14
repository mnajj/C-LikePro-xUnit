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
public sealed class AirportRepositoryTests
{
	private readonly AirportRepository _sut;
	private readonly FlyingDutchmanAirlinesContext _context;

	public AirportRepositoryTests()
	{
		_context = new FlyingDutchmanAirlinesContextStub(
			new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
				.UseInMemoryDatabase("FlyingDutchman").Options);
		_sut = new AirportRepository(_context);
		_sut.Should().NotBeNull();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public async Task GetAirport_ShouldReturnAirport_WhenDataIsValid(int id)
	{
		// Arrange
		var airports = new SortedList<string, Airport>
		{
			{ "GOH",
				new Airport
				{
					AirportId = 0,
					City = "Nuuk",
					Iata = "GOH" } },
			{
				"PHX",
				new Airport
				{
					AirportId = 1,
					City = "Phoenix",
					Iata = "PHX"
				}
			},
			{
				"DDH",
				new Airport
				{
					AirportId = 2,
					City = "Bennington",
					Iata = "DDH"
				}
			},
			{
				"RDU",
				new Airport
				{
					AirportId = 3,
					City = "Raleigh-Durham",
					Iata = "RDU"
				}
			}
		};
		_context.Airports.AddRange(airports.Values);
		await _context.SaveChangesAsync();
		// Act
		var result = await _sut.GetAirportById(id);
		
		// Assert
		result
			.Should()
			.BeEquivalentTo(
			airports.Values.FirstOrDefault(a => a.AirportId == id));
	}

	[Fact]
	public async Task GetAirportById_ShouldThrowArgumentException_WhenDataIsInvalid()
	{
		// Arrange
		var outputStream = new StringWriter();
		Console.SetOut(outputStream); 
		
		// Act
		var result = async () => await _sut.GetAirportById(-1); 
		
		// Assert
			try
			{
				await result.Should().ThrowAsync<ArgumentException>();
			}
			catch (Exception e)
			{
				outputStream.ToString().Should().Contain("Argument Exception in GetAirportByID! AirportID = -1");
				throw;
			}
			finally
			{
				await outputStream.DisposeAsync();
				RecoverStandardOutputStream();
			}
	}

	[Fact]
	public async Task GetAirport_ShouldThrowDataBaseException_WhenIdAboveNine()
	{
		// Act
		var result = async () => await _sut.GetAirportById(10);
		// Assert
		await result.Should().ThrowAsync<CouldNotReadAirportFromDatabaseException>();
	}
	private void RecoverStandardOutputStream()
	{ 
		var standardOutput = new StreamWriter(Console.OpenStandardOutput()); 
		standardOutput.AutoFlush = true; 
		Console.SetOut(standardOutput);
	}
}