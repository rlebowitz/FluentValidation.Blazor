using System;
using System.Threading.Tasks;
using FluentValidation.Blazor;
using FluentValidation.Blazor.Interfaces;

namespace Microsoft.AspNetCore.Components.Forms
{
    /// <summary>
    ///     The Razor component used to add FluentValidation support to a Blazor EditContext.
    /// </summary>
    /// <remarks>
    ///     Adopted namespace and naming convention from the Accelist.FluentValidation.Blazor project.
    ///     https://github.com/ryanelian/FluentValidation.Blazor/blob/master/FluentValidation.Blazor/FluentValidator.cs
    /// </remarks>
    public class FluentValidator : ComponentBase
    {
        [CascadingParameter] private EditContext CurrentEditContext { get; set; }
        [Inject] private IValidationProviderRepository Repository { get; set; }
        [Inject] private IServiceProvider ServiceProvider { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var previousEditContext = CurrentEditContext;

            await base.SetParametersAsync(parameters);

            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException("FluentValidator requires a cascading parameter of type {nameof(EditContext)}");
            }

            if (CurrentEditContext != previousEditContext)
            {
                EditContextChanged();
            }
        }

        private void EditContextChanged()
        {
            foreach (var providerType in Repository.Providers)
            {
                var validationProvider = (IValidationProvider) ServiceProvider.GetService(providerType);
                validationProvider.InitializeEditContext(CurrentEditContext, ServiceProvider);
            }
        }
    }
}