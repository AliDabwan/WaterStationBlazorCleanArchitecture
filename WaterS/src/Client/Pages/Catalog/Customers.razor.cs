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
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;
using WaterS.Client.Shared;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class Customers
    {
        [Inject] private ICustomerManager CustomerManager { get; set; }
        [Inject] private IDriverRegionManager DriverRegionManager { get; set; }

        [Inject] private ICustomerPhoneManager CustomerPhoneManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        //[CascadingParameter] private Error error { get; set; }
        private IEnumerable<GetAllPagedDriverRegionsResponse> _driversRegion ;
        private IEnumerable<GetAllPagedCustomersResponse> _pagedDataJoined;

        private IEnumerable<GetAllPagedCustomersResponse> _pagedData;
        private MudTable<GetAllPagedCustomersResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = ""; 
                    private string _searchBy = ""; 

        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateCustomers;
        private bool _canEditCustomers;
        private bool _canDeleteCustomers;
        private bool _canExportCustomers;
        private bool _canSearchCustomers;
        private bool _loaded;
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "x";

        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;
        private int mydriver { get; set; } = 0;



        protected override async Task OnInitializedAsync()
        {


            try
            {


                _currentUser = await _authenticationManager.CurrentUser();

                _canCreateCustomers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Customers.Create)).Succeeded;
                _canEditCustomers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Customers.Edit)).Succeeded;
                _canDeleteCustomers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Customers.Delete)).Succeeded;
                _canExportCustomers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Customers.Export)).Succeeded;
                _canSearchCustomers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Customers.Search)).Succeeded;




                var state = await _stateProvider.GetAuthenticationStateAsync();
                var user = state.User;
                if (user == null) return;




                if (user.Identity?.IsAuthenticated == true)
                {
                    CurrentUserId = user.GetUserId();
                    //_snackBar.Add("CurrentUserId= :" + CurrentUserId);

                    var myUser = await _userManager.GetAsync(CurrentUserId);
                    CurrentUserId = user.GetUserId();
                    CurrentUserRool = myUser.Data.KindType;
                    mycomp = myUser.Data.KindId;
                    mystion = myUser.Data.StationId;
                    mydriver = myUser.Data.DriverId;


                    //_snackBar.Add("mydriver= :" + mydriver+"  "+ "mystion= :" + mystion + "  " + "mycomp= :" + mycomp);
                    _searchBy = "all";
                }
                _loaded = true;

            }
            catch
            {

                //error.ProcessError(ex);
            }
            try
            {

                HubConnection = HubConnection.TryInitialize(_navigationManager);
                if (HubConnection.State == HubConnectionState.Disconnected)
                {

                    await HubConnection.StartAsync();
                }
                //_navigationManager.NavigateTo("", true) ;
            }
          catch 
            {

                //error.ProcessError(ex);
                return;
            }



        }
        private async Task InvokePhoneModal(int id )
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var custom = await CustomerManager.GetCustomerAsync(id);
                if (!custom.Succeeded)
                {
                    _snackBar.Add("لم يتم جلب تفاصيل الزبون بنجاح", Severity.Error);
                    return;
                }

                parameters.Add(nameof(AddPhoneModal.AddEditCustomerPhoneModel), new AddEditCustomerPhoneCommand
                    {Id=0,
                        CustomerId = custom.Data.Id,
                        PhoneNumber="0",
                        CompanyId = custom.Data.CompanyId,
                        StationId = custom.Data.StationId,
                    DriverId = custom.Data.DriverId,
                   
                });
                
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddPhoneModal>(_localizer["حفظ"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }
        private async Task<TableData<GetAllPagedCustomersResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
          


            await LoadData(state.Page, state.PageSize, state);
            StateHasChanged();
            return new TableData<GetAllPagedCustomersResponse> { TotalItems = _totalItems, Items = _pagedDataJoined };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            //_snackBar.Add("CurrentUserRool= :" + CurrentUserRool);
            if (string.IsNullOrEmpty(state.SortLabel))
            {

                if (_searchBy == "LastFillDays")
                {
                    state.SortLabel = "LastFillDateDays";
                    state.SortDirection = SortDirection.Descending;
                    pageSize = 10000;

                }
                else
                {
                    state.SortLabel = "BottleNo";
                    state.SortDirection = SortDirection.Ascending;

                }
            }
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }
            if (showAll)
            {

                pageSize = 10000;
            }

            var request = new GetAllPagedCustomersRequest { PageSize = pageSize, PageNumber = pageNumber + 1, CompanyId = mycomp, StationId = mystion, withOutInclud=0, DriverId = mydriver, SearchString = _searchString, SearchBy = _searchBy, Orderby = orderings };
          
            var response = await CustomerManager.GetCustomersAsync(request);

            //_snackBar.Add("TotalCount= :" + response.TotalCount);

            if (response.Succeeded)
            {
                _currentPage = response.CurrentPage;
                _totalItems = response.TotalCount;
                _pagedData = response.Data;

                //_snackBar.Add("try region deriver");

                var requestR = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion, DriverId = mydriver};
                var responseRegion = await DriverRegionManager.GetDriverRegionsAsync(requestR);

                if (responseRegion.Succeeded)
                {

                    _driversRegion = responseRegion.Data;

                    

                        _pagedDataJoined = _pagedData.Join(_driversRegion, c => c.RegionId, dr => dr.RegionId, (c, dr) => new { c, dr }).Where(z => z.dr.StationId == z.c.StationId)
                          .Select(cdr => new GetAllPagedCustomersResponse
                          {
                              Id = cdr.c.Id,
                              Name = cdr.c.Name,
                              No = cdr.c.No,
                              AccountId = cdr.c.AccountId,
                              Adress = cdr.c.Adress,
                              Email = cdr.c.Email,
                              Phone = cdr.c.Phone,
                              LoginName = cdr.c.LoginName,
                              LoginPassword = cdr.c.LoginPassword,
                              Userid = cdr.c.Userid,
                              BottleNo = cdr.c.BottleNo,
                              BottleNoStatue = cdr.c.BottleNoStatue,

                              BottleTypeId = cdr.c.BottleTypeId,
                              CompanyId = cdr.c.CompanyId,
                              StationId = cdr.c.StationId,
                              DriverId = cdr.dr.DriverId,
                              RegionId = cdr.c.RegionId,
                              Company = cdr.c.Company,
                              Station = cdr.c.Station,
                              Driver = cdr.dr.Driver,
                              Region = cdr.c.Region,
                              BottleType = cdr.c.BottleType,
                              BottleTypeName = cdr.c.BottleType.Name,
                              updatedOn = cdr.c.updatedOn,
                              CustomerPhones = cdr.c.CustomerPhones,
                              ShowDetails = false,
                              LastFillDate = cdr.c.LastFillDate,
                              LastFillDateDays = cdr.c.LastFillDateDays,
                              statue = cdr.c.statue,
                              serial = cdr.c.serial


                          }

                          ).ToList();


                    if (_searchBy == "LastFillDays")
                    {
                        //_snackBar.Add("LastFillDays");
                        _pagedDataJoined = _pagedDataJoined.Where(x => x.LastFillDateDays > 0 && x.LastFillDateDays <=360);
                        _totalItems = _pagedDataJoined.Count();
                    }


                }
                OpenOverlay();





            }
            else
            {
                OpenOverlay();

                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private bool isVisible;
        private bool dataLoaded;


        public async void OpenOverlay()
        {
            isVisible = true;
            await Task.Delay(1000);
            isVisible = false;
            dataLoaded = true;
            StateHasChanged();
        }
        public void ResetExample()
        {
            dataLoaded = false;
        }

        bool showAll = false;
        private void OnSearch(string text)
        {
            ResetExample();

            if (_searchBy == "ShowAll")
            {
                showAll = true;
                _searchString = "";

            }
            else
            {
                showAll = false;
                _searchString = text;

            }
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            var response = await CustomerManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Customers).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Customers exported"]
                    : _localizer["Filtered Customers exported"], Severity.Success);
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
            var tmpPerson = _pagedDataJoined.First(f => f.Id == nr);
            tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
        }
        private async Task InvokPhoneeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var customerPhones = await CustomerPhoneManager.GetAllAsync();
                var _brand = customerPhones.Data.Where(x => x.Id == id).FirstOrDefault();
                if (_brand != null)
                {
                    parameters.Add(nameof(AddEditCustomerPhoneModal.AddEditCustomerPhoneModel), new AddEditCustomerPhoneCommand
                    {
                        Id = _brand.Id,
                        PhoneNumber = _brand.PhoneNumber,
                        Description = _brand.Description,
                        CustomerId = _brand.CustomerId,
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
                OnSearch("");

            }
        }
        private async Task InvokPhoneeModalNew(int id ,int compId,int station,int drivid)
        {
            var parameters = new DialogParameters();
           
                    parameters.Add(nameof(AddEditCustomerPhoneGModal.AddEditCustomerPhoneModel), new AddEditCustomerPhoneCommand
                    {
                        Id=0,
                        CustomerId = id,
                        CompanyId = compId,
                        StationId = station,
                        DriverId = drivid,
                    });
              
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditCustomerPhoneGModal>(id == 0 ? _localizer["حفظ"] : _localizer["تعديل"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");

            }
        }


        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var Customer = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Customer != null)
                {
                    parameters.Add(nameof(AddEditCustomerModal.AddEditCustomerModel), new AddEditCustomerCommand
                    {
                        Id = Customer.Id,
                        Name = Customer.Name,
                        Adress = Customer.Adress,
                        Email = Customer.Email,
                        Phone = Customer.Phone,

                        LoginName = Customer.LoginName,
                        LoginPassword = Customer.LoginPassword,

                        No = Customer.No,
                        Userid = Customer.Userid,
                        CompanyId = Customer.CompanyId,
                        StationId = Customer.StationId,
                        DriverId = Customer.DriverId,
                        RegionId = Customer.RegionId,
                        BottleNo = Customer.BottleNo,
                        BottleTypeId = Customer.BottleTypeId,
                        BottleType = Customer.BottleType,
                        AccountId = Customer.AccountId
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditCustomerModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {

                _snackBar.Add("تم التعديل بنجاح ! يرجى النقر على زر تحديث لتطبيق التغيرات");
                //OnSearch("");
            }
        }


        private async Task DeleteCustomerPhone(int id)
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
                var response = await CustomerPhoneManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
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
                var response = await CustomerManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}