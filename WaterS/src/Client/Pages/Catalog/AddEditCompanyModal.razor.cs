using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditCompanyModal
    {
        [Inject] private ICompanyManager CompanyManager { get; set; }

        [Parameter] public AddEditCompanyCommand AddEditCompanyModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            AddEditCompanyModel.KindType = RoleConstants.ManagerRole;
            AddEditCompanyModel.KindTypeAr = RoleConstants.ManagerRoleAr ;

            //if (date.HasValue)
            //{
            //    AddEditCompanyModel.ActivateDate = date.Value;


            //}else
            //{
            //    AddEditCompanyModel.ActivateDate = DateTime.Today;

            //}
            var response = await CompanyManager.SaveAsync(AddEditCompanyModel);


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
        //MudDatePicker _picker;
        //DateTime? date = DateTime.Today;
        protected override async Task OnInitializedAsync()
        {
           
            if (AddEditCompanyModel.Id==0)
            {
                AddEditCompanyModel.IsActive = true;
                AddEditCompanyModel.LoginName = "1";
                AddEditCompanyModel.LoginPassword = "";

                AddEditCompanyModel.ActivateDate = DateTime.Today;
                //AddEditCompanyModel.ActivateDate = DateTime.Today;
                AddEditCompanyModel.EndDate = DateTime.Today.AddYears(1);
            }else
            {
               

                    //date= AddEditCompanyModel.ActivateDate;


                
            }

            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            var myCompany = "";
            await Task.CompletedTask;
        }
    }
}