using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.Stations.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.Stations.Commands.AddEdit
{
    public class AddEditStationsCommandValidator : AbstractValidator<AddEditStationCommand>
    {
        public AddEditStationsCommandValidator(IStringLocalizer<AddEditStationsCommandValidator> localizer)
        {
          
            RuleFor(request => request.CompanyId)
              .Must(x => x!=0).WithMessage(x => localizer["CompanyId is required!"]);
            RuleFor(request => request.Name)
                  .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

          //  RuleFor(request => request.LoginName)
          //.Must(x => !x.Contains(" ")).WithMessage(x => localizer["Space is Not Allowed"]);

            RuleFor(request => request.Phone)
                 .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Phone is required"])
          .MinimumLength(10).WithMessage(localizer["Phone must be at least of length 10"]);

            RuleFor(request => request.LoginName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["UserName is required"])
                .MinimumLength(4).WithMessage(localizer["UserName must be at least of length 4"]);
            RuleFor(request => request.LoginPassword)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"])
                .MinimumLength(4).WithMessage(localizer["Password must be at least of length 4"]);

        }
    }
}