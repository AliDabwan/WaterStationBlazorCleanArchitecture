using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.AccountMovments.Commands.AddEdit
{
    public class AddEditAccountMovmentsCommandValidator : AbstractValidator<AddEditAccountMovmentCommand>
    {
        public AddEditAccountMovmentsCommandValidator(IStringLocalizer<AddEditAccountMovmentsCommandValidator> localizer)
        {
        
            //RuleFor(request => request.EntryType)
            //  .Must(x =>x!=0).WithMessage(x => localizer["EntryType is required!"]);
         
        }
    }
}