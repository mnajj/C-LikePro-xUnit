namespace FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;

public sealed class Customer
{
	public Customer(string name) => (Name) = (name);
	
    public int CustomerId { get; set; }

    public string? Name { get; set; }

    public ICollection<Booking> Bookings { get; } = new List<Booking>();
}
