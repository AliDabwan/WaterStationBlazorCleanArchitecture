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
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;
using WaterS.Application.Features.CustomerPhones.Queries.GetAll;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Client.Pages.Catalog
{
    public partial class CustomerPhones
    {
        [Inject] private ICustomerPhoneManager CustomerPhoneManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllCustomerPhonesResponse> _brandList = new();
        private GetAllCustomerPhonesResponse _brand = new();
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
        private string CurrentUserId { get; set; }

        private string CurrentUserRool { get; set; } = "x";
        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;
        private int mydriver { get; set; } = 0;
        private int mycustomer { get; set; } = 0;
        private int myAccountId { get; set; } = 0;
        protected override async Task OnInitializedAsync()
        {
            _loaded = false;

            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.CustomerPhones.Create)).Succeeded;
            _canEditBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.CustomerPhones.Edit)).Succeeded;
            _canDeleteBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.CustomerPhones.Delete)).Succeeded;
            _canExportBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.CustomerPhones.Export)).Succeeded;
            _canSearchBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.CustomerPhones.Search)).Succeeded;



            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                //CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;
                mycomp = myUser.Data.KindId;
                mystion = myUser.Data.StationId;
                mydriver = myUser.Data.DriverId;
                mycustomer = myUser.Data.CustomerId;
            }


            await GetCustomerPhonessAsync();
            _loaded = true;
            try
            {
                HubConnection = HubConnection.TryInitialize(_navigationManager);
                if (HubConnection.State == HubConnectionState.Disconnected)
                {
                    await HubConnection.StartAsync();
                }
            }
            catch
            {

                return;
            }
           
        }

        private async Task GetCustomerPhonessAsync()
        {
            var response = await CustomerPhoneManager.GetAllAsync();
            if (response.Succeeded)
            {
                _brandList = response.Data.Where(x=>x.CustomerId== mycustomer).ToList();
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
            string deleteContent = _localizer["حذف السجل"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["حذف"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await CustomerPhoneManager.DeleteAsync(id);
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
            var response = await CustomerPhoneManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Brands).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
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
                    parameters.Add(nameof(AddEditCustomerPhoneModal.AddEditCustomerPhoneModel), new AddEditCustomerPhoneCommand
                    {
                        Id = _brand.Id,
                        PhoneNumber = _brand.PhoneNumber,
                        Description = _brand.Description,
                        CustomerId= _brand.CustomerId,
                        CompanyId = _brand.CompanyId,
                        StationId = _brand.StationId,
                        DriverId = _brand.DriverId,
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditCustomerPhoneModal>(id == 0 ? _localizer["حفظ"] : _localizer["تعديل"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _brand = new GetAllCustomerPhonesResponse();
            await GetCustomerPhonessAsync();
        }

        private bool Search(GetAllCustomerPhonesResponse brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }
    }
}