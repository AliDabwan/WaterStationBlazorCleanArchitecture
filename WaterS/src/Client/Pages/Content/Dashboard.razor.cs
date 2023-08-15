using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Dashboard;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Client.Pages.Content
{
    public partial class Dashboard
    {
        [Inject] private IDashboardManager DashboardManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        //[Parameter] public int ProductCount { get; set; }
        [Parameter] public int BottleTypeCount { get; set; }
        //[Parameter] public int BrandCount { get; set; }
        //[Parameter] public int DocumentCount { get; set; }
        //[Parameter] public int DocumentTypeCount { get; set; }
        //[Parameter] public int DocumentExtendedAttributeCount { get; set; }
        [Parameter] public int UserCount { get; set; }
        [Parameter] public int RoleCount { get; set; }


       [Parameter]public int CompanyAdminCount { get; set; }
       [Parameter]public int StationAdminCount { get; set; }
        [Parameter] public int DriverAdminCount { get; set; }
        [Parameter] public int CustomerAdminCount { get; set; }
        [Parameter] public int TalapsCompleted { get; set; }
        [Parameter] public int TalapsInProcess { get; set; }

        [Parameter] public ICollection<GetAllPagedTalapsResponse> AllTalaps { get; set; }
        [Parameter] public int AllTrans { get; set; }

        private ClaimsPrincipal _authenticationStateProviderUser;

        private string CustomerRoll { get; set; } = "Customer";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        private string StationRoll { get; set; } = "Station";
        private string DriverRoll { get; set; } = "Driver";
        private bool _canViewCompanies;
        //private bool _canViewRegions;
        private bool _canViewDriverRegions;
        private bool _canViewCustomers;
        private bool _canViewStations;
        private bool _canViewDrivers;
        private bool _canViewTalaps;


        private readonly string[] _dataEnterBarChartXAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private readonly List<ChartSeries> _dataEnterBarChartSeries = new();
        private bool _loaded;
        private ClaimsPrincipal _currentUser;
        private string CurrentUserId { get; set; }
        private int CurrentUserCustomerId { get; set; }

        private string CurrentUserRool { get; set; } = "";

        protected override async Task OnParametersSetAsync()
        {
            _authenticationStateProviderUser = await _stateProvider.GetAuthenticationStateProviderUserAsync();
        _canViewCompanies = (await _authorizationService.AuthorizeAsync(_authenticationStateProviderUser, Permissions.Companies.View)).Succeeded;
            _canViewStations = (await _authorizationService.AuthorizeAsync(_authenticationStateProviderUser, Permissions.Stations.View)).Succeeded;
            _canViewDrivers = (await _authorizationService.AuthorizeAsync(_authenticationStateProviderUser, Permissions.Drivers.View)).Succeeded;
            _canViewCustomers = (await _authorizationService.AuthorizeAsync(_authenticationStateProviderUser, Permissions.Customers.View)).Succeeded;
            _canViewTalaps = (await _authorizationService.AuthorizeAsync(_authenticationStateProviderUser, Permissions.Talaps.View)).Succeeded;
        }
        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;
                CurrentUserCustomerId = myUser.Data.CustomerId;
            }
            await LoadDataAsync();
            _loaded = true;
            HubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.HubUrl))
            .Build();
            HubConnection.On(ApplicationConstants.SignalR.ReceiveUpdateDashboard, async () =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
            await HubConnection.StartAsync();
        }

        private async Task LoadDataAsync()
        {
            var response = await DashboardManager.GetDataAsync();
            if (response.Succeeded)
            {
               
                UserCount = response.Data.UserCount;
                //RoleCount = response.Data.RoleCount;
                CompanyAdminCount = response.Data.CompanyAdminCount;
                StationAdminCount = response.Data.StationAdminCount;
                DriverAdminCount = response.Data.DriverAdminCount;
                CustomerAdminCount = response.Data.CustomerAdminCount;
                TalapsCompleted = response.Data.TalapsCompleted;
                TalapsInProcess = response.Data.TalapsInProcess;
                AllTalaps = response.Data.AllTalaps;


                decimal getMySum = 0;
                foreach (var item in AllTalaps)
                {
                    if (item.CustomerId== CurrentUserCustomerId)
                    {
                        getMySum =+ item.Price;

                    }
                }
                foreach (var item in response.Data.DataEnterBarChart)
                {
                    _dataEnterBarChartSeries
                        .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                    _dataEnterBarChartSeries.Add(new ChartSeries { Name = item.Name, Data = item.Data });
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}