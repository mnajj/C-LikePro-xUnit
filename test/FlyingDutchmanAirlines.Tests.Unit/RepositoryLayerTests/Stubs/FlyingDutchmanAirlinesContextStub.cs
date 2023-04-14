using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.Tests.Unit.RepositoryLayerTests.Stubs;

public class FlyingDutchmanAirlinesContextStub : FlyingDutchmanAirlinesContext
{
	public FlyingDutchmanAirlinesContextStub(DbContextOptions<FlyingDutchmanAirlinesContext> options)
		: base(options)
	{
		base.Database.EnsureDeleted();
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		var entityEntries = ChangeTracker
			.Entries()
			.Where(e => e.State == EntityState.Added).ToList();
		
		IEnumerable<Booking> bookings = entityEntries
			.Select(e => e.Entity).OfType<Booking>();
		if (bookings.Any(b => b.CustomerId != 1)) 
		{
			throw new Exception("Database Error!");
		}
		IEnumerable<Airport> airports = entityEntries
			.Select(e => e.Entity).OfType<Airport>();
		if (airports.Any(a=>a.AirportId == 10)) 
		{
			throw new Exception("Database Error!");
		}
		await base.SaveChangesAsync(cancellationToken);
		return 1;
	}
}