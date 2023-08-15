using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Requests.Identity;

namespace WaterS.Application.Validators.Requests.Identity
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IStringLocalizer<UpdateProfileRequestValidator> localizer)
        {
            RuleFor(request => request.FirstName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["First Name is required"]);
            
        }
    }
}