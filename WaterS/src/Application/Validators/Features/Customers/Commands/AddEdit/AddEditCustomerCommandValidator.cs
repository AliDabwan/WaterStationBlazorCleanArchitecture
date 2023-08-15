using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.Customers.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.Customers.Commands.AddEdit
{
    public class AddEditCustomerCommandValidator : AbstractValidator<AddEditCustomerCommand>
    {
        public AddEditCustomerCommandValidator(IStringLocalizer<AddEditCustomerCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

            RuleFor(request => request.CompanyId)
             .Must(x => x!=0).WithMessage(x => localizer["CompanyId is required!"]);

            RuleFor(request => request.StationId)
             .Must(x => x != 0).WithMessage(x => localizer["StationId is required!"]);

            RuleFor(request => request.DriverId)
          .Must(x => x != 0).WithMessage(x => localizer["DriverId is required!"]);

            RuleFor(request => request.RegionId)
          .Must(x => x != 0).WithMessage(x => localizer["RegionId is required!"]);

            RuleFor(request => request.BottleNo)
       .Must(x => x != 0).WithMessage(x => localizer["BottleNo is required!"]);

            RuleFor(request => request.BottleTypeId)
               .Must(x => x!=0).WithMessage(x => localizer["BottleType is required!"]);


            RuleFor(request => request.Phone)
                                .Must(x => x!=null).WithMessage(x => localizer["Phone is required"])
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Phone is required"])
                                .Must(x => x.StartsWith("0")).WithMessage(x => localizer["يجب أن يبدء رقم الهاتف ب صفر"])
                .MinimumLength(11).WithMessage(localizer["رقم الهاتف يجب ان يكون 11 ارقام على الاقل"])
                                .MaximumLength(11).WithMessage(localizer["رقم الهاتف يجب ان يكون 11 ارقام على الاقل"])

                .Matches(@"[0-9]").WithMessage(localizer["ارقام فقط"]);







            RuleFor(request => request.LoginPassword)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"])
                .MinimumLength(4).WithMessage(localizer["Password must be at least of length 4"]);
                //.Matches(@"[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"])
                //.Matches(@"[0-9]").WithMessage(localizer["Password must contain at least one digit"]);
            //RuleFor(request => request.Description)
            //    .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);

        }
    }
}