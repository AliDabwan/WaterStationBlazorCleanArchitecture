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
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Application.Features.Customers.Queries.GetCustomerById;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Client.Pages.Catalog
{
    public partial class RegionsDetails
    {
        [Inject] private IDriverRegionManager DriverRegionManager { get; set; }
        [Inject] private ICustomerManager CustomerManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedDriverRegionsResponse> _pagedData;
        private MudTable<GetAllPagedDriverRegionsResponse> _table;
        private int _totalItems;
        private int _totalCustomersItems;

        private int _currentPage;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDriverRegions;
        private bool _canEditDriverRegions;
        private bool _canDeleteDriverRegions;
        private bool _canExportDriverRegions;
        private bool _canSearchDriverRegions;
        private bool _loaded;

        private int mystion { get; set; } = 0;
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private int mycomp { get; set; } = 0;
        private int mydriver { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();

            _canCreateDriverRegions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DriverRegions.Create)).Succeeded;
            _canEditDriverRegions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DriverRegions.Edit)).Succeeded;
            _canDeleteDriverRegions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DriverRegions.Delete)).Succeeded;
            _canExportDriverRegions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DriverRegions.Export)).Succeeded;
            _canSearchDriverRegions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DriverRegions.Search)).Succeeded;
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

        private async Task<TableData<GetAllPagedDriverRegionsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            StateHasChanged();
            return new TableData<GetAllPagedDriverRegionsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }
        public int serial { get; set; } = 0;
        List<GetAllPagedCustomersResponse> mycuslist = new List<GetAllPagedCustomersResponse>();

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            //_loaded = false;

            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedDriverRegionsRequest { PageSize = pageSize, PageNumber = pageNumber + 1,CompanyId=mycomp,StationId=mystion,DriverId=mydriver, SearchString = _searchString, Orderby = orderings };
            var response = await DriverRegionManager.GetDriverRegionsAsync(request);
            if (response.Succeeded)
            {
                _currentPage = response.CurrentPage;

       
                    _pagedData = response.Data;
                int iserial = 1;
                foreach (var item in _pagedData)
                {
                    item.serial = iserial;
                    iserial++;
                }

                _totalItems = response.TotalCount;

                


                //List<Customer> ll = new List<Customer>();
                //List<Customer> llList = new List<Customer>();

                var requestCustomer = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber = 0 + 1,CompanyId = mycomp, StationId = mystion,DriverId=mydriver, withOutInclud = 1, SearchString ="", Orderby = orderings };


                var myCustomrts = await CustomerManager.GetCustomersAsync(requestCustomer);

                if (myCustomrts.Succeeded)
                {
                    _totalCustomersItems = myCustomrts.TotalCount;
                    //_snackBar.Add(" العملاء  " + _totalCustomersItems, Severity.Info);
                    //_snackBar.Add(" تم جلب العملاء  " + myCustomrts.Data.Count, Severity.Warning);



                    mycuslist = myCustomrts.Data;


                }
                else
                {
                    _snackBar.Add("لايوجد", Severity.Error);

                }
                //IList<GetAllPagedDriverRegionsResponse> people = new List<GetAllPagedDriverRegionsResponse>();






                foreach (var subitem in _pagedData)
                {
                    //    serial = 0;

                    //    //ll.Clear();
                    var myCustomrtsList = mycuslist.Where(x => x.DriverId == subitem.DriverId && x.RegionId == subitem.RegionId);
                    subitem.CustomerCounts = myCustomrtsList.Count();
                    //    //_snackBar.Add("  DriverId " + subitem.DriverId + "  RegionId " + subitem.RegionId + "  myCustomrtsList " + myCustomrtsList.Count(), Severity.Success);
                    //    //_snackBar.Add("x.DriverId "+ subitem.DriverId + " RegionId " + subitem.RegionId + "  count:  "+ myCustomrtsList.Count().ToString(), Severity.Success);

                    //    subitem.CustomerList = myCustomrtsList.ToList();
                    //    foreach (var item in subitem.CustomerList)
                    //    {
                    //        serial++;

                    //        item.serial = serial;

                    //    }



                }




            }
            else
            {
                //_loaded = true;

                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }


        private async Task InvokeModalSowCustomer(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                GetCustomerByIdResponse customer;
                var Customerresult=await CustomerManager.GetCustomerAsync(id);
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



        private bool isVisible;
        private bool dataLoaded;

        public async void OpenOverlay()
        {
            isVisible = true;
            await Task.Delay(2000);
            isVisible = false;
            dataLoaded = true;
            StateHasChanged();
        }

        public void ResetExample()
        {
            dataLoaded = false;
        }
        private async Task ShowBtnPress(int nr,int drivId,int regId)
        {
            var tmpPerson = _pagedData.First(f => f.Id == nr);
            tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
            if (tmpPerson.ShowDetails)
            {
                var tmpPersonOthers = _pagedData.Where(f => f.Id != nr);

                foreach (var item in tmpPersonOthers)
                {
                    item.ShowDetails = false;
                }
                mycuslist.Clear();
              

                ResetExample();

            }

            var requestCustomer = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion, DriverId = drivId, withOutInclud = 1, SearchString = "", Orderby = null,RegionId= regId };


            var myCustomrts = await CustomerManager.GetCustomersAsync(requestCustomer);

            if (myCustomrts.Succeeded)
            {

                mycuslist = myCustomrts.Data;
                //var myCustomrtsList = mycuslist.Where(x => x.DriverId == drivId && x.RegionId == regId);

                int serialCus = 0;
                foreach (var item in mycuslist)
                {
                    //mycuslist.Clear();
                    serialCus++;
                    item.serial = serialCus;

                }
                OpenOverlay();
            }
            else
            {
                tmpPerson.ShowDetails = false;
                //_snackBar.Add("else", Severity.Success);

            }

            //serial = 0;

            //ll.Clear();


            //_snackBar.Add("  DriverId " + subitem.DriverId + "  RegionId " + subitem.RegionId + "  myCustomrtsList " + myCustomrtsList.Count(), Severity.Success);
            //_snackBar.Add("x.DriverId "+ subitem.DriverId + " RegionId " + subitem.RegionId + "  count:  "+ myCustomrtsList.Count().ToString(), Severity.Success);



            //var myrow = _pagedData.Where(x=>x.Id== nr).FirstOrDefault();

            //myrow.CustomerList = myCustomrtsList.ToList();



            //foreach (var item in mycuslist)
            //{
            //    serial++;

            //    item.serial = serial;

            //}









        }
        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            var response = await DriverRegionManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(DriverRegions).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["DriverRegions exported"]
                    : _localizer["Filtered DriverRegions exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private class Address
        {
            public int regionId { get; set; }
            public int serial { get; set; }

            public string Name { get; set; }
            public string Phone { get; set; }
            public int BottleNo { get; set; }
            public string BottleType { get; set; }
        }
        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                //_snackBar.Add("1" + 1, Severity.Normal);

                var DriverRegion = _pagedData.FirstOrDefault(c => c.Id == id);
                //_snackBar.Add("2" + 1, Severity.Normal);

                if (DriverRegion != null)
                {
                    parameters.Add(nameof(AddEditDriverRegionModal.AddEditDriverRegionModel), new AddEditDriverRegionCommand
                    {
                        Id = id,
                        DriverId = DriverRegion.DriverId,
                        RegionId = DriverRegion.RegionId,
                        StationId = DriverRegion.StationId,
                        CompanyId = DriverRegion.CompanyId
                        
                    });
                }
            }
            //_snackBar.Add("3" + 1, Severity.Normal);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditDriverRegionModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
            //var result = await dialog.Result;
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
                var response = await DriverRegionManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
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