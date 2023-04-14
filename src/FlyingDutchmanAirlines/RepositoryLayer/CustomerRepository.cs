using System.Reflection;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class CustomerRepository
{
	private readonly FlyingDutchmanAirlinesContext _context;
	
	public CustomerRepository(FlyingDutchmanAirlinesContext context)
	{
		_context = context;
	}

	public CustomerRepository()
	{
		if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
		{
			throw new Exception("This constructor should only be used for testing");
		}
	}
	
	public virtual async Task<bool> CreateCustomer(string name)
	{
		if (IsInvalidCustomerName(name)) return false;
		var customer = new Customer(name);
		using (_context)
		{
			try
			{ 
			await _context.Customers.AddAsync(customer);
				await _context.SaveChangesAsync();
			}
			catch (Exception e)
			{
				return false;
			}
		}
		return true;
	}

	private bool IsInvalidCustomerName(string name)
	{
		char[] forbiddenCharacters = {'!', '@', '#', '$', '%', '&', '*'};
		return string.IsNullOrEmpty(name) ||
			name.Any(c => forbiddenCharacters.Contains(c));
	}

	public virtual async Task<Customer> GetCustomerByName(string name)
	{
		if (IsInvalidCustomerName(name))
		{
			throw new CustomerNotFoundException();
		}
		return await _context.Customers.FirstOrDefaultAsync(c => c.Name == name) 
		       ?? throw new CustomerNotFoundException();
	}
}