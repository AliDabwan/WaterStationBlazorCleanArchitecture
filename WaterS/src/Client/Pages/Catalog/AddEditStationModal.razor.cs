using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.Stations.Commands.AddEdit;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditStationModal
    {
        [Inject] private IStationManager StationManager { get; set; }
        [Inject] private ICompanyManager CompanyManager { get; set; }

        [Parameter] public AddEditStationCommand AddEditStationModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
            
        
        private List<GetAllCompaniesResponse> mycompanies = new ();
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private int mycomp { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    
        public void Cancel()
        {
            MudDialog.Cancel();
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

        private async Task SaveAsync()
        {
            AddEditStationModel.KindType = RoleConstants.StationRole;
            AddEditStationModel.KindTypeAr = RoleConstants.StationRoleAr ;
            AddEditStationModel.MyCompanyId = AddEditStationModel.CompanyId ;



           

            var response = await StationManager.SaveAsync(AddEditStationModel);


            if (response.Succeeded)
            {
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

            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
        }

        protected override async Task OnInitializedAsync()
        {


            //_snackBar.Add(AddEditStationModel.Id.ToString());

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;


            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);

                CurrentUserRool = myUser.Data.KindType;


                if (AddEditStationModel.Id==0)
                {

                    AddEditStationModel.LoginName = "1";
                    AddEditStationModel.LoginPassword = "";


                    //await LoadDataAsync();

                    if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                    {

                    }
                    else
                    {
                        mycomp = myUser.Data.KindId;

                        AddEditStationModel.CompanyId = myUser.Data.KindId;

                        AddEditStationModel.MyCompanyId = mycomp;

                    }
                    AddEditStationModel.KindType = RoleConstants.StationRole;
                    AddEditStationModel.KindTypeAr = RoleConstants.StationRoleAr;

                }
                else
                {
                    mycomp = AddEditStationModel.CompanyId;

                }

                await LoadDataAsync();



            }
            else
            {
                Cancel();
            }


            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }



        private async Task LoadBrandsAsync()
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
                    mycompanies = mydata.Data.Where(x => x.Id == mycomp).ToList();

                }
            }
        }
        private async Task LoadDataAsync()
        {
            await LoadBrandsAsync();

            var myCompany = "";
            await Task.CompletedTask;
        }
    }
}