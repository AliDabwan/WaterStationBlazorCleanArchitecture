using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Requests.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Pages.Identity
{
    public partial class RegisterUserModal
    {
        
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly RegisterRequest _registerUserModel = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private ClaimsPrincipal _currentUser;

        public string myuser { get; set; }="123";
        public string myRoll { get; set; } = "222";

        private void Cancel()
        {
            MudDialog.Cancel();
        }
     
        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            _currentUser = await _authenticationManager.CurrentUser();
            myuser = (await _authenticationManager.CurrentUser()).Identity.Name;
            if (_currentUser.IsInRole("Administrator"))
            {
                myRoll = "Administrator";
            }
            else if (_currentUser.IsInRole("Admin"))
            {
                myRoll = "Admin";
                _registerUserModel.KindType = "Manager";
            }
            else if (_currentUser.IsInRole("Manager"))
            {
                myRoll = "Manager";
                _registerUserModel.KindType = "Station";

            }
            else if (_currentUser.IsInRole("Station"))
            {
                _registerUserModel.KindType = "Driver";

                myRoll = "Station";
            }
            else if (_currentUser.IsInRole("Driver"))
            {
                myRoll = "Driver";
            }
            else
            {
                myRoll = "Basic";

            }

            _registerUserModel.ActivateUser = true;
            _registerUserModel.AutoConfirmEmail = true;

        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
      



        private async Task SubmitAsync()
        {
            

               var response = await _userManager.RegisterUserAsync(_registerUserModel);

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
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
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
    }
}