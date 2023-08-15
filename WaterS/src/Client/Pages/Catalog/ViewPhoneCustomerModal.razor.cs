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
using WaterS.Application.Features.CustomerPhones.Queries.GetAll;
using WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone;
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;

//using GoogleMapsComponents;
//using GoogleMapsComponents.Maps;

namespace WaterS.Client.Pages.Catalog
{
    public partial class ViewPhoneCustomerModal
    {
        [Inject] private ICustomerManager CustomerManager { get; set; }
        
        [Parameter] public AddEditCustomerPhoneCommand AddEditCustomerModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        //[CascadingParameter] private HubConnection HubConnection { get; set; }

        [Inject] private ICustomerPhoneManager CustomerPhoneManager { get; set; }

        private List<GetAllCustomerPhonesResponse> _phones = new();

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

        public string phone1 { get; set; } = "";
        public string phone2 { get; set; } = "";
        public string phone3 { get; set; } = "";
        public string phone4 { get; set; } = "";
        public string phone5 { get; set; } = "";

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
                    AddEditCustomerModel.PhoneNumber = "0";
                if (CurrentRool == RoleConstants.ManagerRole )
                    {
                        //Cancel();
                        mycomp = myUser.Data.KindId;
                        AddEditCustomerModel.CompanyId = myUser.Data.KindId;
                        _loaded = false;




                    }
                    else if (CurrentRool == RoleConstants.StationRole)
                    {
                        //Cancel();
                        mycomp = myUser.Data.KindId;
                        mystion = myUser.Data.StationId;
                        AddEditCustomerModel.CompanyId = mycomp;

                        AddEditCustomerModel.StationId = mystion;
                   


                        //CheckStationSelected = mystion;

                    }
                    else
                    {
                        mycomp = myUser.Data.KindId;

                        mystion = myUser.Data.StationId;

                        AddEditCustomerModel.CompanyId = mycomp;

                        AddEditCustomerModel.StationId = mystion;




                        AddEditCustomerModel.DriverId = myUser.Data.DriverId;



                    }
                    //_snackBar.Add(" myUser.Data.KindId" + myUser.Data.KindId, Severity.Error);




                }
                else
                {
                    mycomp = AddEditCustomerModel.CompanyId;
                    mystion = AddEditCustomerModel.StationId;
                    mydriver = AddEditCustomerModel.DriverId;
                    //if (myregion != 0)
                    //{
                    //    selectedRegionValue = myregion;

                 
                }

           

              
                var customer = await CustomerPhoneManager.GetAllAsync();

                if (customer.Succeeded)
                {
                    _loaded = true;

                    //_snackBar.Add("customer 5" + customer.Data.Count, Severity.Error);

                    _phones = customer.Data.Where(x=>x.CustomerId== AddEditCustomerModel.CustomerId).ToList();
                    if (_phones.Count>0)
                    {
                        phone1 = _phones[0].PhoneNumber;

                    }
                    if (_phones.Count > 1)
                    {
                        phone2 = _phones[1].PhoneNumber;

                    }
                    if (_phones.Count > 2)
                    {
                        phone3 = _phones[2].PhoneNumber;

                    }
                    if (_phones.Count > 3)
                    {
                        phone4 = _phones[3].PhoneNumber;

                    }
                    if (_phones.Count > 4)
                    {
                        phone5 = _phones[4].PhoneNumber;

                    }
                }




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
            _processing = true;

            if (!string.IsNullOrEmpty(phone1))
            {
                if (_phones.Count > 0)
                {
                    AddEditCustomerModel.Id = _phones[0].Id;
                    var response = await CustomerPhoneManager.SaveAsync(AddEditCustomerModel);
                    if (response.Succeeded)
                    {
                        _snackBar.Add(response.Messages[0], Severity.Success);
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

            if (!string.IsNullOrEmpty(phone2))
            {
                if (_phones.Count > 1)
                {
                    AddEditCustomerModel.Id = _phones[1].Id;
                    var response = await CustomerPhoneManager.SaveAsync(AddEditCustomerModel);
                    if (response.Succeeded)
                    {
                        _snackBar.Add(response.Messages[0], Severity.Success);
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

            //await Task.Delay(2000);




            _processing = false;

            MudDialog.Close();

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











        private bool _processing = false;

     
        private async Task LoadDataAsync()
        {

            await Task.CompletedTask;
        }
    }
}