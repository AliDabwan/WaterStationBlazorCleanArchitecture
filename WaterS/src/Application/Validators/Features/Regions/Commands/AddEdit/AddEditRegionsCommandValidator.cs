using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.Regions.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.Regions.Commands.AddEdit
{
    public class AddEditRegionsCommandValidator : AbstractValidator<AddEditRegionCommand>
    {
        public AddEditRegionsCommandValidator(IStringLocalizer<AddEditRegionsCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
        
         
        }
    }
}