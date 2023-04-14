using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FlyingDutchmanAirlines.ControllerLayer.Contracts;

public sealed record BookingRequest : IValidatableObject
{
	private string? _firstName;
	private string? _lastName;
	
	public string? FirstName
	{
		get => _firstName!;
		init => _firstName = ValidateName(value, nameof(FirstName));
	}

	public string? LastName
	{
		get => _lastName!;
		init => _lastName = ValidateName(value, nameof(LastName));
	}

	private string ValidateName(string? value, string propertyName) =>
		string.IsNullOrEmpty(value) ? 
			throw new InvalidOperationException("could not set " + propertyName)
			: value;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var errors = new List<ValidationResult>();
		if (FirstName is null)
		{
			errors.Add(new ValidationResult("FirstName is null"));
		}
		if (LastName is null)
		{
			errors.Add(new ValidationResult("LastName is null"));
		}

		return errors;
	}
}