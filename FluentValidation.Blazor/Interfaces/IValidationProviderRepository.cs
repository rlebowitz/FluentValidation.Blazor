using System;
using System.Collections.Generic;

namespace FluentValidation.Blazor.Interfaces
{
    public interface IValidationProviderRepository
    {
        IEnumerable<Type> Providers { get; }
        IValidationProviderRepository Add(Type providerType);
        IValidationProviderRepository Remove(Type providerType);
    }
}
