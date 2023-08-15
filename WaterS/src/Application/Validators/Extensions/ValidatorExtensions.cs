using FluentValidation;
using FluentValidation.Validators;
using WaterS.Application.Interfaces.Serialization.Serializers;

namespace WaterS.Application.Validators.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeJson<T>(this IRuleBuilder<T, string> ruleBuilder, IPropertyValidator<T, string> validator) where T : class
        {
            return ruleBuilder.SetValidator(validator);
        }
    }
}