using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Talap;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddTalapbyCustomerModal
    {
        [Inject] private ITalapManager TalapManager { get; set; }
        [Parameter] public AddEditTalapCommand AddEditTalapModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
            
        
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; }
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        private string StationrRoll { get; set; } = "Station";
        private string DriverRoll { get; set; } = "Driver";
        private string CustomerRoll { get; set; } = "Customer";
        private int mycomp { get; set; }
        private int mystion { get; set; }
        private int mydriver { get; set; }
        private int mycustomer { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private bool _processingSubmit = false;


        private async Task Confirm()
        {
            if (AddEditTalapModel.BottleNo==0)
            {
                _snackBar.Add("يجب إدخال رقم الخزان لتأكيد الطلب");
                return;
            }
            string OkContent = _localizer["هل انت متأكد ! سيتم تقديم طلب جديد"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.OkConfirmation.ContentText),OkContent}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.OkConfirmation>(_localizer["تأكيد الطلب"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                 await SaveAsync(); 
              
            }
        }

        public CultureInfo _en = CultureInfo.GetCultureInfo("en-US");

        private async Task SaveAsync()
        {

            if (AddEditTalapModel.BottleNo == 0)
            {
                _snackBar.Add("يجب تحديد رقم الخزان", Severity.Warning);
                return;
            }

            _processingSubmit = true;

            var response = await TalapManager.SaveAsync(AddEditTalapModel);
            //await Task.Delay(2000);


            if (response.Succeeded)
            {
                _processingSubmit = false;

                _snackBar.Add(response.Messages[0], Severity.Success);
                try
                {
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateHome);

                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    MudDialog.Close();

                }
                catch
                {

                    MudDialog.Close();

                    return;
                }
            }
            else
            {
                _processingSubmit = false;

                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
                MudDialog.Close();

            }
            //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateHome);

            //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);

        }



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



                if (AddEditTalapModel.Id == 0)
                {
                    mycomp = myUser.Data.KindId;
                    mystion = myUser.Data.StationId;

                    AddEditTalapModel.TalapStatue = "Running";
                    AddEditTalapModel.TalapStatueAr = "جاري الطلب";



                        AddEditTalapModel.CompanyId = myUser.Data.KindId;
                        AddEditTalapModel.StationId = myUser.Data.StationId;
                        AddEditTalapModel.DriverId = myUser.Data.DriverId;
                        AddEditTalapModel.CustomerId = myUser.Data.CustomerId;

                    

                }
                else
                {

                    mycomp = AddEditTalapModel.CompanyId;
                    mystion = AddEditTalapModel.StationId;
                    mydriver = AddEditTalapModel.DriverId;
                    mycustomer = AddEditTalapModel.CustomerId;
                }

             





            }
            else
            {
                Cancel();
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

            await Task.CompletedTask;
        }
    }
}