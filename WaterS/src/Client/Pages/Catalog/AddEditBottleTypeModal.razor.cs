﻿using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.BottleType;
using WaterS.Shared.Constants.Application;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditBottleTypeModal
    {
        [Inject] private IBottleTypeManager BottleTypeManager { get; set; }

        [Parameter] public AddEditBottleTypeCommand AddEditBottleTypeModel { get; set; } = new();
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
            var response = await BottleTypeManager.SaveAsync(AddEditBottleTypeModel);
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