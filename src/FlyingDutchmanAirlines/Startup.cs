using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingDutchmanAirlines;

public class Startup
{
	public void Configure(IApplicationBuilder app)
	{
		app.UseRouting();
		app.UseEndpoints(e => e.MapControllers());
		app.UseSwagger();
		app.UseSwaggerUI(s =>
			s.SwaggerEndpoint("/swagger/v1/swagger.json", 
				"Flying Dutchman Airlines"));
	}
	
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();
		services.AddTransient<FlightService>();
		services.AddTransient<BookingService>();
		
		services.AddTransient<FlightRepository>();
		services.AddTransient<AirportRepository>();
		services.AddTransient<BookingRepository>();
		services.AddTransient<CustomerRepository>();
		
		services.AddTransient<FlyingDutchmanAirlinesContext>();

		services.AddSwaggerGen();
	}
}