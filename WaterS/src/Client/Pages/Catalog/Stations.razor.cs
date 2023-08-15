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
using WaterS.Application.Features.Stations.Commands.AddEdit;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class Stations
    {
        [Inject] private IStationManager StationManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedStationsResponse> _pagedData;
        private MudTable<GetAllPagedStationsResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateStations;
        private bool _canEditStations;
        private bool _canDeleteStations;
        private bool _canExportStations;
        private bool _canSearchStations;
        private bool _loaded;
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "x";
        private int mycomp { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();

            _canCreateStations = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Create)).Succeeded;
            _canEditStations = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Edit)).Succeeded;
            _canDeleteStations = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Delete)).Succeeded;
            _canExportStations = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Export)).Succeeded;
            _canSearchStations = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Search)).Succeeded;

           
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

            }
            _loaded = true;

            try
            {
                //await HubConnection.StopAsync();

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

        private async Task<TableData<GetAllPagedStationsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllPagedStationsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }
            
        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new GetAllPagedStationsRequest { PageSize = pageSize, PageNumber = pageNumber + 1, SearchString = _searchString, Orderby = orderings };
          
            
            var response = await StationManager.GetStationsAsync(request);



            if (response.Succeeded)
            {

               
                //_totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    _pagedData = response.Data;

                }
                else
                {

                    _pagedData = response.Data.Where(x => x.CompanyId == mycomp).ToList();

                }
                _totalItems = _pagedData.Count();
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
            var response = await StationManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Stations).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Stations exported"]
                    : _localizer["Filtered Stations exported"], Severity.Success);
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
                var Station = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Station != null)
                {
                    parameters.Add(nameof(AddEditStationModal.AddEditStationModel), new AddEditStationCommand
                    {
                        Id = Station.Id,
                        Name = Station.Name,
                        ResName = Station.ResName,
                        Adress = Station.Adress,
                        Email = Station.Email,
                        Phone = Station.Phone,
                        KindType = Station.KindType,
                        KindTypeAr = Station.KindTypeAr,
                        LoginName = Station.LoginName,
                        LoginPassword = Station.LoginPassword,
                        MyCompanyId = Station.MyCompanyId,
                        MyStationId = Station.MyStationId,
                        No = Station.No,
                        Userid = Station.Userid,
                        CompanyId = Station.CompanyId,
                        AccountId= Station.AccountId,
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditStationModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
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
                var response = await StationManager.DeleteAsync(id);
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