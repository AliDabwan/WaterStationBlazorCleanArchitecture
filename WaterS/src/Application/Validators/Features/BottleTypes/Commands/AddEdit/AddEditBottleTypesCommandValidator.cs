using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.Bottletypes.Commands.AddEdit
{
    public class AddEditBottleTypesCommandValidator : AbstractValidator<AddEditBottleTypeCommand>
    {
        public AddEditBottleTypesCommandValidator(IStringLocalizer<AddEditBottleTypesCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            //RuleFor(request => request.Description)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            RuleFor(request => request.FillDays)
              .Must(x =>x!=0).WithMessage(x => localizer["FillDays is required!"]);
            RuleFor(request => request.Price)
              .Must(x => x>=0).WithMessage(x => localizer["Price Greater or equal than zero "]);

        }
    }
}