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
using WaterS.Application.Features.Drivers.Commands.AddEdit;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Driver;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class Drivers
    {
        [Inject] private IDriverManager DriverManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedDriversResponse> _pagedData;
        private MudTable<GetAllPagedDriversResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDrivers;
        private bool _canEditDrivers;
        private bool _canDeleteDrivers;
        private bool _canExportDrivers;
        private bool _canSearchDrivers;
        private bool _loaded;
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "x";
        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;
        private int mydriver{ get; set; } = 0;

       
        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateDrivers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Drivers.Create)).Succeeded;
            _canEditDrivers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Drivers.Edit)).Succeeded;
            _canDeleteDrivers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Drivers.Delete)).Succeeded;
            _canExportDrivers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Drivers.Export)).Succeeded;
            _canSearchDrivers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Drivers.Search)).Succeeded;

         



            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;
                mycomp = myUser.Data.KindId;
                mystion = myUser.Data.StationId;
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




        private static IEnumerable<GetAllPagedDriversResponse> People { get; set; }
        private class myCustomer
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public int BottleNo { get; set; }
            public string BottleType { get; set; }
        }
      
      

        private void FillPeople()
        {
            IList<GetAllPagedDriversResponse> people = new List<GetAllPagedDriversResponse>();
            //IList<Address> addresses = new List<Address>();


            if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
            {


                foreach (var subitem in _pagedData)
                {
                    people.Add(new GetAllPagedDriversResponse
                    {
                        Id = subitem.Id,
                        Name = subitem.Name,

                        CustomerList = subitem.CustomerList,
                        Phone = subitem.Name,
                        Adress = subitem.Adress,
                        myStation = subitem.myStation
                    });

                }

            }
            else if (CurrentUserRool == RoleConstants.ManagerRole)
            {


                foreach (var subitem in _pagedData)
                {
                    people.Add(new GetAllPagedDriversResponse
                    {
                        Id = subitem.Id,
                        Name = subitem.Name,

                        CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp).ToList(),
                        Phone = subitem.Name,
                        Adress = subitem.Adress,
                        myStation = subitem.myStation

                       
                    });

                }


            }
            else if (CurrentUserRool == RoleConstants.StationRole)
            {
                foreach (var subitem in _pagedData)
                {
                    people.Add(new GetAllPagedDriversResponse
                    {
                        Id = subitem.Id,
                        Name = subitem.Name,

                        CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp && x.StationId == mystion).ToList(),
                        Phone = subitem.Name,
                        Adress = subitem.Adress,
                        myStation = subitem.myStation
                     
                    });

                }
            }
            else
            {
                foreach (var subitem in _pagedData)
                {
                    people.Add(new GetAllPagedDriversResponse
                    {
                        Id = subitem.Id,
                        Name = subitem.Name,

                        CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp && x.StationId == mystion && x.DriverId == mydriver).ToList(),
                        Phone = subitem.Name,
                        Adress = subitem.Adress,
                        myStation = subitem.myStation
                      
                    });

                }

            }




            People = people;
        }

        public bool showSub { get; set; } = false;

        private void ShowBtnPress(int nr)
        {
            var tmpPerson = People.First(f => f.Id == nr);
            tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
        }



        private async Task<TableData<GetAllPagedDriversResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            StateHasChanged();
            return new TableData<GetAllPagedDriversResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            //_snackBar.Add("CurrentUserRool= :" + CurrentUserRool);

            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedDriversRequest { PageSize = pageSize, PageNumber = pageNumber + 1, CompanyId = mycomp, StationId = mystion, DriverId = mydriver, SearchString = _searchString, Orderby = orderings };
            var response = await DriverManager.GetDriversAsync(request);
        
            //_snackBar.Add("TotalCount= :" + response.TotalCount);
             
            if (response.Succeeded)
            {
                _currentPage = response.CurrentPage;
                _totalItems = response.TotalCount;
                //if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                //{
                    _pagedData = response.Data;

                //}
                //else if(CurrentUserRool == RoleConstants.ManagerRole)
                //{

                //    _pagedData = response.Data.Where(x => x.CompanyId == mycomp).ToList();

                //}else
                //{
                //    _pagedData = response.Data.Where(x => x.CompanyId == mycomp&&x.StationId==mystion).ToList();


                //}
                //int serial = 0;

                //foreach (var item in _pagedData)
                //{
                //    serial++;
                //    item.serial = serial;
                //}


                //_totalItems = _pagedData.Count();


                if (_pagedData.Any())
                {
                    FillPeople();

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

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            var response = await DriverManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Drivers).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Drivers exported"]
                    : _localizer["Filtered Drivers exported"], Severity.Success);
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
                var Driver = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Driver != null)
                {
                    parameters.Add(nameof(AddEditDriverModal.AddEditDriverModel), new AddEditDriverCommand
                    {
                        Id = Driver.Id,
                        Name = Driver.Name,
                        ResName = Driver.ResName,
                        Adress = Driver.Adress,
                        Email = Driver.Email,
                        Phone = Driver.Phone,
                        KindType = Driver.KindType,
                        KindTypeAr = Driver.KindTypeAr,
                        LoginName = Driver.LoginName,
                        LoginPassword = Driver.LoginPassword,
                        No = Driver.No,
                        Userid = Driver.Userid,
                        CompanyId = Driver.CompanyId,
                        StationId = Driver.StationId,
                        AccountId=Driver.AccountId
                       
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditDriverModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
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
                var response = await DriverManager.DeleteAsync(id);
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