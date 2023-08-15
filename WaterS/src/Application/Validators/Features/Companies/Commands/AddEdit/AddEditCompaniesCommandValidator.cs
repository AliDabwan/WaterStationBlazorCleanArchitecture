using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.Companies.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.Companies.Commands.AddEdit
{
    public class AddEditCompaniesCommandValidator : AbstractValidator<AddEditCompanyCommand>
    {
        public AddEditCompaniesCommandValidator(IStringLocalizer<AddEditCompaniesCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        
            RuleFor(request => request.Phone)
                 .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Phone is required"])
          .MinimumLength(10).WithMessage(localizer["Phone must be at least of length 10"]);
            RuleFor(request => request.LoginName)
               .Must(x => !x.Contains(" ")).WithMessage(x => localizer["Space is Not Allowed"]);

            RuleFor(request => request.LoginName)
                .Must(x => !string.IsNullOrWhiteSpace(x) ).WithMessage(x => localizer["UserName is required"])

                .MinimumLength(4).WithMessage(localizer["UserName must be at least of length 4"]);
            RuleFor(request => request.LoginPassword)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"])
                .MinimumLength(4).WithMessage(localizer["Password must be at least of length 4"]);


                //.Matches(@"[A-Z]").WithMessage(localizer["Password must contain at least one capital letter"])
                //.Matches(@"[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"])
                //.Matches(@"[0-9]").WithMessage(localizer["Password must contain at least one digit"]);
            //RuleFor(request => request.Description)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);

        }
    }
}