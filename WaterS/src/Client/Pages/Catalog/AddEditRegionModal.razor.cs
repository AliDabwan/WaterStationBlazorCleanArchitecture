using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterS.Application.Features.Regions.Commands.AddEdit;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Region;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditRegionModal
    {
        [Inject] private IRegionManager RegionManager { get; set; }

        [Parameter] public AddEditRegionCommand AddEditRegionModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            //_snackBar.Add("station : "+AddEditRegionModel.StationId, Severity.Error);

            var response = await RegionManager.SaveAsync(AddEditRegionModel);
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
        [Inject] private IStationManager StationManager { get; set; }

        private int mystion { get; set; }
        private int mycomp { get; set; }

        private List<GetAllPagedStationsResponse> mystations = new();

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
        protected  override async Task OnInitializedAsync()
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



             

                    if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole || CurrentUserRool == RoleConstants.ManagerRole)
                    {

                    }
                    else
                    {
                        mycomp = myUser.Data.KindId;
                        mystion = myUser.Data.StationId;

                        AddEditRegionModel.StationId = mystion;

                    }

                

            }


                await LoadDataAsync();
               await LoadStationssAsync();

            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}