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
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;
using WaterS.Application.Features.BottleTypes.Queries.GetAll;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Queries.GetCustomerById;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.BottleType;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Client.Pages.Catalog
{
    public partial class BottleTypes
    {
        [Inject] private IBottleTypeManager BottleTypeManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllBottleTypesResponse> _brandList = new();
        private GetAllBottleTypesResponse _brand = new();
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
        private int mystion { get; set; } = 0;
        private int mydriver { get; set; } = 0;

        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private int mycomp { get; set; } = 0;
        protected override async Task OnInitializedAsync()
        {
            _loaded = false;

            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.BottleTypes.Create)).Succeeded;
            _canEditBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.BottleTypes.Edit)).Succeeded;
            _canDeleteBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.BottleTypes.Delete)).Succeeded;
            _canExportBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.BottleTypes.Export)).Succeeded;
            _canSearchBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.BottleTypes.Search)).Succeeded;
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                CurrentUserRool = myUser.Data.KindType;
                mystion = myUser.Data.StationId;
                mycomp = myUser.Data.KindId;
                mydriver = myUser.Data.DriverId;
            }




            await GetBottleTypessAsync();
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
        [Inject] private ICustomerManager CustomerManager { get; set; }

        private async Task GetBottleTypessAsync()
        {
            var response = await BottleTypeManager.GetAllAsync();
            if (response.Succeeded)
            {

                var requestCustomer = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion,DriverId=mydriver,withOutInclud=1, SearchString = "", Orderby = null };


                var myCustomrts = await CustomerManager.GetCustomersAsync(requestCustomer);

                if (myCustomrts.Succeeded)
                {
                    //_snackBar.Add(" تم جلب العملاء  " + myCustomrts.Data.Count, Severity.Warning);

                    var mycuslist = myCustomrts.Data;
                    int serial = 0;
                    foreach (var item in mycuslist)
                    {
                        //mycuslist.Clear();
                        serial++;
                        item.serial = serial;

                    }



                        _brandList = response.Data.ToList();

                    //var xxList = response.Data.ToList();


                    foreach (var item in _brandList)
                    {
                        item.BottlCount = mycuslist.Where(x => x.BottleTypeId == item.Id).Count();
                        item.CustomerList = mycuslist.Where(x => x.BottleTypeId == item.Id).ToList();

                }
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
        private void ShowBtnPress(int nr)
        {
            var tmpPerson = _brandList.First(f => f.Id == nr);
            tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
        }
        private async Task InvokeModalSowCustomer(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                GetCustomerByIdResponse customer;
                var Customerresult = await CustomerManager.GetCustomerAsync(id);
                if (Customerresult.Succeeded)
                {
                    customer = Customerresult.Data;
                }
                else
                {
                    customer = null;

                }

                if (customer != null)
                {
                    parameters.Add(nameof(ViewCustomerModal.AddEditCustomerModel), new AddEditCustomerCommand
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Adress = customer.Adress,
                        Email = customer.Email,
                        Phone = customer.Phone,

                        LoginName = customer.LoginName,
                        LoginPassword = customer.LoginPassword,

                        No = customer.No,
                        Userid = customer.Userid,
                        CompanyId = customer.CompanyId,
                        StationId = customer.StationId,
                        DriverId = customer.DriverId,
                        RegionId = customer.RegionId,
                        BottleNo = customer.BottleNo,
                        BottleTypeId = customer.BottleTypeId,
                        BottleType = customer.BottleType,
                        AccountId = customer.AccountId
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<ViewCustomerModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            //if (!result.Cancelled)
            //{
            //    OnSearch("");
            //}
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
                var response = await BottleTypeManager.DeleteAsync(id);
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
            var response = await BottleTypeManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(BottleTypes).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
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
                    parameters.Add(nameof(AddEditBottleTypeModal.AddEditBottleTypeModel), new AddEditBottleTypeCommand
                    {
                        Id = _brand.Id,
                        Name = _brand.Name,
                        Description = _brand.Description,
                        FillDays= _brand.FillDays,
                        Price= _brand.Price
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditBottleTypeModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _brand = new GetAllBottleTypesResponse();
            await GetBottleTypessAsync();
        }

        private bool Search(GetAllBottleTypesResponse brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
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