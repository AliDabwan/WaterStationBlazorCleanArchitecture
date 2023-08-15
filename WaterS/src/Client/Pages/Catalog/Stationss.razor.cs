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
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class Stationss
    {
        [Inject] private IStationManager StationManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllStationsResponse> _brandList = new();
        private GetAllStationsResponse _brand = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;
        private List<GetAllStationsResponse> mycompanies = new();

        private ClaimsPrincipal _currentUser;
        private bool _canCreateBrands;
        private bool _canEditBrands;
        private bool _canDeleteBrands;
        private bool _canExportBrands;
        private bool _canSearchBrands;
        private bool _loaded;
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "x";
        private string OwnerRoll { get; set; }="Administrator";
        private string AdminRoll { get; set; }="Admin";

        private int mycomp { get; set; }
        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Create)).Succeeded;
            _canEditBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Edit)).Succeeded;
            _canDeleteBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Delete)).Succeeded;
            _canExportBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Export)).Succeeded;
            _canSearchBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Stations.Search)).Succeeded;



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
            else
            {
               // Cancel();
            }
            await GetCompaniessAsync();
            await LoadBrandsAsync();

            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }
        private async Task LoadBrandsAsync()
        {
            var mydata = await StationManager.GetAllAsync();
            if (mydata.Succeeded)
            {

                mycompanies = mydata.Data.Where(x => x.KindType == RoleConstants.ManagerRole).ToList();
            }
        }
        private async Task GetCompaniessAsync()
        {
            var response = await StationManager.GetAllAsync();


            if (response.Succeeded)
            {
                //_snackBar.Add(mycomp.ToString() + " =mycomp ", Severity.Error);

                _brandList = response.Data.Where(x=>x.KindType == RoleConstants.StationRole && x.MyCompanyID==mycomp).ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }


        private int selectedItemMyCompany { get; set; }
        private int CheckSelected
        {
            get
            {
                return selectedItemMyCompany;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                selectedEventArgs.Value = value;
                OnChangeSelected(selectedEventArgs);
            }
        }
        private async void OnChangeSelected(ChangeEventArgs e)
        {
            //_snackBar.Add(selectedItemMyCompany.ToString()+ " =selectedItemMyCompany ", Severity.Normal);
            //_snackBar.Add(mycomp.ToString() + " =mycomp ", Severity.Success);


            if (e.Value.ToString() != string.Empty)
            {
                selectedItemMyCompany = (int)e.Value;
                mycomp = selectedItemMyCompany;

                await Reset();
                await Reset();
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
            var response = await StationManager.ExportToExcelAsync(_searchString);
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
                    parameters.Add(nameof(AddEditStationModal.AddEditStationModel), new AddEditStationCommand
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
                        No = _brand.No,
                        Userid = _brand.Userid,
                        
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditStationModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _brand = new GetAllStationsResponse();
            await GetCompaniessAsync();
        }

        private bool Search(GetAllStationsResponse brand)
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