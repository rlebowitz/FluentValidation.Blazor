using FluentValidation.Blazor.Samples.Models;

namespace FluentValidation.Blazor.Samples.Validators
{
    public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(address => address.Line1).NotEmpty();
        RuleFor(address => address.City).NotEmpty();
        RuleFor(address => address.Postcode).NotEmpty().MaximumLength(10);
    }
}
}
