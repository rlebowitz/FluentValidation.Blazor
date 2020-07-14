using System;
using FluentValidation.Blazor.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation.Blazor.Configuration
{
    public class ValidationConfiguration
    {
        public IServiceCollection Services { get; }
        public IValidationProviderRepository Repository { get; }

        public ValidationConfiguration(IServiceCollection services, IValidationProviderRepository repository)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
