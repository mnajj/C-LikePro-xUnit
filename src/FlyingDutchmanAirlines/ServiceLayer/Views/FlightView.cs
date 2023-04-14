namespace FlyingDutchmanAirlines.ServiceLayer.Views;

public struct FlightView
{
	public FlightView(string flightNumber, (string city, string code) origin, (string city, string code) destination)
	{
		FlightNumber = string.IsNullOrEmpty(flightNumber) ?
			"No flight number found" : flightNumber;
		Origin = new AirportInfo(origin);
		Destination = new AirportInfo(destination);
	}

	public string FlightNumber { get; init; }
	public AirportInfo Origin { get; init; }
	public AirportInfo Destination { get; init; }
}


public struct AirportInfo
{
	public string City { get; set; }
	public string Code { get; set; }

	public AirportInfo((string city, string code) airport)
	{
		City =string.IsNullOrEmpty(airport.city) ?
			"No city found" : airport.city;
		Code = string.IsNullOrEmpty(airport.code) ?
			"No code found" : airport.code; 
	}
}