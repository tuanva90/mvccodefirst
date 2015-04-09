using System;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace Insyston.Operations.WPF.ViewModels.Common.Validation
{
    public static class FluentValidationExtensions
    {
        public static ValidationResult ValidateRecursive(this IValidator validator, object instance, params string[] properties)
        {
            var context = new ValidationContext(instance, new PropertyChain(), new RecursiveValidatorSelector(properties));
            return validator.Validate(context);
        }

        public static ValidationResult ValidateRecursive(this IValidator validator, object instance, params Expression<Func<object, object>>[] propertyExpressions)
        {
            var context = new ValidationContext(instance, new PropertyChain(), RecursiveValidatorSelector.FromExpressions(propertyExpressions));
            return validator.Validate(context);
        }
    }
}