using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation.Blazor.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation.Blazor.Extensions
{
    public static class ValidationConfigurationExtensions
    {
        public static ValidationConfiguration AddFluentValidation(this ValidationConfiguration config)
        {
            var assemblies = GetAssembliesForSolution();
            ScanForValidators(config.Services, assemblies);
            config.Services.AddScoped<ValidationProvider>();
            config.Repository.Add(typeof(ValidationProvider));
            return config;
        }

        //https://stackoverflow.com/questions/851248/c-sharp-reflection-get-all-active-assemblies-in-a-solution/10253634
        private static IEnumerable<Assembly> GetAssembliesForSolution()
        {
            var list = new List<Assembly>();
            var mainAssembly = Assembly.GetEntryAssembly();
            if (mainAssembly == null)
            {
                return list;
            }

            list.Add(mainAssembly);
            list.AddRange(mainAssembly.GetReferencedAssemblies().Select(Assembly.Load));
            return list.Where(a => a.FullName != null && (!a.FullName.StartsWith("System.") && !a.FullName.StartsWith("Microsoft.")));
        }

        private static void ScanForValidators(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            Guard.NotNull(services);
            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            Guard.NotNull(enumerable);
            var validatorDictionary = new Dictionary<Type, List<Type>>();  // stores the type(s) of validators used for each class type.

            var validatorTypes = enumerable
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition)
                .Where(x =>
                    x.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                );

            foreach (var validatorType in validatorTypes)
            {
                if (validatorType.BaseType == null)
                {
                    continue;
                }

                var genericArgumentType = validatorType.BaseType.GetGenericArguments()[0];
                if (!validatorDictionary.TryGetValue(genericArgumentType, out var validatorTypeList))
                {
                    validatorTypeList = new List<Type>();
                    validatorDictionary[genericArgumentType] = validatorTypeList;
                }
                validatorTypeList.Add(validatorType);
            }

            var repository = new ValidatorTypeRepository(
                validatorDictionary.Select(pair => new KeyValuePair<Type, IEnumerable<Type>>(pair.Key, pair.Value))
            );

            validatorDictionary
                .SelectMany(x => x.Value)
                .Distinct()
                .ToList()
                .ForEach(x => services.AddScoped(x));

            services.AddSingleton(repository);
        }
    }
}