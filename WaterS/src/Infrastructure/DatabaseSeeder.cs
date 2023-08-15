using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Services;
using WaterS.Domain.Entities.Catalog;
using WaterS.Infrastructure.Contexts;
using WaterS.Infrastructure.Helpers;
using WaterS.Infrastructure.Models.Identity;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;
using WaterS.Shared.Constants.User;

namespace WaterS.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<DatabaseSeeder> _localizer;
        private readonly BlazorHeroContext _db;
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly RoleManager<BlazorHeroRole> _roleManager;

        public DatabaseSeeder(
            UserManager<BlazorHeroUser> userManager,
            RoleManager<BlazorHeroRole> roleManager,

            BlazorHeroContext db,
            ILogger<DatabaseSeeder> logger,
            IStringLocalizer<DatabaseSeeder> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
            _localizer = localizer;
        }

        public void Initialize()
        {
            AddAdministrator();
            AddBasicUser();
            AddAccount();

            _db.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists 
                var adminRole = new BlazorHeroRole(RoleConstants.AdministratorRole, _localizer["مسئول"]);
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                    _logger.LogInformation(_localizer["Seeded Administrator Role."]);
                }
                //Check if User Exists
                var superUser = new BlazorHeroUser
                {
                    FirstName = "AboGhaith",
                    LastName = "Ali",
                    Email = "dev.alidabwan@gmail.com",
                    UserName = "AboGhaith",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    KindType = RoleConstants.AdministratorRole
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(_localizer["Seeded Default SuperAdmin User."]);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }
                foreach (var permission in Permissions.GetRegisteredPermissions())
                {
                    await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                }
            }).GetAwaiter().GetResult();
        }


        private void AddAccount()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists 
                var TalapAcc = await _db.Accounts.Where(x=>x.Name== "الوجبات").FirstOrDefaultAsync();
                if (TalapAcc == null)
                {
                    var newAcc = new Accounts()
                    {
                        No = 3021001,
                        Name = "الوجبات",
                        AccountType = 1,
                        CategoryType = "الوجبات",
                        CreatedOn = DateTime.Now,

                    };
                    var result = await _db.Accounts.AddAsync(newAcc);
                    
                }
             
            }).GetAwaiter().GetResult();
        }

        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var newRole = new BlazorHeroRole(RoleConstants.AdminRole, RoleConstants.AdminRoleAr);
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdminRole);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(newRole);
                    _logger.LogInformation(_localizer["Seeded "+ newRole.Name+" Role."]);
                }

                var newRoleManagerRole = new BlazorHeroRole(RoleConstants.ManagerRole, RoleConstants.ManagerRoleAr);
                var basicRoleInDbManagerRole = await _roleManager.FindByNameAsync(RoleConstants.ManagerRole);
                if (basicRoleInDbManagerRole == null)
                {
                    await _roleManager.CreateAsync(newRoleManagerRole);
                    _logger.LogInformation(_localizer["Seeded " + newRoleManagerRole.Name + " Role."]);
                }


                var newRoleStationRole = new BlazorHeroRole(RoleConstants.StationRole, RoleConstants.StationRoleAr);
                var basicRoleInDbStationRole = await _roleManager.FindByNameAsync(RoleConstants.StationRole);
                if (basicRoleInDbStationRole == null)
                {
                    await _roleManager.CreateAsync(newRoleStationRole);
                    _logger.LogInformation(_localizer["Seeded " + newRoleStationRole.Name + " Role."]);
                }
                var newRoleDriverRole = new BlazorHeroRole(RoleConstants.DriverRole, RoleConstants.DriverRoleAr);
                var basicRoleInDbDriverRole = await _roleManager.FindByNameAsync(RoleConstants.DriverRole);
                if (basicRoleInDbDriverRole == null)
                {
                    await _roleManager.CreateAsync(newRoleDriverRole);
                    _logger.LogInformation(_localizer["Seeded " + newRoleDriverRole.Name + " Role."]);
                }
                var newRoleCustomerRole = new BlazorHeroRole(RoleConstants.CustomerRole, RoleConstants.CustomerRoleAr);
                var basicRoleInDbCustomerRole = await _roleManager.FindByNameAsync(RoleConstants.CustomerRole);
                if (basicRoleInDbCustomerRole == null)
                {
                    await _roleManager.CreateAsync(newRoleCustomerRole);
                    _logger.LogInformation(_localizer["Seeded " + newRoleCustomerRole.Name + " Role."]);
                }
                //Check if User Exists
                var basicUser = new BlazorHeroUser
                {
                    FirstName = "Nine",
                    LastName = "soft",
                    Email = "ninesoft@roiraq.com",
                    UserName = "ninesoft",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    KindType = RoleConstants.AdminRole

                };
                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword2);
                    await _userManager.AddToRoleAsync(basicUser, RoleConstants.AdminRole);
                    _logger.LogInformation(_localizer["Seeded User with Admin Role."]);
                }
            }).GetAwaiter().GetResult();
        }
    }
}