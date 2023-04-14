using System.Reflection;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer.Views;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class FlightService
{
	private readonly FlightRepository _flightRepository;
	private readonly AirportRepository _airportRepository;

	public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
	{
		_flightRepository = flightRepository;
		_airportRepository = airportRepository;
	}
	
	public FlightService()
	{
		if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
		{
			throw new Exception("This constructor should only be used for testing");
		}
	}
	
	public virtual async IAsyncEnumerable<FlightView> GetFlights()
	{
		Queue<Flight> flights = _flightRepository.GetFlights();
		foreach (var flight in flights)
		{
			Airport origin;
			Airport destination;
			try
			{
				origin = await _airportRepository.GetAirportById(flight.Origin);
				destination = await _airportRepository.GetAirportById(flight.Destination);
			}
			catch (AirportNotFoundException)
			{
				throw new FlightNotFoundException();
			}
			catch (Exception)
			{
				throw new ArgumentException();
			}
			yield return new FlightView(flight.FlightNumber.ToString(),
				(origin.City, origin.Iata),
				(destination.City, destination.Iata));
		}
	}
	
	public virtual async Task<FlightView> GetFlightByFlightNumber(int number)
	{
		try
		{
			var flight = await _flightRepository.GetFlightByFlightNumber(number);
			var origin = await _airportRepository.GetAirportById(flight.Origin);
			var destination = await _airportRepository.GetAirportById(flight.Destination);
		return new FlightView(flight.FlightNumber.ToString(),
		(origin.City, origin.Iata),
			(destination.City, destination.Iata));
		}
		catch (Exception ex) when(ex is AirportNotFoundException || ex is FlightNotFoundException)
		{
			throw new FlightNotFoundException();
		}
		catch (Exception)
		{
			throw new ArgumentException();
		}	
	}
}