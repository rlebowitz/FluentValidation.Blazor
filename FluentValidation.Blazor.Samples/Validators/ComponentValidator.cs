using FluentValidation.Blazor.Samples.Models;

namespace FluentValidation.Blazor.Samples.Validators
{
    public class ComponentValidator : AbstractValidator<ComponentModel>
    {
        public ComponentValidator()
        {

            RuleFor(component => component.Student).SetValidator(new StudentValidator());
            RuleFor(teacher => teacher.Teacher).NotEmpty().MaximumLength(50);
        }
    }
}
