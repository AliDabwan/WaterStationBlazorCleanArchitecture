using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Regions.Commands.AddEdit;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;
using WaterS.Client.Infrastructure.Managers.Catalog.Region;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Application.Requests.Catalog;

namespace WaterS.Client.Pages.Catalog
{
    public partial class DriverRegionsGroups
    {
        [Inject] private IRegionManager RegionManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllRegionsResponse> _regionList = new();
        private GetAllRegionsResponse _region = new();
        private string _searchString = "";
       

        private ClaimsPrincipal _currentUser;
        private bool _canCreateBrands;
        private bool _canEditBrands;
        private bool _canDeleteBrands;
        private bool _canExportBrands;
        private bool _canSearchBrands;
        private bool _loaded;




        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "x";

        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;
        private int mydriver { get; set; } = 0;


        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Regions.Create)).Succeeded;
            _canEditBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Regions.Edit)).Succeeded;
            _canDeleteBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Regions.Delete)).Succeeded;
            _canExportBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Regions.Export)).Succeeded;
            _canSearchBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Regions.Search)).Succeeded;

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;




            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
                //_snackBar.Add("CurrentUserId= :" + CurrentUserId);

                var myUser = await _userManager.GetAsync(CurrentUserId);
                CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;
                mycomp = myUser.Data.KindId;
                mystion = myUser.Data.StationId;
                mydriver = myUser.Data.DriverId;


                //_snackBar.Add("mydriver= :" + mydriver + "  " + "mystion= :" + mystion + "  " + "mycomp= :" + mycomp);

            }




            await GetRegionssAsync();

            _loaded = true;
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

        private async Task GetRegionssAsync()
        {
            var response = await RegionManager.GetAllAsync();
            if (response.Succeeded)
            {
                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole )
                {
                    _regionList = response.Data.ToList();

                }
                else if (CurrentUserRool == RoleConstants.ManagerRole)
                {
                    _regionList = response.Data.Where(x=>x.Station.CompanyId== mycomp).ToList();
                    //_snackBar.Add(_regionList.Count.ToString());
                }
                else if (CurrentUserRool == RoleConstants.StationRole)
                {
                    _regionList = response.Data.Where(x => x.StationId == mystion).ToList();

                }

                //if (string.IsNullOrEmpty(_searchString))
                //{
                //    _regionList = _regionList.Where(x => x.Name.Contains(_searchString) || x.Station.Name.Contains(_searchString)).ToList();
                //}


                if (_regionList.Any())
                {

                await FillPeople();

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
        private bool Search()
        {

            if (string.IsNullOrWhiteSpace(_searchString))
            {
                FillPeople();
                return true;
            }
            else
            {

            }
             

            return false;
        }
        private async Task ExportToExcel()
        {
            var response = await RegionManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Brands).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Brands exported"]
                    : _localizer["Filtered Brands exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }



        private void OnSearch(string text)
        {
            _searchString = text;
            Reset();
            //StateHasChanged();
        }
        private static IEnumerable<CustomersByGroup> People { get; set; }

        private class Address
        {
            public int regionId { get; set; }

            public string Name { get; set; }
            public string Phone { get; set; }
            public int BottleNo { get; set; }
            public int CustomerCount { get; set; }
            public int Serial { get; set; }

            public string BottleType { get; set; }
        }
        private class CustomersByGroup
        {

            public bool ShowDetails { get; set; }
            public string NameofRegion { get; set; }
            public string StationName { get; set; }

            public int ItemCount { get; set; }
            public int Id { get; set; }
            public int Serial { get; set; }

            public ICollection<Address> CustomerList { get; set; }
        }
        [Inject] private ICustomerManager CustomerManager { get; set; }

        [Inject] private IDriverRegionManager DriverRegionManager { get; set; }
        public int AllCounter { get; set; } = 0;

        private async Task FillPeople()
        {
            IList<CustomersByGroup> people = new List<CustomersByGroup>();
                        IList<Address> address = new List<Address>();



            IList<GetAllPagedDriverRegionsResponse> mydriverList = new List<GetAllPagedDriverRegionsResponse>();
            IList<GetAllPagedCustomersResponse> mycustomerList = new List<GetAllPagedCustomersResponse>();

            var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion, SearchString = "", Orderby = null };


            //IList<Address> addresses = new List<Address>();
            var myDrivers =await DriverRegionManager.GetDriverRegionsAsync(request);
            if (myDrivers.Succeeded)
            {
                //_snackBar.Add("تم جلب السائقين  " + myDrivers.TotalCount, Severity.Success);

                mydriverList = myDrivers.Data;


                var requestCustomer = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion, SearchString = "", Orderby = null };

                //IList<Address> addresses = new List<Address>();
                var myCustomers = await CustomerManager.GetCustomersAsync(requestCustomer);
                if (myCustomers.Succeeded)
                {
                    //_snackBar.Add("تم جلب العملاء   " + myCustomers.TotalCount, Severity.Normal);

                   

                }





            }
            else
            {
                //_snackBar.Add("لايوجد", Severity.Error);

            }
            int serial = 0;
            int serialSub = 0;

            AllCounter = 0;
            //if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
            //{

                foreach (var subitem in _regionList)
                {
                serialSub = 0;
                serial++;
                    foreach (var itemaddress in mydriverList.Where(x => x.RegionId == subitem.Id).ToList())
                    {
                        serialSub++;
                        var customers = mycustomerList.Where(z=>z.DriverId== itemaddress.DriverId &&z.RegionId== subitem.Id);
                        address.Add(new Address
                        {
                            Serial = serialSub,
                            Name = itemaddress.Driver.Name,
                            regionId = itemaddress.RegionId,
                            Phone = itemaddress.Driver.Phone,
                            CustomerCount = customers.Count()
                            //BottleType= itemaddress.BottleType.Name

                        }) ;
                        //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

                    }
                    AllCounter += mydriverList.Where(x => x.RegionId == subitem.Id).Count();
                    people.Add(new CustomersByGroup
                    {
                        Id = subitem.Id,
                        Serial=serial,
                        ShowDetails = false,
                        NameofRegion = subitem.Name,
                        StationName= subitem.Station.Name,
                        CustomerList = address.Where(x => x.regionId == subitem.Id).ToList(), //mydriverList.Where(x => x.RegionId== subitem.Id).ToList(),
                        ItemCount = mydriverList.Where(x => x.RegionId == subitem.Id).Count()
                    });

                }

            //}
            //else if (CurrentUserRool == RoleConstants.ManagerRole)
            //{


            //    foreach (var subitem in _regionList)
            //    {
            //        serial++;

            //        foreach (var itemaddress in mydriverList.Where(x =>x.CompanyId == mycomp && x.RegionId == subitem.Id).ToList())
            //        {
            //            serialSub++;
            //            var customers = mycustomerList.Where(z => z.DriverId == itemaddress.DriverId && z.RegionId == subitem.Id);

            //            address.Add(new Address
            //            {
            //                Serial = serialSub,

            //                Name = itemaddress.Driver.Name,
            //                regionId = itemaddress.RegionId,
            //                Phone = itemaddress.Driver.Phone,
            //                CustomerCount = customers.Count()

            //                //BottleType= itemaddress.BottleType.Name

            //            });
            //            //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

            //        }
            //        AllCounter += mydriverList.Where(x => x.CompanyId == mycomp && x.RegionId == subitem.Id).Count();

            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            Serial = serial,

            //            ShowDetails = false,
            //            NameofRegion = subitem.Name,
            //            StationName = subitem.Station.Name,

            //            CustomerList = address.Where(x => x.regionId == subitem.Id).ToList(), //mydriverList.Where(x => x.RegionId== subitem.Id).ToList(),
            //            ItemCount = mydriverList.Where(x => x.CompanyId == mycomp && x.RegionId == subitem.Id).Count()

            //            //CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp).ToList(),
            //            // CustomerList = mydriverList.Where(x => x.RegionId == subitem.Id ).ToList(),

            //        });

            //    }


            //}
            //else if (CurrentUserRool == RoleConstants.StationRole)
            //{

            //    foreach (var subitem in _regionList)
            //    {
            //        serial++;

            //        foreach (var itemaddress in mydriverList.Where(x =>x.StationId==mystion && x.RegionId == subitem.Id).ToList())
            //        {
            //            serialSub++;
            //            var customers = mycustomerList.Where(z => z.DriverId == itemaddress.DriverId && z.RegionId == subitem.Id);

            //            address.Add(new Address {
            //                Serial = serialSub,

            //                Name = itemaddress.Driver.Name,
            //                regionId = itemaddress.RegionId,
            //                Phone = itemaddress.Driver.Phone,
            //                CustomerCount = customers.Count()


            //            });
            //            //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

            //        }
            //        AllCounter += mydriverList.Where(x => x.StationId == mystion && x.RegionId == subitem.Id).Count();

            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            Serial = serial,

            //            ShowDetails = false,
            //            NameofRegion = subitem.Name,
            //            StationName = subitem.Station.Name,

            //            //CustomerList = subitem.CustomerList.Where(x => x.StationId==mystion).ToList(),
            //            CustomerList = address.Where(x => x.regionId == subitem.Id).ToList(), //mydriverList.Where(x => x.RegionId== subitem.Id).ToList(),

            //            ItemCount = mydriverList.Where(x => x.StationId == mystion && x.RegionId == subitem.Id).Count()
            //        });

            //    }
            //}
            //else
            //{
            //    foreach (var subitem in _regionList)
            //    {
            //        serial++;
            //        AllCounter += subitem.CustomerList.Count();

            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            Serial = serial,

            //            ShowDetails = false,
            //            NameofRegion = subitem.Name,
            //            StationName = subitem.Station.Name,


            //            //CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp && x.StationId == mystion&&x.DriverId==mydriver).ToList(),
            //            //    CustomerList = mydriverList.Where(x => x.RegionId == subitem.Id && x.DriverId == mydriver).ToList(),

            //            ItemCount = subitem.CustomerList.Count()
            //        });

            //    }

            //}


            if (string.IsNullOrEmpty(_searchString))
            {
                People = people;
                //_snackBar.Add("empty "+ People.Count()+" :"+_searchString);

            }
            else
            {
                People = people.Where(x=>x.NameofRegion.Contains(_searchString)||x.StationName.Contains(_searchString));
                //_snackBar.Add("search " + People.Count() + " :" + _searchString);
                StateHasChanged();
            }


            //int iserial = 1;
            //foreach (var item in People)
            //{
            //    foreach (var itemsub in item.CustomerList)
            //    {
            //        itemsub.Serial = iserial;
            //        iserial++;
            //    }
              
            //}

        }

        private void ShowBtnPress(int nr)
        {
            var tmpPerson = People.First(f => f.Id == nr);
            tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
        }


        private async Task Reset()
        {
            //_region = new GetAllRegionsResponse();
            //People = new GetAllRegionsResponse();

            await FillPeople();
        }
    }
}