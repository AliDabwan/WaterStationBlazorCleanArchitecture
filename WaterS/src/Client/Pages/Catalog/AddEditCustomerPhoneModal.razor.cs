using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone;
using WaterS.Shared.Constants.Application;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditCustomerPhoneModal
    {
        [Inject] private ICustomerPhoneManager CustomerPhoneManager { get; set; }

        [Parameter] public AddEditCustomerPhoneCommand AddEditCustomerPhoneModel { get; set; } = new();
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
            var response = await CustomerPhoneManager.SaveAsync(AddEditCustomerPhoneModel);
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
        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;
        private int mydriver { get; set; } = 0;
        private int mycustomer { get; set; } = 0;
        private int myAccountId { get; set; } = 0;
        private string CurrentUserId { get; set; }

        private string CurrentUserRool { get; set; } = "x";
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
                mycomp = myUser.Data.KindId;
                mystion = myUser.Data.StationId;
                mydriver = myUser.Data.DriverId;
                mycustomer = myUser.Data.CustomerId;
            }
            //AddEditCustomerPhoneModel.PhoneNumber = "0";
            if (AddEditCustomerPhoneModel.Id==0)
            {
                AddEditCustomerPhoneModel.CustomerId = mycustomer;
                AddEditCustomerPhoneModel.CompanyId = mycomp;
                AddEditCustomerPhoneModel.StationId = mystion;
                AddEditCustomerPhoneModel.DriverId = mydriver;

            }

            await LoadDataAsync();
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

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}