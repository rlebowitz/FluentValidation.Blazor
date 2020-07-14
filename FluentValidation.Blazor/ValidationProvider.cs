using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Blazor.Interfaces;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Forms;

namespace FluentValidation.Blazor
{
    public class ValidationProvider : IValidationProvider
    {
        private static readonly char[] Separators = { '.', '[' };

        public void InitializeEditContext(EditContext editContext, IServiceProvider serviceProvider)
        {
            Guard.NotNull(editContext);
            Guard.NotNull(serviceProvider);

            var messages = new ValidationMessageStore(editContext);

            editContext.OnValidationRequested +=
                (sender, _) => ValidateModel(editContext, messages, serviceProvider);

            editContext.OnFieldChanged +=
                (sender, eventArgs) => ValidateField(editContext, messages, eventArgs.FieldIdentifier, serviceProvider);
        }

        private static async void ValidateModel(EditContext editContext, ValidationMessageStore messages, IServiceProvider serviceProvider)
        {
            Guard.NotNull(editContext);
            Guard.NotNull(messages);
            Guard.NotNull(serviceProvider);
            if (editContext.Model == null)
            {
                throw new NullReferenceException($"{nameof(editContext)}.{nameof(editContext.Model)}");
            }

            messages.Clear();
            editContext.NotifyValidationStateChanged();

            var validators = GetValidatorsForObject(editContext.Model, serviceProvider);

            dynamic context = GetValidationContext(editContext.Model);
            var validationResults = new List<ValidationResult>();
            foreach (var validator in validators)
            {
                var validationResult = await validator.ValidateAsync(context);
                validationResults.Add(validationResult);
            }

            var validationErrors = validationResults.SelectMany(result => result.Errors);
            foreach (var error in validationErrors)
            {
                //  var fieldIdentifier = new FieldIdentifier(editContext.Model, error.PropertyName);

                var fieldIdentifier = ToFieldIdentifier(editContext, error.PropertyName);
                messages.Add(fieldIdentifier, error.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static async void ValidateField(EditContext editContext, ValidationMessageStore messages,
            FieldIdentifier fieldIdentifier, IServiceProvider serviceProvider)
        {
            Guard.NotNull(editContext);
            Guard.NotNull(messages);
            Guard.NotNull(serviceProvider);
            if (editContext.Model == null)
            {
                throw new NullReferenceException($"{nameof(editContext)}.{nameof(editContext.Model)}");
            }

            dynamic validationContext = GetValidationContext(fieldIdentifier);

            messages.Clear(fieldIdentifier);
            editContext.NotifyValidationStateChanged();

            var validators = GetValidatorsForObject(fieldIdentifier.Model, serviceProvider);
            var validationResults = new List<ValidationResult>();

            foreach (var validator in validators)
            {
                var validationResult = await validator.ValidateAsync(validationContext);
                validationResults.Add(validationResult);
            }

            var errorMessages = validationResults
                .SelectMany(result => result.Errors)
                .Select(failure => failure.ErrorMessage)
            .Distinct();

            foreach (var errorMessage in errorMessages)
            {
                messages.Add(fieldIdentifier, errorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static IEnumerable<IValidator> GetValidatorsForObject(object model, IServiceProvider serviceProvider)
        {
            var repository = (ValidatorTypeRepository)serviceProvider.GetService(typeof(ValidatorTypeRepository));
            var validatorTypes = repository.GetValidatorTypes(model);
            var validators =
                validatorTypes.Select(validatorType => (IValidator)serviceProvider.GetService(validatorType));
            return validators;
        }

        private static object GetValidationContext(FieldIdentifier identifier)
        {
            var contextType = typeof(ValidationContext<>).MakeGenericType(identifier.Model.GetType());
            var properties = new[] { identifier.FieldName };
            var parameters = new List<object>
            {
                identifier.Model,
                new PropertyChain(),
                new MemberNameValidatorSelector(properties)
            };
            return Activator.CreateInstance(contextType, parameters.ToArray());
        }

        private static object GetValidationContext(object model)
        {
            var contextType = typeof(ValidationContext<>).MakeGenericType(model.GetType());
            var parameters = new List<object> { model };
            return Activator.CreateInstance(contextType, parameters.ToArray());
        }

        /// <summary>
        ///     Method that parses property paths
        ///     and returns a FieldIdentifier. which is an (instance, propName) pair
        /// </summary>
        /// <param name="editContext"></param>
        /// <param name="propertyPath"></param>
        /// <example>
        ///     For the property path 'SomeProp.MyCollection[123].ChildProp' the method returns the FieldIdentifier pair
        ///     (SomeProp.MyCollection[123], "ChildProp")
        /// </example>
        /// <returns>A <see cref="FieldIdentifier">FieldIdentifier</see> based on the specified property path.</returns>
        /// <remarks>https://gist.github.com/SteveSandersonMS/090145d7511c5190f62a409752c60d00#file-fluentvalidator-cs</remarks>
        private static FieldIdentifier ToFieldIdentifier(EditContext editContext, string propertyPath)
        {
            var obj = editContext.Model;

            while (true)
            {
                var nextTokenEnd = propertyPath.IndexOfAny(Separators);
                if (nextTokenEnd < 0)
                {
                    return new FieldIdentifier(obj, propertyPath);
                }

                var nextToken = propertyPath.Substring(0, nextTokenEnd);
                propertyPath = propertyPath.Substring(nextTokenEnd + 1);

                object newObj = null;
                if (nextToken.EndsWith("]"))
                {
                    // It's an indexer
                    // This code assumes C# conventions (one indexer named Item with one param)
                    nextToken = nextToken[..^1];
                    var prop = obj.GetType().GetProperty("Item");
                    if (prop != null)
                    {
                        var indexerType = prop.GetIndexParameters()[0].ParameterType;
                        var indexerValue = Convert.ChangeType(nextToken, indexerType);
                        newObj = prop.GetValue(obj, new[] { indexerValue });
                    }
                }
                else
                {
                    // It's a regular property
                    var prop = obj.GetType().GetProperty(nextToken);
                    if (prop == null)
                    {
                        throw new InvalidOperationException(
                            $"Could not find property named {nextToken} on object of type {obj.GetType().FullName}.");
                    }

                    newObj = prop.GetValue(obj);
                }

                if (newObj == null)
                {
                    // This is as far as we can go
                    return new FieldIdentifier(obj, nextToken);
                }

                obj = newObj;
            }
        }
    }
}