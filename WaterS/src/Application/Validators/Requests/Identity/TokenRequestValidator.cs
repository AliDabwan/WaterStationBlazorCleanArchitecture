using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Requests.Identity;

namespace WaterS.Application.Validators.Requests.Identity
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator(IStringLocalizer<TokenRequestValidator> localizer)
        {
            RuleFor(request => request.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email , User Name or Phone Number  is required"]);
              //.WithMessage(x => localizer["Email or User Name is not correct"]);
            RuleFor(request => request.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"]);
        }
    }
}
