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

namespace WaterS.Application.Features.Customers.Commands.AddEdit
{
    public partial class AddEditCustomerCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Userid { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public int AccountId { get; set; }
        public int No { get; set; }
        public int BottleNo { get; set; }

        //[Required]
        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int RegionId { get; set; }
        public int BottleTypeId { get; set; }
        public string statue { get; set; }
        public string BottleNoStatue { get; set; }

        public virtual Company Company { get; set; }
        public virtual Station Station { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Region Region { get; set; }
        public virtual BottleType BottleType { get; set; }

    }

    internal class AddEditCustomerCommandHandler : IRequestHandler<AddEditCustomerCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditCustomerCommandHandler> _localizer; 
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;

        public AddEditCustomerCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, UserManager<BlazorHeroUser> userManager, IStringLocalizer<AddEditCustomerCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadService = uploadService;
            this.userManager = userManager;
            _localizer = localizer;
        }
        private async Task<Result<int>> AddBasicUser(string Roool, string Name, string FirstName, string Email, string Phone, string Passw, string KindType, string KindTypeAr, int KindId,
            int StationId,int DriverId,int CustomerId ,int compNo,int accountId)
        {

            //Check if Role Exists

            //Check if User Exists
            //Phone.Replace("0", "");
           
            var basicUser = new BlazorHeroUser
            {
                FirstName = FirstName,
                LastName = " ",

                Email = Phone + "@roiraq.com",
                UserName = Phone,

                EmailConfirmed = true,
                IsActive = true,
                KindType = KindType,
                KindTypeAr = KindTypeAr,
                KindId = KindId,
                StationId = StationId,
                DriverId = DriverId,
                CustomerId = CustomerId,
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

            if (accountId == 0)
            {


                var myacc = new Accounts()
                {
                    Name = FirstName,
                    CategoryType = RoleConstants.CustomerRoleAr,
                    AccountType = 0,
                    UserId = basicUser.Id,
                    No = 1023000 + compNo,
                    CustomerId = CustomerId,
                    CompanyId = KindId,
                    StationId = StationId,
                    DriverId = DriverId



                };
                var result = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().AddAsync(myacc);
                await _unitOfWork.Commit();
                accountId = myacc.Id;

            }
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(CustomerId);
            if (customer != null)
            {
                customer.AccountId = accountId;
                customer.Userid = basicUser.Id;
                await _unitOfWork.Repository<Customer>().UpdateAsync(customer);
                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.Commit();// (cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);

                basicUser.AccountId = accountId;
                await userManager.UpdateAsync(basicUser);

                return await Result<int>.SuccessAsync(_localizer["New User Saved"]);

            }
            else
            {
                return await Result<int>.FailAsync(_localizer["لم يتم تحديث الحساب بنجاح"]);

            }

            // logger.LogInformation(_localizer["Seeded User with Basic Role."]);

            //}).GetAwaiter().GetResult();
        }

        public async Task<Result<int>> Handle(AddEditCustomerCommand command, CancellationToken cancellationToken)
        {

            //if (await _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().Entities.Where(p => p.Id != command.Id)
            //    .AnyAsync(p => p.CompanyId == command.CompanyId && p.RegionId==command.RegionId, cancellationToken))
            //{
            //    return await Result<int>.FailAsync(_localizer["already exists."]);
            //}




            if (command.Id == 0)
            {

                var Company = _mapper.Map<Customer>(command);



                Company.Email = Company.Phone + "@roiraq.com";
                var check = await _unitOfWork.Repository<Customer>().Entities.ToListAsync();


                var myCompanyType = check.FindAll(x => x.CompanyId == Company.CompanyId);

                int maxvalue = int.MinValue;
                if (myCompanyType == null || myCompanyType.Count == 0)
                {
                    maxvalue = 0;
                }
                else
                {
                    foreach (var item in myCompanyType)
                    {
                        if (item.Name == Company.Name && item.statue != "deleted")
                        {
                            return await Result<int>.FailAsync(_localizer["Customer Repeated"]);

                        }
                        if (item.Phone == Company.Phone)
                        {
                            return await Result<int>.FailAsync(_localizer["Phone Repeated"]);

                        }
                        if (item.Email == Company.Email)
                        {
                            return await Result<int>.FailAsync(_localizer["Email Repeated"]);

                        }
                        if (item.BottleNo == Company.BottleNo && item.StationId == Company.StationId && item.statue != "deleted")
                        {
                            return await Result<int>.FailAsync(_localizer["BottleNo Repeated"]);

                        }

                        if (item.No > maxvalue)
                        {
                            maxvalue = item.No;

                        }
                    }

                  var xx=  myCompanyType.Where(x=>x.BottleNo == command.BottleNo && x.StationId == command.StationId && x.statue == "deleted").ToList();
                    int isfound = 0;
                    foreach (var item in xx)
                    {
                        if (item.BottleNoStatue == "مستخدم")
                        {
                            isfound++;

                        }
                        if (item.BottleNoStatue == "متاح")
                        {
                            isfound=0;

                        }

                    }


                    if (isfound>0)
                    {
                        return await Result<int>.FailAsync(_localizer["BottleNo Repeated"]);

                    }
                }


                var basicUserInDbEmail = await userManager.FindByEmailAsync(Company.Email);
                if (basicUserInDbEmail != null)
                {
                    return await Result<int>.FailAsync(_localizer["Email Repeated"]);


                }
                var basicUserInDbName = await userManager.FindByNameAsync(Company.Phone);
                if (basicUserInDbName != null)
                {
                    return await Result<int>.FailAsync(_localizer["LoginName Repeated"]);

                }
                Company.No = maxvalue + 1;
                Company.LoginName = Company.Phone;
                Company.statue = "saved";


                var resultCompany = await _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().AddAsync(Company);




                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCustomersCacheKey);


                var customers = check.FindAll(x => x.CompanyId == Company.CompanyId);

                if (customers != null)
                {
               
                    foreach (var item in customers)
                    {
                    
                        if (item.BottleNo == command.BottleNo && item.StationId == command.StationId && item.statue == "deleted")
                        {
                            item.BottleNoStatue = "مستخدم";
                            await _unitOfWork.Repository<Customer>().UpdateAsync(item);

                        }
                      
                    }
                }


                var newPhone = new CustomerPhone()
                {
                    CustomerId = Company.Id,
                    PhoneNumber= Company.Phone,
                    CompanyId= Company.CompanyId,
                    StationId = Company.StationId,
                    DriverId = Company.DriverId,
                    Description= "الجوال الاساسي",
                    



                };

                if (newPhone!=null)
                {
                    var resultPhone= await _unitOfWork.Repository<Domain.Entities.Catalog.CustomerPhone>().AddAsync(newPhone);

                }
                //await _unitOfWork.Commit(cancellationToken);

                //await _unitOfWork.Commit(cancellationToken);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCustomersCacheKey);


                
                var responseUser = await AddBasicUser("Customer", Company.Phone, Company.Name, Company.Email,
                    Company.Phone, Company.LoginPassword, "Customer", "زبون", Company.CompanyId, Company.StationId, 
                    Company.DriverId, Company.Id, Company.No,0);



                if (responseUser.Succeeded)
                {
                    return await Result<int>.SuccessAsync(Company.Id, _localizer["تم إضافة الزبون :" + Company.Name]);

                }
                else
                {
                 await _unitOfWork.Repository<Customer>().DeleteAsync(Company);
                    await _unitOfWork.Commit();

                    return await Result<int>.FailAsync(_localizer["لم يتم إضافة الزبون بنجاح يرجى تصحيح البيانات "]);

                }


                //await _unitOfWork.Rollback();


                //return await Result<int>.SuccessAsync(Company.Id, _localizer["تم إضافة الزبون :"+ Company.Name]);
            }
            else
            {


                var Customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(command.Id);


                if (Customer != null)
                {


                    var check = await _unitOfWork.Repository<Customer>().Entities.ToListAsync();



                    int maxvalue = int.MinValue;
                    if (check == null || check.Count == 0)
                    {
                        return await Result<int>.FailAsync(_localizer["No Data"]);
                    }
                    else
                    {
                        foreach (var item in check)
                        {
                            if (item.Name == command.Name && item.Id != command.Id && item.statue != "deleted")
                            {
                                return await Result<int>.FailAsync(_localizer["Customer Repeated"]);

                            }
                            //if (item.BottleNo == command.BottleNo && item.StationId == command.StationId && item.Id != command.Id && item.BottleNoStatue == "مستخدم")
                            //{
                            //    return await Result<int>.FailAsync(_localizer["BottleNo Repeated"]);

                            //}
                            if (item.Phone == command.Phone && item.Id != command.Id)
                            {
                                return await Result<int>.FailAsync(_localizer["Phone Repeated"]);

                            }
                            if (item.BottleNo == command.BottleNo && item.StationId == command.StationId && item.Id != command.Id && item.statue != "deleted")
                            {
                                return await Result<int>.FailAsync(_localizer["BottleNo Repeated"]);

                                //item.BottleNoStatue = "مستخدم";
                                //await _unitOfWork.Repository<Customer>().UpdateAsync(item);

                            }


                        }


                        var xx = check.Where(x => x.BottleNo == command.BottleNo && x.StationId == command.StationId && x.Id != command.Id && x.statue == "deleted").ToList();
                        int isfound = 0;
                        foreach (var item in xx)
                        {
                            if (item.BottleNoStatue == "مستخدم")
                            {
                                isfound++;

                            }
                            if (item.BottleNoStatue == "متاح")
                            {
                                isfound = 0;
                                item.BottleNoStatue = "مستخدم";
                                await _unitOfWork.Repository<Customer>().UpdateAsync(item);

                            }

                        }



                        if (Customer.BottleNo== command.BottleNo)
                        {
                            isfound = 0;


                        }

                        if (isfound > 0)
                        {
                            return await Result<int>.FailAsync(_localizer["BottleNo Repeated"]);

                        }

                    
                    }


                            Customer.CompanyId = command.CompanyId;
                    Customer.StationId = command.StationId;
                    Customer.DriverId = command.DriverId;
                    Customer.RegionId = command.RegionId;
                    Customer.Name = command.Name;
                    Customer.Adress = command.Adress;
                    Customer.Email = command.Phone + "@roiraq.com";
                    Customer.BottleNo = command.BottleNo;
                    Customer.BottleTypeId = command.BottleTypeId;
                    //Customer.AccountId = command.AccountId;
                    var prevLoginName =string.IsNullOrEmpty( Customer.LoginName)? Customer.Phone: Customer.LoginName;

                    Customer.LoginName =  command.Phone ;
                    var prevPhone = Customer.Phone;

                    Customer.Phone = command.Phone;
                    var prevPassword = Customer.LoginPassword;
                    Customer.LoginPassword = command.LoginPassword;
                    var myuser = await userManager.FindByNameAsync(prevLoginName);
                    if (myuser == null)
                    {


                        var responseUser = await AddBasicUser("Customer", Customer.Phone, Customer.Name, Customer.Email,
                            Customer.Phone, Customer.LoginPassword, "Customer", "زبون", Customer.CompanyId,
                            Customer.StationId, Customer.DriverId, Customer.Id, Customer.No,Customer.AccountId);

                        if (!responseUser.Succeeded)
                        {
                            return await Result<int>.FailAsync(_localizer[ responseUser.Messages[0]]);

                        }
                        myuser = await userManager.FindByNameAsync(Customer.LoginName);
                        if (myuser == null)
                        {
                            return await Result<int>.FailAsync(_localizer["بيانات الحساب غير صحيحة يرجى حذف هذا الزبون وإضافته مرة أخرى" + prevLoginName]);

                        }
                    }

                    myuser.UserName = Customer.Phone;
                    myuser.PhoneNumber = Customer.Phone;
                    myuser.Email = Customer.Email;
                    var myuserEm = await userManager.FindByEmailAsync(Customer.Email);
                    if (myuserEm != null && myuserEm.Id!= myuser.Id )
                    {
                        return await Result<int>.FailAsync(_localizer["رقم الهاتف مسجل مع حساب اخر" + Customer.Phone]);

                    }

                    var myuserName = await userManager.FindByNameAsync(Customer.LoginName);
                    if (myuserName != null && myuserEm.Id != myuser.Id)
                    {
                        return await Result<int>.FailAsync(_localizer["رقم الهاتف مسجل مع حساب اخر#" + Customer.Phone]);

                    }
                    Customer.Userid = myuser.Id;
                    Customer.statue = "saved";


                    var myPhones = await _unitOfWork.Repository<CustomerPhone>().Entities.Where(x=>x.PhoneNumber == prevPhone && x.CustomerId== Customer.Id).FirstOrDefaultAsync();
                    if (myPhones!=null)
                    {
                        myPhones.PhoneNumber = command.Phone;
                        await _unitOfWork.Repository<CustomerPhone>().UpdateAsync(myPhones);

                    }
                    else
                    {
                        //return await Result<int>.FailAsync(_localizer["لايوجد هاتف"]);

                        var newPhone = new CustomerPhone()
                        {
                            CustomerId = Customer.Id,
                            PhoneNumber = Customer.Phone,
                            CompanyId = Customer.CompanyId,
                            StationId = Customer.StationId,
                            DriverId = Customer.DriverId,
                            Description = "الجوال الاساسي",




                        };

                        if (newPhone != null)
                        {
                            var resultPhone = await _unitOfWork.Repository<Domain.Entities.Catalog.CustomerPhone>().AddAsync(newPhone);

                        }
                    }
                    var CustomerAcc = await _unitOfWork.Repository<Accounts>().GetByIdAsync(Customer.AccountId);

                    if (CustomerAcc==null)
                    {
                        return await Result<int>.FailAsync(_localizer["لايوجد حساب مرتبط ! لم يتم التعديل"]);

                    }
                    CustomerAcc.Name = command.Name;
                    CustomerAcc.UserId = myuser.Id;

                    //return await Result<int>.FailAsync(_localizer["asdasdasdasdasasd"]);
                    await _unitOfWork.Repository<Accounts>().UpdateAsync(CustomerAcc);

                    await _unitOfWork.Repository<Customer>().UpdateAsync(Customer);


                    try
                    {
                        await userManager.UpdateAsync(myuser);
                        await userManager.ChangePasswordAsync(myuser, prevPassword, Customer.LoginPassword);



                    }
                    catch
                    {

                        return await Result<int>.FailAsync(_localizer["لم يتم تعديل بيانات اليوزر"]);
                    }



                    await _unitOfWork.Commit(cancellationToken);
                    return await Result<int>.SuccessAsync(Customer.Id, _localizer["تم تعديل بيانات الزبون"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Customer Not Found!"]);
                }
            }
        }
    }
}