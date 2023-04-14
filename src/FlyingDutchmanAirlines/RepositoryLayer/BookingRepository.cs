using System.Reflection;
using System.Runtime.CompilerServices;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class BookingRepository
{
	private readonly FlyingDutchmanAirlinesContext _context;

	public BookingRepository(FlyingDutchmanAirlinesContext context)
	{
		_context = context;
	}
	
	[MethodImpl(MethodImplOptions.NoInlining)]
	public BookingRepository()
	{
		if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
		{
			throw new Exception("This constructor should only be used for testing");
		}
	}

	public virtual async Task CreateBooking(int customerId, int flightNumber)
	{
		if (!customerId.IsPositive() || !flightNumber.IsPositive())
		{
			Console.WriteLine($"Argument Exception in CreateBooking! CustomerID = {customerId}, flightNumber = {flightNumber}");
			throw new ArgumentException("Invalid arguments provided");
		}

		var newBooking = new Booking
		{
			CustomerId = customerId,
			FlightNumber = flightNumber
		};
		try
		{
			await _context.Bookings.AddAsync(newBooking);
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			Console.WriteLine($"Exception during database query: {e.Message}");
			throw new CouldNotAddBookingToDatabaseException();
		}
	}
}