using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Chat;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Models.Chat;
using WaterS.Application.Requests.Identity;
using WaterS.Domain.Contracts;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;
using WaterS.Shared.Wrapper;
namespace WaterS.Application.Features.Companies.Commands.AddEdit
{
    public partial class AddEditCompanyCommand : IRequest<Result<int>>
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
        public int MyCompanyID { get; set; }
        public int MystationID { get; set; }
        public bool IsActive { get; set; }

        public DateTime ActivateDate { get; set; }
        public DateTime EndDate { get; set; }
    }





    internal class AddEditCompanyCommandHandler : IRequestHandler<AddEditCompanyCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditCompanyCommandHandler> _localizer;
        //private readonly ICurrentUserService currentUser;
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditCompanyCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditCompanyCommandHandler> localizer, 
           UserManager<BlazorHeroUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            //this.currentUser = currentUser;
            this.userManager = userManager;
        }

        private async Task<Result<int>> AddBasicUser(string Roool,string Name,string FirstName,string Email,string Phone,string Passw,string KindType, string KindTypeAr, int KindId,int stationId,int compNo)
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
                    KindTypeAr= KindTypeAr,
                    KindId=KindId,
                    StationId= stationId,
                    PhoneNumber= Phone
                                  };
                var basicUserInDbEmail = await userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDbEmail!=null)
                {
                return await Result<int>.FailAsync(_localizer["رقم الهاتف مكرر"]);


            }
            var basicUserInDbName = await userManager.FindByNameAsync(basicUser.UserName);
                if (basicUserInDbName != null)
                {
                    return await Result<int>.FailAsync(_localizer["اسم المستخدم موجود بحساب اخر"]);

                }
              
                    await userManager.CreateAsync(basicUser,Passw);
                    await userManager.AddToRoleAsync(basicUser, Roool);

            var myacc = new Accounts()
            {
                Name =FirstName,
                CategoryType = RoleConstants.ManagerRoleAr,
                AccountType = 0,
                UserId = basicUser.Id,
                No = 102000 + compNo,
                CompanyId= KindId


            };

            var result = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().AddAsync(myacc);
            await _unitOfWork.Commit();

            var Company = await _unitOfWork.Repository<Company>().GetByIdAsync(KindId);
            if (Company != null)
            {
                Company.AccountId = myacc.Id;
                await _unitOfWork.Repository<Company>().UpdateAsync(Company);
                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.Commit();// (cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);

                basicUser.AccountId = myacc.Id;
                await userManager.UpdateAsync(basicUser);


            }








            return await Result<int>.SuccessAsync( _localizer["تم إضافة اليوزر بنجاح"]);

            // logger.LogInformation(_localizer["Seeded User with Basic Role."]);

            //}).GetAwaiter().GetResult();
        }


        public async Task<Result<int>> Handle(AddEditCompanyCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                
                   var Company = _mapper.Map<Company>(command);
                
                var check = await _unitOfWork.Repository<Company>().GetAllAsync();


                var myCompanyType = check.FindAll(x=>x.KindType== Company.KindType);
              
                int maxvalue = int.MinValue;
                if (myCompanyType==null || myCompanyType.Count==0)
                {
                    maxvalue = 0;
                }
                else
                {
                    foreach (var item in myCompanyType)
                    {
                        if (item.Name == Company.Name)
                        {
                            return await Result<int>.FailAsync(_localizer["هذا الإسم مكرر"]);

                        }
                        if (item.Phone == Company.Phone)
                        {
                            return await Result<int>.FailAsync(_localizer["رقم الجوال مكرر"]);

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
                    return await Result<int>.FailAsync(_localizer["الهاتف موجود بحساب اخر"]);


                }
                var basicUserInDbName = await userManager.FindByNameAsync(Company.LoginName);
                if (basicUserInDbName != null)
                {
                    return await Result<int>.FailAsync(_localizer["اسم المستخدم موجود بحساب اخر"]);

                }
                Company.No = maxvalue + 1;
                var resultCompany = await _unitOfWork.Repository<Company>().AddAsync(Company);


                


                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);


                if (Company.KindType == "Manager")
                {
                    var responseUser = await AddBasicUser(Company.KindType, Company.LoginName, Company.Name,
                        Company.Email, Company.Phone, Company.LoginPassword, Company.KindType, Company.KindTypeAr, Company.Id, 0,Company.No);


                }


                //else if (Company.KindType == "Station")
                //{
                //    var responseUser = await AddBasicUser(Company.KindType, Company.LoginName, Company.Name, Company.Email, Company.Phone, Company.LoginPassword, Company.KindType, Company.KindTypeAr, Company.MyCompanyID, Company.Id);

                //}
                //else if (Company.KindType == "Driver")
                //{
                //    var responseUser = await AddBasicUser(Company.KindType, Company.LoginName, Company.Name, Company.Email, Company.Phone, Company.LoginPassword, Company.KindType, Company.KindTypeAr, Company.MyCompanyID, Company.MystationID);

                //}
             



                //await _unitOfWork.Rollback();


                return await Result<int>.SuccessAsync(Company.Id,_localizer["تم إضافة الشركة بنجاح"]);

            }
            else
            {
                var Company = await _unitOfWork.Repository<Company>().GetByIdAsync(command.Id);
                if (Company != null)
                {


                    Company.Name = command.Name;
                    Company.Adress = command.Adress;
                    Company.Email = command.Phone + "@roiraq.com";
                    Company.Phone = command.Phone;
                    Company.ResName = command.ResName;
                    Company.IsActive = command.IsActive;
                    Company.ActivateDate = command.ActivateDate;
                    Company.EndDate = command.EndDate;
                    var prevLoginName = Company.LoginName;

                    Company.LoginName = command.LoginName;
                    var prevPassword = Company.LoginPassword;
                    Company.LoginPassword = command.LoginPassword;

                    var myuser = await userManager.FindByNameAsync(prevLoginName);
                    if (myuser == null)
                    {
                        return await Result<int>.FailAsync(_localizer["لايوجد يوزر بهذا الاسم"]);

                    }
                    myuser.UserName = Company.LoginName;
                    myuser.PhoneNumber = Company.Phone;
                    myuser.Email = Company.Email;



                    await _unitOfWork.Repository<Company>().UpdateAsync(Company);
                    //await _unitOfWork.Commit(cancellationToken);
                    try
                    {
                        await userManager.UpdateAsync(myuser);
                        await userManager.ChangePasswordAsync(myuser, prevPassword, Company.LoginPassword);



                    }
                    catch
                    {

                        return await Result<int>.FailAsync(_localizer["لم يتم تعديل بيانات اليوزر"]);
                    }

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);
                    return await Result<int>.SuccessAsync(Company.Id, _localizer["تم التعديل"]+"   "+command.Id);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["لايوجد تفاصيل"]);
                }
            }
        }
    }
}
public class BlazorHeroUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string CreatedBy { get; set; }

    [Column(TypeName = "text")]
    public string ProfilePictureDataUrl { get; set; }

    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string KindType { get; set; }//Company/Station/Driver/Admin/Owner
    public string KindTypeAr { get; set; }//
    public int KindId { get; set; }//
    public int StationId { get; set; }//
    public int DriverId { get; set; }//
    public int CustomerId { get; set; }//
    public int AccountId { get; set; }//

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryFromUsers { get; set; }
    public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryToUsers { get; set; }

    public BlazorHeroUser()
    {
        ChatHistoryFromUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
        ChatHistoryToUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
    }
}
