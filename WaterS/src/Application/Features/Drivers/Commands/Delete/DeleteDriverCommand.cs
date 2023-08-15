using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;
using WaterS.Application.Features.Companies.Commands.AddEdit;
namespace WaterS.Application.Features.Drivers.Commands.Delete
{
    public class DeleteDriverCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteDriverCommandHandler> _localizer;
        private readonly UserManager<BlazorHeroUser> userManager;

        public DeleteDriverCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteDriverCommandHandler> localizer, UserManager<BlazorHeroUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            this.userManager = userManager;
        }

        public async Task<Result<int>> Handle(DeleteDriverCommand command, CancellationToken cancellationToken)
        {
            var Customer =  _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().Entities
              .FirstOrDefault(x => x.DriverId == command.Id);
            if (Customer != null)
            {
               
                    return await Result<int>.FailAsync(_localizer["لا يمكن الحذف ! يوجد زبائن مرتبطين بهذا السائق"]);

                

            }


           

                var Driver =  _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().Entities.FirstOrDefault(x => x.Id == command.Id);

            if (Driver != null)
            {
                var DriverReg = await _unitOfWork.Repository<Domain.Entities.Catalog.DriverRegion>().Entities.Where(x => x.DriverId == command.Id).ToListAsync();

                var myuser = userManager.Users.FirstOrDefault(x => x.DriverId == command.Id && x.KindType == "Driver");
              
                //var myuser = await userManager.FindByIdAsync(Driver.Userid);

                if (myuser == null)
                {

                    int reg = 0;
                    foreach (var item in DriverReg)
                    {
                        reg++;
                        await _unitOfWork.Repository<Domain.Entities.Catalog.DriverRegion>().DeleteAsync(item);

                    }

                    await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().DeleteAsync(Driver);
                    await _unitOfWork.Commit(cancellationToken);

                    if (reg > 0)
                    {
                        return await Result<int>.SuccessAsync(Driver.Id, _localizer["@تم حذف السائق" + " والغاء ارتباطه بعدد " + reg + " مناطق "]);

                    }
                    else
                    {
                        return await Result<int>.SuccessAsync(Driver.Id, _localizer["تم حذف السائق@"]);

                    }
                }
                else

                {


                    myuser.IsActive = false;
                     await userManager.UpdateAsync(myuser);
                    int reg = 0;
                    foreach (var item in DriverReg)
                    {
                        reg++;
                        await _unitOfWork.Repository<Domain.Entities.Catalog.DriverRegion>().DeleteAsync(item);

                    }

                    await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().DeleteAsync(Driver);
                    await _unitOfWork.Commit(cancellationToken);




                    var deletedUserRoll = await userManager.RemoveFromRoleAsync(myuser, "Driver");
                    try
                    {
                        var deletedUser = await userManager.DeleteAsync(myuser);

                    }
                    catch
                    {

                        return await Result<int>.SuccessAsync(Driver.Id, _localizer["تم حذف السائقErr"]);
                    }

                    //if (deletedUserRoll.Succeeded)
                    //{
                    //var deletedUser = await userManager.DeleteAsync(myuser);
                    //if (deletedUser.Succeeded)
                    //{


                    //    int reg3 = 0;
                    //    foreach (var item in DriverReg)
                    //    {
                    //        reg3++;
                    //        await _unitOfWork.Repository<Domain.Entities.Catalog.DriverRegion>().DeleteAsync(item);

                    //    }

                    //    await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().DeleteAsync(Driver);
                    //    await _unitOfWork.Commit(cancellationToken);

                    //    if (reg3 > 0)
                    //    {
                    //        return await Result<int>.SuccessAsync(Driver.Id, _localizer["تم حذف السائق" + " والغاء ارتباطه بعدد " + reg + " مناطق "]);

                    //    }
                    //    else
                    //    {
                    //        return await Result<int>.SuccessAsync(Driver.Id, _localizer["تم حذف السائق"]);

                    //    }

                    //}
                    return await Result<int>.SuccessAsync(Driver.Id, _localizer["تم حذف السائق"]);

                        //return await Result<int>.FailAsync(_localizer["@لم يتم الحذف بسبب اليوزر "]);

                    //}
                    //else
                    //{
                    //    //var deletedUser = await userManager.DeleteAsync(myuser);

                    //    return await Result<int>.FailAsync(_localizer["لم يتم الحذف ! انتظر قليلا ثم حاول مرة اخرى "]);

                    //}


                }
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Driver Not Found!"]);
            }
        }
    }
}