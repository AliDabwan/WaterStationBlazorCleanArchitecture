using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Drivers.Commands.AddEdit;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Client.Infrastructure.Managers.Catalog.Driver;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditDriverModal
    {
        [Inject] private IDriverManager DriverManager { get; set; }
        [Inject] private IStationManager StationManager { get; set; }
        [Inject] private ICompanyManager CompanyManager { get; set; }
        [Parameter] public AddEditDriverCommand AddEditDriverModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
            
        
        private List<GetAllCompaniesResponse> mycompanies = new ();
        private List<GetAllPagedStationsResponse> mystations = new();
        private List<GetAllPagedDriversResponse> mydrivers = new();
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        private int mycomp { get; set; }
        private int mystion { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            AddEditDriverModel.KindType = RoleConstants.DriverRole;
            AddEditDriverModel.KindTypeAr = RoleConstants.DriverRoleAr ;





            var response = await DriverManager.SaveAsync(AddEditDriverModel);


            if (response.Succeeded)
            {


                try
                {
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);

                }
                catch
                {
                    _snackBar.Add(response.Messages[0], Severity.Success);
                    MudDialog.Close();
                    return;
                }


                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
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
          

            if (e.Value.ToString() != string.Empty)
            {
                selectedItemMyCompany = (int)e.Value;
                mycomp = selectedItemMyCompany;
                mystion = 0;
                AddEditDriverModel.CompanyId = mycomp;
                await LoadStationssAsync();
            }
        }
        private async Task LoadStationssAsync()
        {


            var mydata = await StationManager.GetStationsAsync();
            if (mydata.Succeeded)
            {
                //_snackBar.Add("will change "+ mycomp, Severity.Error);
                mystations = mydata.Data.Where(x => x.CompanyId == mycomp).ToList();
                //_snackBar.Add(" changed" + mycomp, Severity.Info);

                StateHasChanged();

            }


        }
        //public  IMask mask2 = new RegexMask(@"^[1-6]\d{0,5}$");

        protected override async Task OnInitializedAsync()
        {

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                //CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;



                if (AddEditDriverModel.Id == 0)
                {

                    AddEditDriverModel.LoginName = "1";
                    AddEditDriverModel.LoginPassword = "";

                    mycomp = myUser.Data.KindId;
                    mystion = myUser.Data.StationId;
                    if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                    {

                    }
                    else
                    {
                        AddEditDriverModel.CompanyId = myUser.Data.KindId;

                        AddEditDriverModel.StationId = myUser.Data.StationId;

                    }
                 
                }
                else
                {

                    mycomp = AddEditDriverModel.CompanyId;
                    mystion = AddEditDriverModel.StationId;
                    CheckSelected = mycomp;
                }

             





            }
            else
            {
                Cancel();
            }
            await LoadDataAsync();


            await LoadstationsAsync();
            try
            {
                HubConnection = HubConnection.TryInitialize(_navigationManager);
                if (HubConnection.State == HubConnectionState.Disconnected)
                {
                    await HubConnection.StartAsync();
                }
            }
            catch (Exception)
            {

                return ;
            }
            
           
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }


        private async Task LoadcompaniesAsync()
        {
            var mydata = await CompanyManager.GetAllAsync();
            if (mydata.Succeeded)
            {

                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    mycompanies = mydata.Data;


                }
                else

                {
                    mycomp = AddEditDriverModel.CompanyId;
                    mycompanies = mydata.Data.Where(x => x.Id == mycomp).ToList();

                }
            }
        }

        private async Task LoadstationsAsync()
        {
            var mydata = await StationManager.GetStationsAsync();
            if (mydata.Succeeded)
            {
                mycomp = AddEditDriverModel.CompanyId;
                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole|| CurrentUserRool == RoleConstants.ManagerRole)
                {
                    mystations = mydata.Data;


                
                    mystations = mydata.Data.Where(x => 
                 x.CompanyId == mycomp).ToList();

                }
                else

                {
                    mystations = mydata.Data.Where(x => 
                                  x.CompanyId == mycomp && x.Id==mystion).ToList();
                }
              
            }
        }
        private async Task LoadDataAsync()
        {
            await LoadcompaniesAsync();

            var myCompany = "";
            await Task.CompletedTask;
        }
    }
}