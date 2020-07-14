using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FluentValidation.Blazor
{
    internal class ValidatorTypeRepository
    {
        private readonly ReadOnlyDictionary<Type, IEnumerable<Type>> _validatorTypesDictionary;

        public ValidatorTypeRepository(IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> validatorTypes)
        {
            Guard.NotNull(validatorTypes);
            _validatorTypesDictionary = new ReadOnlyDictionary<Type, IEnumerable<Type>>(
                validatorTypes.ToDictionary(pair => pair.Key, pair => pair.Value));
        }

        /// <summary>Returns the Types of validators associated with the specified entity model.</summary>
        /// <param name="model">The specified entity model.</param>
        /// <returns>An enumerable of the various validator Types associated with the specified entity model.</returns>
        public IEnumerable<Type> GetValidatorTypes(object model)
        {
            Guard.NotNull(model);
            var validatorType = model.GetType();
            if (!_validatorTypesDictionary.TryGetValue(validatorType, out var validatorTypes))
            {
                return Array.Empty<Type>();
            }

            return validatorTypes;
        }
    }
}