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

namespace WaterS.Application.Features.Stations.Commands.AddEdit
{
    public partial class AddEditStationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ResName { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public int AccountId { get; set; }

        public string Userid { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public int No { get; set; }
        public string KindType { get; set; }
        public string KindTypeAr { get; set; }
        public int MyCompanyId { get; set; }
        public int MyStationId { get; set; }
        public int CompanyId { get; set; }
        public virtual Company myCompany { get; set; }
    }

    internal class AddEditStationCommandHandler : IRequestHandler<AddEditStationCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditStationCommandHandler> _localizer; 
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;

        public AddEditStationCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, UserManager<BlazorHeroUser> userManager, IStringLocalizer<AddEditStationCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadService = uploadService;
            this.userManager = userManager;
            _localizer = localizer;
        }
        private async Task<Result<int>> AddBasicUser(string Roool, string Name, string FirstName, string Email, string Phone, string Passw, string KindType, string KindTypeAr, int KindId, int stationId,int compNo)
        {

            //Check if Role Exists

            //Check if User Exists
            var basicUser = new BlazorHeroUser
            {
                FirstName = FirstName,
                LastName = " ",
                Email = Phone+"@roiraq.com",
                UserName = Name,

                EmailConfirmed = true,
                IsActive = true,
                KindType = KindType,
                KindTypeAr = KindTypeAr,
                KindId = KindId,
                StationId = stationId,
                PhoneNumber = Phone
            };
            var basicUserInDbEmail = await userManager.FindByEmailAsync(basicUser.Email);
            if (basicUserInDbEmail != null)
            {
                return await Result<int>.FailAsync(_localizer["رقم الهاتف مكرر"]);


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
                CategoryType = RoleConstants.StationRoleAr,
                AccountType = 0,
                UserId = basicUser.Id,
                No = 1021000 + compNo,
                StationId= stationId,
                    CompanyId = KindId,
              


            };

            var result = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().AddAsync(myacc);
            await _unitOfWork.Commit();

            var station = await _unitOfWork.Repository<Station>().GetByIdAsync(stationId);
            if (station != null)
            {
                station.AccountId = myacc.Id;
                await _unitOfWork.Repository<Station>().UpdateAsync(station);
                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.Commit();// (cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);

                basicUser.AccountId = myacc.Id;
                await userManager.UpdateAsync(basicUser);

            }



            return await Result<int>.SuccessAsync(_localizer["New User Saved"]);

            // logger.LogInformation(_localizer["Seeded User with Basic Role."]);

            //}).GetAwaiter().GetResult();
        }

        public async Task<Result<int>> Handle(AddEditStationCommand command, CancellationToken cancellationToken)
        {

            //if (await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().Entities.Where(p => p.Id != command.Id)
            //    .AnyAsync(p => p.CompanyId == command.CompanyId && p.RegionId==command.RegionId, cancellationToken))
            //{
            //    return await Result<int>.FailAsync(_localizer["already exists."]);
            //}




            if (command.Id == 0)
            {

                var Company = _mapper.Map<Domain.Entities.Catalog.Station>(command);

            var check = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().GetAllAsync();


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
            var basicUserInDbEmail = await userManager.FindByEmailAsync(Company.Phone+"@roiraq.com");
            if (basicUserInDbEmail != null)
            {
                return await Result<int>.FailAsync(_localizer["Phone Repeated"]);


            }
            var basicUserInDbName = await userManager.FindByNameAsync(Company.LoginName);
            if (basicUserInDbName != null)
            {
                return await Result<int>.FailAsync(_localizer["LoginName Repeated"]);

            }
            Company.No = maxvalue + 1;
                Company.MyCompanyId = Company.CompanyId;

            var resultCompany = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().AddAsync(Company);



            //await _unitOfWork.Commit(cancellationToken);

            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllStationsCacheKey);


          
                var responseUser = await AddBasicUser(Company.KindType, Company.LoginName, Company.Name, Company.Email, Company.Phone, Company.LoginPassword, Company.KindType, Company.KindTypeAr, Company.CompanyId, Company.Id, Company.No);




            //await _unitOfWork.Rollback();


            return await Result<int>.SuccessAsync(Company.Id, _localizer["Saved"]);
        }
            else
            {
                var Station = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().GetByIdAsync(command.Id);
                if (Station != null)
                {
                    Station.CompanyId = command.CompanyId ;
                    Station.Name = command.Name;
                    Station.Adress = command.Adress;
                    Station.Email = command.Phone+"@roiraq.com";
                    Station.Phone = command.Phone;
                    Station.ResName = command.ResName;
                    var prevLoginName = Station.LoginName;

                    Station.LoginName = command.LoginName;
                    var prevPassword = Station.LoginPassword;
                    Station.LoginPassword = command.LoginPassword;

                 var myuser = await userManager.FindByNameAsync(prevLoginName);
                    if (myuser==null)
                    {
                        return await Result<int>.FailAsync(_localizer["لايوجد يوزر بهذا الاسم"]);

                    }
                    myuser.UserName = Station.LoginName;
                    myuser.PhoneNumber = Station.Phone;
                    myuser.Email = Station.Email;

                    var myuserEm = await userManager.FindByEmailAsync(Station.Email);
                    if (myuserEm != null && myuserEm.Id != myuser.Id)
                    {
                        return await Result<int>.FailAsync(_localizer["الايميل مسجل مع حساب اخر" + Station.Email]);

                    }
                    var myuserName = await userManager.FindByNameAsync(myuser.UserName);
                    if (myuserName != null && myuserName.Id != myuser.Id)
                    {
                        return await Result<int>.FailAsync(_localizer["اسم المستخدم مسجل مع حساب اخر" + myuser.UserName]);

                    }

                    Station.Userid = myuser.Id;


                    var CustomerAcc = await _unitOfWork.Repository<Accounts>().GetByIdAsync(command.AccountId);

                    if (CustomerAcc == null)
                    {
                        return await Result<int>.FailAsync(_localizer["لايوجد حساب مرتبط ! لم يتم التعديل"]);

                    }
                    CustomerAcc.Name = command.Name;
                    await _unitOfWork.Repository<Accounts>().UpdateAsync(CustomerAcc);

                    await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().UpdateAsync(Station);


                    try
                    {
                        await userManager.UpdateAsync(myuser);
                        await userManager.ChangePasswordAsync(myuser, prevPassword, Station.LoginPassword);



                    }
                    catch
                    {

                        return await Result<int>.FailAsync(_localizer["لم يتم تعديل بيانات اليوزر"]);
                    }

                    await _unitOfWork.Commit(cancellationToken);
                    return await Result<int>.SuccessAsync(Station.Id, _localizer["Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Not Found!"]);
                }
            }
        }
    }
}