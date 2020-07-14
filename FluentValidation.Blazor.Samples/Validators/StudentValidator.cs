using FluentValidation.Blazor.Samples.Models;

namespace FluentValidation.Blazor.Samples.Validators
{
    public class StudentValidator : AbstractValidator<Student>
    {
        public StudentValidator()
        {
            RuleFor(customer => customer.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(customer => customer.LastName).NotEmpty().MaximumLength(50);
            RuleFor(customer => customer.Grade).ExclusiveBetween(1,12);
        }
    }
}
