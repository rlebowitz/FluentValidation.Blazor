using System;
using Microsoft.AspNetCore.Components.Forms;

namespace FluentValidation.Blazor.Interfaces
{
    public interface IValidationProvider
    {
        void InitializeEditContext(EditContext editContext, IServiceProvider serviceProvider);
    }
}
