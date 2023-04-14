using System.Reflection;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class AirportRepository
{
	private readonly FlyingDutchmanAirlinesContext _context;
	
	public AirportRepository(FlyingDutchmanAirlinesContext context)
	{
		_context = context;
	}
	
	public AirportRepository()
	{
		if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
		{
			throw new Exception("This constructor should only be used for testing");
		}
	}
	
	public virtual async Task<Airport> GetAirportById(int id)
	{
		if (id < 0)
		{
			Console.WriteLine($"Argument Exception in GetAirportByID! AirportID = {id}");
			throw new ArgumentException("Invalid argument provided");
		}
		try
		{
			return await _context.Airports.FirstOrDefaultAsync(a=>a.AirportId==id)
				?? throw new AirportNotFoundException();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new CouldNotReadAirportFromDatabaseException();
		}
	}
}