using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.DriverRegions.Commands.AddEdit
{
    public class AddEditDriverRegionCommandValidator : AbstractValidator<AddEditDriverRegionCommand>
    {
        public AddEditDriverRegionCommandValidator(IStringLocalizer<AddEditDriverRegionCommandValidator> localizer)
        {

            RuleFor(request => request.RegionId)
                .Must(x => x != 0).WithMessage(x => localizer["يجب إختيار المنطقة"]);
            RuleFor(request => request.DriverId)
                .Must(x => x!=0).WithMessage(x => localizer["يجب إختيار السائق"]);
        }
    }
}