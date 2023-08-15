using FluentValidation;
using Microsoft.Extensions.Localization;
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;

namespace WaterS.Application.Validators.Features.CustomerPhones.Commands.AddEdit
{
    public class AddEditCustomerPhoneCommandValidator : AbstractValidator<AddEditCustomerPhoneCommand>
    {
        public AddEditCustomerPhoneCommandValidator(IStringLocalizer<AddEditCustomerPhoneCommandValidator> localizer)
        {
           

            RuleFor(request => request.PhoneNumber)
                                .Must(x => x!=null).WithMessage(x => localizer["رقم الهاتف فارغ"])
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["رقم الهاتف فارغ"])
                                //.Must(x => x.StartsWith("0")).WithMessage(x => localizer["يجب أن يبدء رقم الهاتف ب صفر"])
                .MinimumLength(11).WithMessage(localizer["رقم الهاتف يجب ان يكون 11 ارقام على الاقل"])
                                //.MaximumLength(11).WithMessage(localizer["رقم الهاتف يجب ان يكون 11 ارقام على الاقل"])

                .Matches(@"[0-9]").WithMessage(localizer["ارقام فقط"]);

           

        }
    }
}