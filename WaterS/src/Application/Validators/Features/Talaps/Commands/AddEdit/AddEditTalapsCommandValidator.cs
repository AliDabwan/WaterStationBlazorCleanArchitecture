using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.Talaps.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.Talaps.Commands.AddEdit
{
    public class AddEditTalapsCommandValidator : AbstractValidator<AddEditTalapCommand>
    {
        public AddEditTalapsCommandValidator(IStringLocalizer<AddEditTalapsCommandValidator> localizer)
        {

            RuleFor(request => request.BottleNo)
           .Must(x => x != 0).WithMessage(x => localizer["يجب إدخال رقم الخزان"]);
          
            RuleFor(request => request.CompanyId)
             .Must(x => x!=0).WithMessage(x => localizer["CompanyId is required!"]);

            RuleFor(request => request.StationId)
             .Must(x => x != 0).WithMessage(x => localizer["StationId is required!"]); 

            RuleFor(request => request.DriverId)
             .Must(x => x != 0).WithMessage(x => localizer["DriverId is required!"]);

            RuleFor(request => request.CustomerId)
           .Must(x => x != 0).WithMessage(x => localizer["يجب تحديد الزبون "]);


            //RuleFor(request => request.Description)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);

        }
    }
}