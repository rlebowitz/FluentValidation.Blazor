using FluentValidation.Blazor.Samples.Models;

namespace FluentValidation.Blazor.Samples.Validators
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(customer => customer.LastName).NotEmpty().MaximumLength(50);
            RuleFor(customer => customer.Address1).SetValidator(new AddressValidator());
            RuleFor(customer => customer.Address2).SetValidator(new AddressValidator());
        }
    }
}
