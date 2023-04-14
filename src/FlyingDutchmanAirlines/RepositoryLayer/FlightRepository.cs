using System.Reflection;
using System.Runtime.CompilerServices;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class FlightRepository
{
	private readonly FlyingDutchmanAirlinesContext _context;

	public FlightRepository(FlyingDutchmanAirlinesContext context)
	{
		_context = context;
	}
	
	public FlightRepository()
	{
		if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
		{
			throw new Exception("This constructor should only be used for testing");
		}
	}

	public virtual async Task<Flight> GetFlightByFlightNumber(int flightNumber)
	{
		if (!flightNumber.IsPositive())
		{
			Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! flightNumber = {flightNumber}");
			throw new FlightNotFoundException();	
		}
		try
		{
			return await _context.Flights
				       .FirstOrDefaultAsync(f => 
					       f.FlightNumber == flightNumber) ?? 
			       throw new FlightNotFoundException();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new CouldNotAddFlightToDatabaseException();
		}
	}

	public virtual Queue<Flight> GetFlights()
	{
		Queue<Flight> flights = new Queue<Flight>(_context.Flights.Count());
		foreach (Flight flight in _context.Flights)
		{
			flights.Enqueue(flight);
		}
		return flights;
	}
}