using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Blazor.Interfaces;

namespace FluentValidation.Blazor
{
    public class ValidationProviderRepository : IValidationProviderRepository
    {
        private Type[] _providers = Array.Empty<Type>();
        public IEnumerable<Type> Providers => _providers;

        public IValidationProviderRepository Add(Type providerType)
        {
            Guard.NotNull((providerType));
            if (!typeof(IValidationProvider).IsAssignableFrom(providerType))
                throw new ArgumentException($"{providerType.Name} does not implement {nameof(IValidationProvider)}");

            _providers = _providers.Concat(new Type[] { providerType }).ToArray();
            return this;
        }

        public IValidationProviderRepository Remove(Type providerType)
        {
            _providers = _providers.Where(type => type != providerType).ToArray();
            return this;
        }
    }
}
