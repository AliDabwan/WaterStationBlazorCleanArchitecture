using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Customers.Commands.Delete
{
    public class DeleteCustomerCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteCustomerCommandHandler> _localizer;
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;

        public DeleteCustomerCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteCustomerCommandHandler> localizer  ,UserManager<BlazorHeroUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            this.userManager = userManager;

        }

        public async Task<Result<int>> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
        {
            var CustomersFilterSpec = new CustomerFilterSpecification("","");

            var Customer = await _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().Entities
                   .Specify(CustomersFilterSpec).Where(x => x.Id == command.Id).FirstOrDefaultAsync();


            if (Customer != null)
            {


                Customer.statue = "deleted";
                Customer.BottleNoStatue = "متاح";





                var talaps = await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().Entities
                   .Where(x => x.CustomerId == command.Id).ToListAsync();
                if (talaps != null)
                {
                    foreach (var item in talaps)
                    {
                        item.TalapStatue = "Deleted";
                        item.TalapStatueAr = "محذوف";
                        await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().UpdateAsync(item);

                    }
                }


                    var myuser = await userManager.FindByIdAsync(Customer.Userid);

                if (myuser != null)
                {
                    myuser.IsActive = false;
                    var deletedUser = await userManager.UpdateAsync(myuser);


                    if (deletedUser.Succeeded)
                    {
                        await _unitOfWork.Repository<Customer>().UpdateAsync(Customer);
                        var result = await _unitOfWork.CommitAndRemoveCache(cancellationToken);
                        if (result > 0)
                        {
                            return await Result<int>.SuccessAsync(Customer.Id, _localizer["تم حذف الزبون"]);

                        }
                        else
                        {
                            return await Result<int>.SuccessAsync(Customer.Id, _localizer["@تم حذف الزبون    "]);


                        }
                    }
                    else
                    {
                        return await Result<int>.FailAsync(_localizer["لايمكن الحذف " + " خطأ في بيانات الدخول "]);

                    }
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["لايمكن الحذف "+" لاتوجد بيانات لليوزر "]);


                }





                //var talaps = await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().Entities
                //   .Where(x => x.CustomerId == command.Id).ToListAsync();
                //if (talaps!=null)
                //{
                //    //foreach (var item in talaps)
                //    //{
                //    //    item.TalapStatue = "Deleted";
                //    //    item.TalapStatueAr = "محذوف";
                //    //    await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().UpdateAsync(item);

                //    //}


                //    return await Result<int>.FailAsync(_localizer["لايمكن الحذف يوجد طلبات ماء مرتبطة بهذا الزبون"]);

                //}
                //if (Customer.CustomerPhones.Count>0)
                //{

                //    var CustomerPhones = await _unitOfWork.Repository<Domain.Entities.Catalog.CustomerPhone>().Entities
                //           .Where(x => x.CustomerId == command.Id).ToListAsync();

                //    if (CustomerPhones!=null)
                //    {
                //        foreach (var item in CustomerPhones)
                //        {
                //            await _unitOfWork.Repository<Domain.Entities.Catalog.CustomerPhone>().DeleteAsync(item);

                //        }

                //    }
                //}

                //var myuser = await userManager.FindByIdAsync(Customer.Userid);

                //if (myuser == null)
                //{
                //    var myacc = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().GetByIdAsync(Customer.AccountId);

                //    if (myacc != null)
                //    {
                //        await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().DeleteAsync(myacc);

                //    }
                //    await _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().DeleteAsync(Customer);
                //    await _unitOfWork.Commit(cancellationToken);
                //    return await Result<int>.SuccessAsync(Customer.Id, _localizer["Customer Deleted"]);

                //    //return await Result<int>.FailAsync(_localizer["لا يوجد يوزر خاص بهذا الحساب"]);

                //}
                //else
                //{
                //    var deletedUserRoll = await userManager.RemoveFromRoleAsync(myuser, "Customer");
                //    if (deletedUserRoll.Succeeded)
                //    {
                //        var deletedUser = await userManager.DeleteAsync(myuser);
                //        if (deletedUser.Succeeded)
                //        {
                //            var myacc = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().GetByIdAsync(Customer.AccountId);

                //            if (myacc != null)
                //            {
                //                await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().DeleteAsync(myacc);

                //            }

                //            await _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().DeleteAsync(Customer);
                //            await _unitOfWork.Commit(cancellationToken);
                //            return await Result<int>.SuccessAsync(Customer.Id, _localizer["Customer Deleted"]);

                //        }
                //        else
                //        {
                //            return await Result<int>.FailAsync(_localizer["خطا في حذف الحساب"]);

                //        }


                //    }
                //    else
                //    {
                //        return await Result<int>.FailAsync(_localizer["@خطا في حذف الحساب"]);

                //    }


                //}


            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Customer Not Found!"]);
            }
        }
    }
}