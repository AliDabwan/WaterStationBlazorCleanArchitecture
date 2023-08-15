using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class Companies
    {
        [Inject] private ICompanyManager CompanyManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllCompaniesResponse> _brandList = new();
        private GetAllCompaniesResponse _brand = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateBrands;
        private bool _canEditBrands;
        private bool _canDeleteBrands;
        private bool _canExportBrands;
        private bool _canSearchBrands;
        private bool _loaded;
        private string CurrentUserRool { get; set; } = "x";
        private string CurrentUserId { get; set; }
        private int mycomp { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Companies.Create)).Succeeded;
            _canEditBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Companies.Edit)).Succeeded;
            _canDeleteBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Companies.Delete)).Succeeded;
            _canExportBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Companies.Export)).Succeeded;
            _canSearchBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Companies.Search)).Succeeded;

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                //CurrentUserId = user.GetUserId();

                CurrentUserId = user.GetUserId();
                var myUser = await _userManager.GetAsync(CurrentUserId);

                CurrentUserRool = myUser.Data.KindType;
                mycomp = myUser.Data.KindId;
              

            }

            await GetCompaniessAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }


        }

        private async Task GetCompaniessAsync()
        {
            var response = await CompanyManager.GetAllAsync();
            if (response.Succeeded)
            {


                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    _brandList = response.Data.Where(x => x.KindType == RoleConstants.ManagerRole).ToList();

                }
                else if (CurrentUserRool == RoleConstants.ManagerRole )
                {
                    _brandList = response.Data.Where(x => x.KindType == RoleConstants.ManagerRole &&x.Id==mycomp).ToList();

                }



            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await CompanyManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task ExportToExcel()
        {
            var response = await CompanyManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Companies).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Brands exported"]
                    : _localizer["Filtered Brands exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _brand = _brandList.FirstOrDefault(c => c.Id == id);
                if (_brand != null)
                {
                    parameters.Add(nameof(AddEditCompanyModal.AddEditCompanyModel), new AddEditCompanyCommand
                    {
                        Id = _brand.Id,
                        Name = _brand.Name,
                        ResName = _brand.ResName,
                        Adress = _brand.Adress,
                        Email = _brand.Email,
                        Phone = _brand.Phone,
                        KindType = _brand.KindType,
                        KindTypeAr = _brand.KindTypeAr,
                        LoginName = _brand.LoginName,
                        LoginPassword = _brand.LoginPassword,
                        MyCompanyID = _brand.MyCompanyID,
                        MystationID = _brand.MystationID,
                        AccountId=_brand.AccountId,
                        No = _brand.No,
                        Userid = _brand.Userid,
                        IsActive= _brand.IsActive,
                        ActivateDate= _brand.ActivateDate,
                        EndDate= _brand.EndDate

                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditCompanyModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _brand = new GetAllCompaniesResponse();
            await GetCompaniessAsync();
        }

        private bool Search(GetAllCompaniesResponse brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
          
            return false;
        }
    }
}