using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WaterS.Application.Exceptions;
using WaterS.Application.Extensions;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Interfaces.Services.Identity;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Requests.Mail;
using WaterS.Application.Responses.Identity;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Infrastructure.Models.Identity;
using WaterS.Infrastructure.Specifications;
using WaterS.Shared.Constants.Role;
using WaterS.Shared.Wrapper;
using WaterS.Client.Infrastructure.Routes;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;

namespace WaterS.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly RoleManager<BlazorHeroRole> _roleManager;
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<UserService> _localizer;
        private readonly IExcelService _excelService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork<int> unitOfWork;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<BlazorHeroUser> userManager,
            IMapper mapper,
            RoleManager<BlazorHeroRole> roleManager,
            IMailService mailService,
            IStringLocalizer<UserService> localizer,
            IExcelService excelService,
            ICurrentUserService currentUserService, IUnitOfWork<int> unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _mailService = mailService;
            _localizer = localizer;
            _excelService = excelService;
            _currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<List<UserResponse>>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = _mapper.Map<List<UserResponse>>(users);
            return await Result<List<UserResponse>>.SuccessAsync(result);
        }
        //public async Task<Result<List<UserResponse>>> GetByTypeAsync(GetUsersByTypeRequest request)
        //{
        //    var users = await _userManager.Users.Where.ToListAsync();

        //    var result = _mapper.Map<List<UserResponse>>(users);
        //    return await Result<List<UserResponse>>.SuccessAsync(result);
        //}
        public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                return await Result.FailAsync(string.Format(_localizer["Username {0} is already taken."], request.UserName));
            }

            var rollar = "";
            if (request.KindType == "Administrator")
            {
                rollar = "الدعم الفني";
            }
            else if (request.KindType == "Admin")
            {
                rollar = "مدير النظام";

            }
            else if (request.KindType == "Manager")
            {
                rollar = "شركة";

            }
            else if (request.KindType == "Station")
            {
                rollar = "محطة";


            }
            else if (request.KindType == "Driver")
            {
                rollar = "سائق";


            }
            else if (request.KindType == "Customer")
            {
                rollar = "زبون";


            }
            else if (request.KindType == "Basic")
            {
                rollar = "موظف";


            }

            var user = new BlazorHeroUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.ActivateUser,
                EmailConfirmed = request.AutoConfirmEmail,
                KindType= request.KindType,
                KindTypeAr= rollar

            };

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
                }
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                 
                    var s = await _userManager.AddToRoleAsync(user, request.KindType);
                    if (!s.Succeeded)
                    {
                        return await Result.FailAsync("لم يتم إضافة الرتبة");

                    }

                    if (request.KindType == "Administrator" || request.KindType == "Admin")
                    {//add Newcompany



                    }
                    else if (request.KindType == "Manager")
                    {//add Newcompany
                        Company CompanyModel = new Company()
                        {
                            Name = request.KindName,
                            Adress = request.KindAdress,
                            Email = request.Email,
                            ResName = request.KindRes,
                            Phone = request.PhoneNumber,
                            Userid = user.Id,
                            LoginName= user.UserName,
                            LoginPassword= request.Password,
                            KindType= request.KindType,
                            KindTypeAr= request.KindTypeAr,

                        };

                        //var response = await httpClient.PostAsJsonAsync(Client.Infrastructure.Routes.CompaniesEndpoints.Save, AddEditCompanyModel);
                     var response = await unitOfWork.Repository<Company>().AddAsync(CompanyModel);


                        var response2 =   await unitOfWork.Commit();

                        user.KindId = response.Id;

                        //var response = await companyManager.SaveAsync(AddEditCompanyModel);

                        if (response.Id > 0)
                        {
                            var userUpdateResult = await _userManager.UpdateAsync(user);


                            return await Result<string>.SuccessAsync("تم إضافة اليوزر وتسجيل شركة : "+ response.Name +" ينجاح ");


                        }else
                        {
                            return await Result.FailAsync(response2+ "لم يتم إضافة اليوزر بنجاح"+ response.Id);


                        }

                    }
                    else if (request.KindType == "Station")
                    {//add Newcompany
                        Company CompanyModel = new Company()
                        {
                            Name = request.KindName,
                            Adress = request.KindAdress,
                            Email = request.Email,
                            ResName = request.KindRes,
                            Phone = request.PhoneNumber,
                            Userid = user.Id,
                            LoginName = user.UserName,
                            LoginPassword = request.Password,
                            KindType = request.KindType,
                            KindTypeAr = request.KindTypeAr,
                            MyCompanyID=request.KindId
                        };

                        //var response = await httpClient.PostAsJsonAsync(Client.Infrastructure.Routes.CompaniesEndpoints.Save, AddEditCompanyModel);
                        var response = await unitOfWork.Repository<Company>().AddAsync(CompanyModel);


                        var response2 = await unitOfWork.Commit();

                        user.KindId = response.MyCompanyID;
                        user.StationId = response.Id;

                        //var response = await companyManager.SaveAsync(AddEditCompanyModel);

                        if (response.Id > 0)
                        {
                            var userUpdateResult = await _userManager.UpdateAsync(user);


                            return await Result<string>.SuccessAsync("تم إضافة اليوزر وتسجيل محطة : " + response.Name + " ينجاح ");


                        }
                        else
                        {
                            return await Result.FailAsync(response2 + "لم يتم إضافة اليوزر بنجاح" + response.Id);


                        }


                    }







                    if (!request.AutoConfirmEmail)
                    {
                        var verificationUri = await SendVerificationEmail(user, origin);
                        var mailRequest = new MailRequest
                        {
                            From = "mail@codewithmukesh.com",
                            To = user.Email,
                            Body = string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], verificationUri),
                            Subject = _localizer["Confirm Registration"]
                        };
                        BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
                        return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"], user.UserName));
                    }
                    return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered."], user.UserName));
                }
                else
                {
                    return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
                }
            }
            else
            {
                return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
            }
        }

        private async Task<string> SendVerificationEmail(BlazorHeroUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/identity/user/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            var result = _mapper.Map<UserResponse>(user);
            return await Result<UserResponse>.SuccessAsync(result);
        }
        public async Task<IResult<UserResponse>> GetAsyncByName(string UserName)
        {
            var user = await _userManager.Users.Where(u => u.UserName == UserName).FirstOrDefaultAsync();
            var result = _mapper.Map<UserResponse>(user);
            return await Result<UserResponse>.SuccessAsync(result);
        }

        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
            var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
            if (isAdmin)
            {
                return await Result.FailAsync(_localizer["Administrators Profile's Status cannot be toggled"]);
            }
            if (user != null)
            {
                user.IsActive = request.ActivateUser;
                var identityResult = await _userManager.UpdateAsync(user);
            }
            return await Result.SuccessAsync();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
        {
            var viewModel = new List<UserRoleModel>();
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                viewModel.Add(userRolesViewModel);
            }
            var result = new UserRolesResponse { UserRoles = viewModel };
            return await Result<UserRolesResponse>.SuccessAsync(result);
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user.Email == "mukesh@blazorhero.com")
            {
                return await Result.FailAsync(_localizer["Not Allowed."]);
            }
            if (user.Email == "dev.alidabwan@gmail.com")
            {
                return await Result.FailAsync(_localizer["Not Allowed."]);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
            {
                var tryToAddAdministratorRole = selectedRoles
                    .Any(x => x.RoleName == RoleConstants.AdministratorRole);
                var userHasAdministratorRole = roles.Any(x => x == RoleConstants.AdministratorRole);
                if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
                {
                    return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
                }
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
            return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.Email));
            }
            else
            {
                throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
            }
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            var mailRequest = new MailRequest
            {
                Body = string.Format(_localizer["Please reset your password by <a href='{0}>clicking here</a>."], HtmlEncoder.Default.Encode(passwordResetURL)),
                Subject = _localizer["Reset Password"],
                To = request.Email
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
            }
            else
            {
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _userManager.Users.CountAsync();
            return count;
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            var userSpec = new UserFilterSpecification(searchString);
            var users = await _userManager.Users
                .Specify(userSpec)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
            var result = await _excelService.ExportAsync(users, sheetName: _localizer["Users"],
                mappers: new Dictionary<string, Func<BlazorHeroUser, object>>
                {
                    { _localizer["Id"], item => item.Id },
                    { _localizer["FirstName"], item => item.FirstName },
                    { _localizer["LastName"], item => item.LastName },
                    { _localizer["UserName"], item => item.UserName },
                    { _localizer["Email"], item => item.Email },
                    { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                    { _localizer["PhoneNumber"], item => item.PhoneNumber },
                    { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                    { _localizer["IsActive"], item => item.IsActive },
                    { _localizer["CreatedOn (Local)"], item => DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["CreatedOn (UTC)"], item => item.CreatedOn.ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["ProfilePictureDataUrl"], item => item.ProfilePictureDataUrl },
                });

            return result;
        }
    }
}