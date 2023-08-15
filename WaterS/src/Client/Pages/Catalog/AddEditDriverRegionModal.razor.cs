using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;
using WaterS.Application.Requests;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;
using WaterS.Client.Infrastructure.Managers.Catalog.Region;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;
using WaterS.Client.Infrastructure.Managers.Catalog.Driver;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Application.Requests.Catalog;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditDriverRegionModal
    {
        [Inject] private IDriverRegionManager DriverRegionManager { get; set; }
        [Inject] private IRegionManager RegionManager { get; set; }
        [Inject] private IDriverManager DriverManager { get; set; }
        [Inject] private IStationManager StationManager { get; set; }
        [Inject] private ICompanyManager CompanyManager { get; set; }

        [Parameter] public AddEditDriverRegionCommand AddEditDriverRegionModel { get; set; } = new();

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }



        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private List<GetAllPagedDriversResponse> _drivers = new();
        private List<GetAllRegionsResponse> _regions = new();
        private List<GetAllPagedStationsResponse> _stations = new();
        private List<GetAllCompaniesResponse> _companies = new();
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            //_snackBar.Add("region :"+AddEditDriverRegionModel.RegionId, Severity.Error);
            //_snackBar.Add("station :" + AddEditDriverRegionModel.StationId, Severity.Error);
            _processing = true;
            var response = await DriverRegionManager.SaveAsync(AddEditDriverRegionModel);
            if (response.Succeeded)
            {
                _processing = false;


                _snackBar.Add(response.Messages[0], Severity.Success);
                try
                {
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);

                }
                catch 
                {

                    return;
                }
                MudDialog.Close();
            }
            else
            {
                _processing = false;

                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {

            _loaded = false;

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                //CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;




                if (AddEditDriverRegionModel.Id == 0)
                {
                    //_snackBar.Add("333333333" + 1, Severity.Normal);

                    mycomp = myUser.Data.KindId;
                    mystion = myUser.Data.StationId;
                    await LoadDataAsync();

                    if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                    {
                        await LoadStationsAsync();

                    }
                    else if (CurrentUserRool == RoleConstants.ManagerRole)
                    {
                        AddEditDriverRegionModel.CompanyId = myUser.Data.KindId;

                        await LoadStationsAsync();
                    }
                    else if (CurrentUserRool == RoleConstants.StationRole)
                    {
                        AddEditDriverRegionModel.CompanyId = myUser.Data.KindId;
                        AddEditDriverRegionModel.StationId = myUser.Data.StationId;

                        await LoadDriversAsync();
                    }
                    else
                    {
                        AddEditDriverRegionModel.CompanyId = myUser.Data.KindId;

                        AddEditDriverRegionModel.StationId = myUser.Data.StationId;

                    }

                }
                else
                {
                    mycomp = AddEditDriverRegionModel.CompanyId;
                    mystion = AddEditDriverRegionModel.StationId;
                    //_snackBar.Add("1- mycomp" + mycomp, Severity.Normal);
                    //_snackBar.Add("2- mystion" + mystion, Severity.Success);
                    //await LoadDataAsync();


                    mystationValue = mystion;
                    mycompValue = mycomp;

                    //await LoadStationsAsync();
                    await LoadDriversAsync();

                }


                await LoadRegionsAsync();

                _loaded = true;




            }

            else
            {
                //Cancel();
            }

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

        private async Task LoadDataAsync()
        {                
            //_snackBar.Add(CurrentUserRool, Severity.Success);

            //await LoadDriversAsync();
            await LoadComaniesAsync();
            //await LoadRegionsAsync();
        }



        private int selectedMyCompany { get; set; }


        private int mycompValue
        {
            get
            {
                return selectedMyCompany;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                selectedEventArgs.Value = value;
                OncompanySelected(selectedEventArgs);
            }
        }
        private async void OncompanySelected(ChangeEventArgs e)
        {


            if (e.Value.ToString() != string.Empty)
            {
                selectedMyCompany = (int)e.Value;
                mycomp = selectedMyCompany;
                mystion = 0;
                AddEditDriverRegionModel.CompanyId = mycomp;
                await LoadStationsAsync();
            }
        }










        private bool _processing = false;


        private int selectedMyStaion { get; set; }


        private int mystationValue
        {
            get
            {
                return selectedMyStaion;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                selectedEventArgs.Value = value;
                OnstaionSelected(selectedEventArgs);
            }
        }
        private async void OnstaionSelected(ChangeEventArgs e)
        {


            if (e.Value.ToString() != string.Empty)
            {
                selectedMyStaion = (int)e.Value;
                mystion = selectedMyStaion;
                AddEditDriverRegionModel.StationId = mystion;
                //_snackBar.Add("OnstaionSelected");
                await LoadDriversAsync();
            }
        }




        private async Task LoadDriversAsync()
        {
            var request = new GetAllPagedDriversRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion,withOutInclud=1 ,DriverId = 0, Orderby = null };
            var response = await DriverManager.GetDriversAsync(request);

            //_snackBar.Add("TotalCount= :" + response.TotalCount);

            if (response.Succeeded)
            {



                if (AddEditDriverRegionModel.Id!=0)
                {
                    mycomp = AddEditDriverRegionModel.CompanyId;
                    mystion = AddEditDriverRegionModel.StationId;

                }

                //_snackBar.Add(_drivers.Count.ToString() + "  5" + mystion, Severity.Error);

                _drivers = response.Data.Where(x=>x.StationId== mystion).ToList();
                //_snackBar.Add(_drivers.Count.ToString()+ "6" + mystion, Severity.Info);

            }
        }

        private async Task LoadComaniesAsync()
        {
            var data = await CompanyManager.GetAllAsync();
            if (data.Succeeded)
            {


                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    //_snackBar.Add(_drivers.Count.ToString() + "  1=" + mystion, Severity.Error);

                    _companies = data.Data;
                    //_snackBar.Add(_drivers.Count.ToString() + "  2=" + mystion, Severity.Error);


                }
                else
                {
                    _companies = data.Data.Where(x=>x.Id== mycomp).ToList();

                }
                //_snackBar.Add(_companies.Count.ToString(), Severity.Success);


            }
        }
        private async Task LoadStationsAsync()
        {
            var data = await StationManager.GetStationsAsync();
            if (data.Succeeded)
            {


                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole || CurrentUserRool == RoleConstants.ManagerRole)
                {
                    //_snackBar.Add(_drivers.Count.ToString() + "  3=" + mystion, Severity.Error);

                    _stations = data.Data.Where(x =>(mycomp==0|| x.CompanyId == mycomp)).ToList();
                    //_snackBar.Add(_drivers.Count.ToString() + "  4=" + mystion, Severity.Error);


                }
                else
                {
                    _stations = data.Data.Where(x => x.CompanyId == mycomp && x.Id == mystion).ToList();

                }
            }
        }
        private async Task LoadRegionsAsync()
        {
            var data = await RegionManager.GetAllAsync();
            if (data.Succeeded)
            {
                if (AddEditDriverRegionModel.Id!=0)
                {
                    mystion = AddEditDriverRegionModel.StationId;

                }

                _regions = data.Data.Where(x=>x.StationId== mystion).ToList();
                //_snackBar.Add(_regions.Count.ToString(), Severity.Success);
                //_snackBar.Add("mystion    - " + mystion + "    " + _regions.Count.ToString(), Severity.Success);

            }
            _loaded = true;

        }


        private IBrowserFile _file;

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var extension = Path.GetExtension(_file.Name);
                var format = "image/png";
                var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                   }
        }

        private async Task<IEnumerable<int>> SearchDrivers(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _drivers.Select(x => x.Id);

            return _drivers.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }
        private async Task<IEnumerable<int>> SearchCompanies(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _companies.Select(x => x.Id);

            return _companies.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }
        private async Task<IEnumerable<int>> SearchStations(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _stations.Select(x => x.Id);

            return _stations.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }
        private async Task<IEnumerable<int>> SearchRegions(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(2);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
            {
                //_snackBar.Add("  :seatch=" + value, Severity.Success);
                return _regions.Select(i=>i.Id);

            }

            return _regions.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Id);
        }
    }
}