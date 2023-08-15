using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Client.Infrastructure.Managers.Catalog.Region;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.BottleTypes.Queries.GetAll;
using WaterS.Client.Infrastructure.Managers.Catalog.BottleType;
using System.Globalization;
using WaterS.Application.Requests.Catalog;
using Microsoft.JSInterop;
using WaterS.Extensions;
//using GoogleMapsComponents;
//using GoogleMapsComponents.Maps;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditCustomerModal
    {


        [Inject] private ICustomerManager CustomerManager { get; set; }
        [Inject] private IStationManager StationManager { get; set; }
        [Inject] private ICompanyManager CompanyManager { get; set; }
        [Inject] private IDriverRegionManager DriverRegionManager { get; set; }
        [Inject] private IRegionManager RegionManager { get; set; }
        [Inject] private IBottleTypeManager BottleTypeManager { get; set; }
        [Parameter] public AddEditCustomerCommand AddEditCustomerModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        //[CascadingParameter] private HubConnection HubConnection { get; set; }
            
        
        private List<GetAllCompaniesResponse> mycompanies = new ();
        private List<GetAllPagedStationsResponse> mystations = new();
        private List<GetAllPagedDriverRegionsResponse> myregions = new();
        private List<GetAllBottleTypesResponse> bottletypes = new();
        private List<GetAllPagedDriverRegionsResponse> mydrivers = new(); 
                    private List<GetAllPagedDriverRegionsResponse> mydata = new(); 

        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "";
        public string CurrentRool { get; set; } = "";
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        private string StationRoll { get; set; } = "Station";
        private string DriverRoll { get; set; } = "Driver";
        private int mycomp { get; set; }
            private int mystion { get; set; }
        private int mybottletype { get; set; }

        private int mydriver { get; set; } = 0;
        private int myregion { get; set; } = 0;
        //private GoogleMap map1;
        //private MapOptions mapOptions;
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private string Height = "300px"; //Now you can use this to dynamically set the height


        public CultureInfo _en = CultureInfo.GetCultureInfo("en-US");

        //void domsg()
        //{
        //    _snackBar.Add(" 1" + CurrentRool, Severity.Error);

        //}
        //void domsg2()
        //{
        //    _snackBar.Add(" 2" + CurrentRool, Severity.Info);
        //}


        protected override async Task OnInitializedAsync()
        {
            //await jsRuntime.InitializeCarousel();




            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);

                //CurrentUserRool = myUser.Data.KindType;
                CurrentRool = myUser.Data.KindType;


              

                if (AddEditCustomerModel.Id == 0)
                {
                    mycomp = myUser.Data.KindId;
                    mystion = myUser.Data.StationId;
                    mydriver = myUser.Data.DriverId;
                    _loaded = false;
                    AddEditCustomerModel.Phone = "0";
                if (CurrentRool == RoleConstants.ManagerRole )
                    {
                        //Cancel();
                        mycomp = myUser.Data.KindId;
                        AddEditCustomerModel.CompanyId = myUser.Data.KindId;
                        _loaded = false;

                        await LoadStationssAsync();



                    }
                    else if (CurrentRool == RoleConstants.StationRole)
                    {
                        //Cancel();
                        mycomp = myUser.Data.KindId;
                        mystion = myUser.Data.StationId;
                        AddEditCustomerModel.CompanyId = mycomp;

                        AddEditCustomerModel.StationId = mystion;
                        CheckStationSelected = mystion;
                        await LoadRegionsAsync();


                        //CheckStationSelected = mystion;

                    }
                    else
                    {
                        mycomp = myUser.Data.KindId;

                        mystion = myUser.Data.StationId;

                        AddEditCustomerModel.CompanyId = mycomp;

                        AddEditCustomerModel.StationId = mystion;




                        AddEditCustomerModel.DriverId = myUser.Data.DriverId;


                        await LoadRegionsAsync();

                    }
                    //_snackBar.Add(" myUser.Data.KindId" + myUser.Data.KindId, Severity.Error);

                    OpenOverlay();



                }
                else
                {
                    mycomp = AddEditCustomerModel.CompanyId;
                    mystion = AddEditCustomerModel.StationId;
                    mydriver = AddEditCustomerModel.DriverId;
                    myregion = AddEditCustomerModel.RegionId;
                    //if (myregion != 0)
                    //{
                    //    selectedRegionValue = myregion;

                    //}
                    if (CurrentRool == RoleConstants.ManagerRole)
                    {
                        await LoadStationssAsync();

                        CheckStationSelected = mystion;


                    }
                    await LoadRegionsAsync();

                    //CheckStationSelected = mystion;
                    //CheckSelected = mycomp;
                    //CheckStationSelected = mystion;
                    //selectedRegionValue = myregion;
                }

                //await LoadDataAsync();
                //await LoadStationssAsync();
                //await LoadRegionsAsync();
                await LoadBottleTypesAsync();

                if (myregion!=0)
                {
                    selectedRegionValue = myregion;

                }

                //_snackBar.Add("oninitioalize 5", Severity.Error);




            }
            else
            {
                Cancel();
            }





            //HubConnection = HubConnection.TryInitialize(_navigationManager);
            //if (HubConnection.State == HubConnectionState.Disconnected)
            //{
            //    await HubConnection.StartAsync();
            //}
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }
        private bool _loaded = false;
        private bool _loadedDriver = false;

        private async Task SaveAsync()
        {
            if (_processing)
            {
                return;
            }

            if (AddEditCustomerModel.DriverId==0)
            {
                _snackBar.Add("يجب إختيار السائق", Severity.Warning);
                return;
            }
            if (AddEditCustomerModel.RegionId == 0)
            {
                _snackBar.Add("يجب إختيار المنطقة", Severity.Warning);
                return;
            }
            if (AddEditCustomerModel.DriverId == 0)
            {
                _snackBar.Add("يجب إختيار السائق", Severity.Warning);
                return;
            }
            _processing = true;

            var response = await CustomerManager.SaveAsync(AddEditCustomerModel);
            //await Task.Delay(2000);


            if (response.Succeeded)
            {
                _processing = false;

                _snackBar.Add(response.Messages[0], Severity.Success);

                try
                {
                    //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);

                }
                catch
                {

                    MudDialog.Close();
                    _processing = false;

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



        private int selectedRegion { get; set; }
        private int newDriver { get; set; }

        private int selectedRegionValue
        {
            get
            {
                return selectedRegion;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                selectedEventArgs.Value = value;
                OnRegionChangeSelected(selectedEventArgs);
            }
        }
        private async void OnRegionChangeSelected(ChangeEventArgs e)
        {


            if (e.Value.ToString() != string.Empty)
            {
                _loadedDriver = false;
                selectedRegion = (int)e.Value;
                myregion = selectedRegion;
                AddEditCustomerModel.RegionId = myregion;
                await LoadDriversAsync();
                //Task.Delay(2000).Wait();

                if (AddEditCustomerModel.Id==0)
                {
                    //_snackBar.Add("----1" + newDriver.ToString());
                    AddEditCustomerModel.DriverId = newDriver;

                }
                else
                {
                    AddEditCustomerModel.DriverId = mydriver;


                }
                StateHasChanged();
            }
        }




        private bool isVisible;
        private bool dataLoaded;


        public async void OpenOverlay()
        {
            isVisible = true;
            await Task.Delay(1000);
            isVisible = false;
            dataLoaded = true;
            StateHasChanged();
        }
        public void ResetExample()
        {
            dataLoaded = false;
        }



        //private int selectedItemMyCompany { get; set; }
        //private int CheckCompanySelected
        //{
        //    get
        //    {
        //        return selectedItemMyCompany;
        //    }
        //    set
        //    {
        //        ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
        //        selectedEventArgs.Value = value;
        //        OnChangeSelected(selectedEventArgs);
        //    }
        //}
        //private async void OnChangeSelected(ChangeEventArgs e)
        //{


        //    if (e.Value.ToString() != string.Empty)
        //    {
        //        selectedItemMyCompany = (int)e.Value;
        //        mycomp = selectedItemMyCompany;
        //        mystion = 0;
        //        AddEditCustomerModel.CompanyId = mycomp;
        //        await LoadStationssAsync();
        //    }
        //}


        private int selectedItemMyStation { get; set; }
        private int CheckStationSelected
        {
            get
            {
                return selectedItemMyStation;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                
                selectedEventArgs.Value = value;
                OnStationChangeSelected(selectedEventArgs);
            }
        }
        private async void OnStationChangeSelected(ChangeEventArgs e)
        {


            if (e.Value.ToString() != string.Empty)
            {
                selectedItemMyStation = (int)e.Value;
                mystion = selectedItemMyStation;
                //mydriver = 0;
                AddEditCustomerModel.StationId = mystion;
                await LoadRegionsAsync();
            }
        }
      
        
        
        
        
        
        
        
        
        
        private async Task LoadStationssAsync()
        {


            var mydata = await StationManager.GetStationsAsync();
            if (mydata.Succeeded)
            {
                //_snackBar.Add("will change "+ mycomp, Severity.Error);
                //_snackBar.Add(" changed" + mycomp, Severity.Info);
             if (CurrentRool == RoleConstants.ManagerRole)

                {
                    mystations = mydata.Data.Where(x => x.CompanyId == mycomp).ToList();

                }
                else if (CurrentRool == RoleConstants.StationRole)

                {
                    mystations = mydata.Data.Where(x => x.CompanyId == mycomp &&x.Id==mystion).ToList();

                }
                else if (CurrentRool == RoleConstants.DriverRole)

                {
                    mystations = mydata.Data.Where(x => x.CompanyId == mycomp && x.Id == mystion).ToList();

                }
                //StateHasChanged();

            }


        }
        private async Task LoadDriversAsync()
        {
            ResetExample();
            var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, SearchString = "", StationId = mystion, DriverId = 0, Orderby = null };

            var mydata = await DriverRegionManager.GetDriverRegionsAsync(request);
            //_snackBar.Add("mydata 1 :" + mydata.Data.Count, Severity.Success);

            if (mydata.Succeeded)
            {
                mydrivers = mydata.Data.Where(x=>x.RegionId==myregion && x.StationId==mystion).ToList();
                //_snackBar.Add("mydrivers 1 :" + mydrivers.Count, Severity.Warning);

                if (mydrivers.Count <= 0)
                {
                    newDriver = 0;
                    mydriver = 0;

                    //mydrivers = null;

                }
                else
                {
                   
                        newDriver = mydrivers.Select(x => x.DriverId).FirstOrDefault();

                    mydriver = newDriver;
                    //_snackBar.Add("----2" + newDriver.ToString());

                    _loadedDriver = true;


                }


                OpenOverlay();

            }
            else
            {
                OpenOverlay();

                //mydrivers = null;
            }


        }
        private bool _processing = false;

     
        private async Task LoadRegionsAsync()
        {





            if (CurrentRool == RoleConstants.AdministratorRole || CurrentRool == RoleConstants.AdminRole)
            {

                var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, SearchString = "", StationId = 0, DriverId = 0, Orderby = null };


                var mydata = await DriverRegionManager.GetDriverRegionsAsync(request);
                if (mydata.Succeeded)
                {

                    myregions = mydata.Data;
                }
            }
            else if (CurrentRool == RoleConstants.ManagerRole)
            {
                var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, SearchString = "", StationId = 0, DriverId = 0, Orderby = null };


                var mydata = await DriverRegionManager.GetDriverRegionsAsync(request);
                if (mydata.Succeeded)
                {

                    myregions = mydata.Data;
                }
                //myregions = mydata.Data.Where(x => x.StationId == mystion).ToList();
                //newDriver = myregions.Select(x => x.DriverId).FirstOrDefault();

            }
            else if (CurrentRool == RoleConstants.StationRole)

            {
                var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, SearchString = "", StationId = mystion, DriverId = 0, Orderby = null };


                var mydata = await DriverRegionManager.GetDriverRegionsAsync(request);
                if (mydata.Succeeded)
                {

                    myregions = mydata.Data;
                }
                //myregions = mydata.Data.Where(x => x.StationId == mystion).ToList();
                //newDriver = myregions.Select(x => x.DriverId).FirstOrDefault();

            }
            else if (CurrentRool == RoleConstants.DriverRole)

            {
                var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, SearchString = "", StationId = mystion, DriverId = mydriver, Orderby = null };


                var mydata = await DriverRegionManager.GetDriverRegionsAsync(request);
                if (mydata.Succeeded)
                {

                    myregions = mydata.Data;
                }
                //myregions = mydata.Data.Where(x => x.DriverId == mydriver).ToList();
                //newDriver = myregions.Select(x => x.DriverId).FirstOrDefault();

            }
                _loaded = true;

                if (myregions.Count<=0)
                {
                    selectedRegionValue = 0;

                }
                StateHasChanged();

            
        }
     
        private async Task LoadBottleTypesAsync()
        {
            var mydata = await BottleTypeManager.GetAllAsync();
            //_snackBar.Add("getbottle");

            if (mydata.Succeeded)
            {
                //_snackBar.Add(mydata.Data.Count.ToString());
                bottletypes = mydata.Data;



            }
        }
        private async Task LoadcompaniesAsync()
        {
            var mydata = await CompanyManager.GetAllAsync();
            if (mydata.Succeeded)
            {

                if (CurrentRool == RoleConstants.AdministratorRole || CurrentRool == RoleConstants.AdminRole)
                {
                    mycompanies = mydata.Data;


                }
                else

                {
                    mycompanies = mydata.Data.Where(x => x.Id == mycomp).ToList();

                }
            }
        }

        //private async Task LoadstationsAsync()
        //{
        //    _snackBar.Add("oninitioalize 6", Severity.Error);

        //    var mydata = await StationManager.GetStationsAsync();
        //    _snackBar.Add("oninitioalize 7"+ mydata.TotalCount, Severity.Error);

        //    if (mydata.Succeeded)
        //    {
        //        mycomp = AddEditCustomerModel.CompanyId;
        //        if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole|| CurrentUserRool == RoleConstants.ManagerRole)
        //        {
                  

                
        //            mystations = mydata.Data.Where(x => 
        //         x.CompanyId == mycomp).ToList();

        //        }
        //        else

        //        {
        //            mystations = mydata.Data.Where(x => 
        //                          x.CompanyId == mycomp && x.Id==mystion).ToList();
        //        }
              
        //    }
        //}
        private async Task LoadDataAsync()
        {
            await LoadcompaniesAsync();

            await Task.CompletedTask;
        }
    }
}