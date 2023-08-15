using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Requests;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Drivers.Commands.AddEdit
{
    public partial class AddEditDriverCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        //[Required]
        public string Name { get; set; }
        public string ResName { get; set; }
        public string Adress { get; set; }
        //[Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        public int AccountId { get; set; }

        public string Userid { get; set; }
        //[Required]
        public string LoginName { get; set; }
        //[Required]
        public string LoginPassword { get; set; }
        public int No { get; set; }
        public string KindType { get; set; }
        public string KindTypeAr { get; set; }
        //[Required]
        public int StationId { get; set; }
        public virtual Station myStation { get; set; }
        //[Required]
        public int CompanyId { get; set; }
        public virtual Company myCompany { get; set; }
    }

    internal class AddEditDriverCommandHandler : IRequestHandler<AddEditDriverCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditDriverCommandHandler> _localizer; 
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;

        public AddEditDriverCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, UserManager<BlazorHeroUser> userManager, IStringLocalizer<AddEditDriverCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadService = uploadService;
            this.userManager = userManager;
            _localizer = localizer;
        }
        private async Task<Result<int>> AddBasicUser(string Roool, string Name, string FirstName, string Email, string Phone,
            string Passw, string KindType, string KindTypeAr, int KindId, int StationId,int DriverId,int compNo)
        {

            //Check if Role Exists

            //Check if User Exists
            var basicUser = new BlazorHeroUser
            {
                FirstName = FirstName,
                LastName = " ",
                Email = Phone + "@roiraq.com",
                UserName = Name,

                EmailConfirmed = true,
                IsActive = true,
                KindType = KindType,
                KindTypeAr = KindTypeAr,
                KindId = KindId,
                StationId = StationId,
                DriverId = DriverId,
                PhoneNumber = Phone
            };
            var basicUserInDbEmail = await userManager.FindByEmailAsync(basicUser.Email);
            if (basicUserInDbEmail != null)
            {
                return await Result<int>.FailAsync(_localizer["Phone Repeated"]);


            }
            var basicUserInDbName = await userManager.FindByNameAsync(basicUser.UserName);
            if (basicUserInDbName != null)
            {
                return await Result<int>.FailAsync(_localizer["LoginName Repeated"]);

            }

            await userManager.CreateAsync(basicUser, Passw);
            await userManager.AddToRoleAsync(basicUser, Roool);


            var myacc = new Accounts()
            {
                Name = FirstName,
                CategoryType = RoleConstants.DriverRoleAr,
                AccountType = 0,
                UserId = basicUser.Id,
                No = 1022000 + compNo,
                DriverId= DriverId,
                    CompanyId = KindId,
                StationId = StationId,


            };

            var result = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().AddAsync(myacc);
            await _unitOfWork.Commit();

            var driver = await _unitOfWork.Repository<Driver>().GetByIdAsync(DriverId);
            if (driver != null)
            {
                driver.AccountId = myacc.Id;
                await _unitOfWork.Repository<Driver>().UpdateAsync(driver);
                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.Commit();// (cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);


                basicUser.AccountId = myacc.Id;
                await userManager.UpdateAsync(basicUser);

            }

            return await Result<int>.SuccessAsync(_localizer["New User Saved"]);

            // logger.LogInformation(_localizer["Seeded User with Basic Role."]);

            //}).GetAwaiter().GetResult();
        }

        public async Task<Result<int>> Handle(AddEditDriverCommand command, CancellationToken cancellationToken)
        {

            //if (await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().Entities.Where(p => p.Id != command.Id)
            //    .AnyAsync(p => p.CompanyId == command.CompanyId && p.RegionId==command.RegionId, cancellationToken))
            //{
            //    return await Result<int>.FailAsync(_localizer["already exists."]);
            //}




            if (command.Id == 0)
            {

                var Company = _mapper.Map<Domain.Entities.Catalog.Driver>(command);

            var check = await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().GetAllAsync();


            var myCompanyType = check.FindAll(x => x.KindType == Company.KindType);

            int maxvalue = int.MinValue;
            if (myCompanyType == null || myCompanyType.Count == 0)
            {
                maxvalue = 0;
            }
            else
            {
                foreach (var item in myCompanyType)
                {
                    if (item.Name == Company.Name)
                    {
                        return await Result<int>.FailAsync(_localizer["Repeated"]);

                    }
                    if (item.Phone == Company.Phone)
                    {
                        return await Result<int>.FailAsync(_localizer["Phone Repeated"]);

                    }
                    if (item.No > maxvalue)
                    {
                        maxvalue = item.No;

                    }
                }
            }
                var basicUserInDbEmail = await userManager.FindByEmailAsync(Company.Phone + "@roiraq.com");
                if (basicUserInDbEmail != null)
            {
                return await Result<int>.FailAsync(_localizer["Email Repeated"]);


            }
            var basicUserInDbName = await userManager.FindByNameAsync(Company.LoginName);
            if (basicUserInDbName != null)
            {
                return await Result<int>.FailAsync(_localizer["LoginName Repeated"]);

            }
            Company.No = maxvalue + 1;

            var resultCompany = await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().AddAsync(Company);



            //await _unitOfWork.Commit(cancellationToken);

            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDriversCacheKey);


          
                var responseUser = await AddBasicUser(Company.KindType, Company.LoginName, Company.Name, Company.Email, Company.Phone, Company.LoginPassword, Company.KindType, Company.KindTypeAr, Company.CompanyId, Company.StationId, Company.Id, Company.No);




            //await _unitOfWork.Rollback();


            return await Result<int>.SuccessAsync(Company.Id, _localizer["Saved"]);
        }
            else
            {
                var Driver = await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().GetByIdAsync(command.Id);
                if (Driver != null)
                {
                    Driver.CompanyId = command.CompanyId ;
                    Driver.StationId = command.StationId ;
                    Driver.Name = command.Name;
                    Driver.Adress = command.Adress;
                    Driver.Email = command.Phone + "@roiraq.com";
                    var prevLoginName = Driver.LoginName;

                    Driver.Phone = command.Phone;
                    Driver.LoginName = command.LoginName;
                    var prevPassword = Driver.LoginPassword;
                    Driver.LoginPassword = command.LoginPassword;
                    var myuser = await userManager.FindByNameAsync(prevLoginName);
                    if (myuser == null)
                    {
                        return await Result<int>.FailAsync(_localizer["لايوجد يوزر بهذا الاسم"]);

                    }
                    myuser.UserName = Driver.LoginName;
                    myuser.PhoneNumber = Driver.Phone;
                    myuser.Email = Driver.Email;


                    var myuserEm = await userManager.FindByEmailAsync(Driver.Email);
                    if (myuserEm != null && myuserEm.Id != myuser.Id)
                    {
                        return await Result<int>.FailAsync(_localizer["الايميل مسجل مع حساب اخر" + Driver.Email]);

                    }
                    var myuserName = await userManager.FindByNameAsync(myuser.UserName);
                    if (myuserName != null && myuserName.Id != myuser.Id)
                    {
                        return await Result<int>.FailAsync(_localizer["اسم المستخدم مسجل مع حساب اخر" + myuser.UserName]);

                    }

                    Driver.Userid = myuser.Id;




                    var CustomerAcc = await _unitOfWork.Repository<Accounts>().GetByIdAsync(command.AccountId);

                    if (CustomerAcc == null)
                    {
                        return await Result<int>.FailAsync(_localizer["لايوجد حساب مرتبط ! لم يتم التعديل"+ command.AccountId]);

                    }
                    CustomerAcc.Name = command.Name;
                    await _unitOfWork.Repository<Accounts>().UpdateAsync(CustomerAcc);

                    await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().UpdateAsync(Driver);


                    try
                    {
                        await userManager.UpdateAsync(myuser);
                        await userManager.ChangePasswordAsync(myuser, prevPassword, Driver.LoginPassword);



                    }
                    catch
                    {

                        return await Result<int>.FailAsync(_localizer["لم يتم تعديل بيانات اليوزر"]);
                    }


                    await _unitOfWork.Commit(cancellationToken);
                    return await Result<int>.SuccessAsync(Driver.Id, _localizer["Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Not Found!"]);
                }
            }
        }
    }
}