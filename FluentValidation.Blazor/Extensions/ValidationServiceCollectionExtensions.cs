using System;
using FluentValidation.Blazor.Configuration;
using FluentValidation.Blazor.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation.Blazor.Extensions
{
    public static class ValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddFormValidation(this IServiceCollection instance, Action<ValidationConfiguration> config = null)
        {
            var repository = new ValidationProviderRepository();
            instance.AddScoped<IValidationProviderRepository>((_) => repository);
            if (config != null)
            {
                var c = new ValidationConfiguration(instance, repository);
                config(c);
            }
            return instance;
        }
    }
}
