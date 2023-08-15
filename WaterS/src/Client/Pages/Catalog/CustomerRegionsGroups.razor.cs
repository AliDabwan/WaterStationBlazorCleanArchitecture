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
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Client.Infrastructure.Managers.Catalog.Region;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Requests.Catalog;

namespace WaterS.Client.Pages.Catalog
{
    public partial class CustomerRegionsGroups
    {
        [Inject] private IDriverRegionManager RegionManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllPagedDriverRegionsResponse> _regionList = new();
        private GetAllPagedDriverRegionsResponse _region = new();
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
            var request = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0+ 1, CompanyId=mycomp,StationId=mystion,DriverId= mydriver, SearchString = _searchString, Orderby = null };

            var response = await RegionManager.GetDriverRegionsAsync(request);
            if (response.Succeeded)
            {

                //if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                //{
                    _regionList = response.Data.ToList();

                //}
                //else if (CurrentUserRool == RoleConstants.ManagerRole)
                //{
                //    _regionList = response.Data.Where(x => x.Station.CompanyId == mycomp).ToList();
                //}
                //else if (CurrentUserRool == RoleConstants.StationRole)
                //{
                //    _regionList = response.Data.Where(x => x.StationId == mystion).ToList();

                //}
                //else if (CurrentUserRool == RoleConstants.DriverRole)
                //{
                //    _regionList = response.Data.Where(x => x.DriverId == mydriver).ToList();

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
        private bool Search(GetAllRegionsResponse brand)
        {
           
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
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
                    FileName = $"{nameof(DriverRegion).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
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


     

        private static IEnumerable<CustomersByGroup> People { get; set; }

        private class Address
        {
            public int serial { get; set; }

            public int regionId { get; set; }

            public string Name { get; set; }
            public string Phone { get; set; }
            public int BottleNo { get; set; }
            public string BottleType { get; set; }
        }
        private class CustomersByGroup
        {
            public int serial { get; set; }

            public bool ShowDetails { get; set; }
            public string NameofRegion { get; set; }
            public int ItemCount { get; set; }
            public int Id { get; set; }

            public ICollection<Address> CustomerList { get; set; }
        }

        [Inject] private ICustomerManager CustomerManager { get; set; }

        public int AllCounter { get; set; } = 0;

        int serialRegion = 0;
        int serialCustomer = 0;
        private IEnumerable<GetAllPagedCustomersResponse> _pagedDataCustomers;
        private int _currentPageCustomer;

        private async Task FillPeople()
        {
            IList<CustomersByGroup> people = new List<CustomersByGroup>();
                        IList<Address> address = new List<Address>();

            IList<GetAllPagedCustomersResponse> mycustomerList = new List<GetAllPagedCustomersResponse>();

        var request = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber =  1, CompanyId = mycomp, StationId = mystion,DriverId=mydriver, SearchString = _searchString, Orderby = null };

            var response = await CustomerManager.GetCustomersAsync(request);

            //_snackBar.Add("TotalCount= :" + response.TotalCount);



            //    var requestCustomer = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber = 1 + 1, SearchString = "", Orderby = null };

            //var myCustomers =await CustomerManager.GetCustomersAsync(requestCustomer);
            if (response.Succeeded)
            {

                _currentPageCustomer = response.CurrentPage;
                AllCounter = response.TotalCount;
                _pagedDataCustomers = response.Data;

                //AllCounter = myCustomers.TotalCount;

                //_snackBar.Add(" تم جلب العملاء  " + response.TotalCount, Severity.Success);


                //_snackBar.Add(" تم    " + _pagedDataCustomers.Count(), Severity.Success);

                //mycustomerList = myCustomers.Data;


            }else
            {
                //_snackBar.Add("لايوجد", Severity.Error);

            }

            //if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
            //{

                serialRegion = 0;

                foreach (var subitem in _regionList)
                {
                    serialRegion++;
                    serialCustomer = 0;

                    foreach (var itemaddress in _pagedDataCustomers.Where(x => x.RegionId == subitem.RegionId).ToList())
                    {
                        serialCustomer++;

                        address.Add(new Address
                        {
                            Name = itemaddress.Name,
                            regionId = itemaddress.RegionId,
                            Phone = itemaddress.Phone,
                            BottleNo = itemaddress.BottleNo,
                            serial = serialCustomer,
                            //BottleType = itemaddress.BottleType.Name,

                            BottleType = itemaddress.BottleType.Name

                        });
                        //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

                    }
                    people.Add(new CustomersByGroup
                    {
                        Id = subitem.Id,
                        ShowDetails = false,
                        NameofRegion = subitem.Region.Name,// subitem.Name,
                        serial = serialRegion,

                        CustomerList = address.Where(x => x.regionId == subitem.RegionId).ToList(), //mycustomerList.Where(x => x.RegionId== subitem.Id).ToList(),
                        ItemCount = _pagedDataCustomers.Where(x => x.RegionId == subitem.RegionId).Count()
                    });

                }

            //}
            //else if (CurrentUserRool == RoleConstants.ManagerRole)
            //{


            //    foreach (var subitem in _regionList)
            //    {
            //        serialRegion++;
            //        serialCustomer = 0;
            //        foreach (var itemaddress in _pagedDataCustomers.Where(x => x.CompanyId == mycomp && x.RegionId == subitem.RegionId).ToList())
            //        {
            //            serialCustomer++;

            //            address.Add(new Address
            //            {
            //                Name = itemaddress.Name,
            //                regionId = itemaddress.RegionId,
            //                Phone = itemaddress.Phone,
            //                serial = serialCustomer,

            //                BottleNo = itemaddress.BottleNo,
            //                BottleType = itemaddress.BottleType.Name

            //                //BottleType= itemaddress.BottleType.Name

            //            });
            //            //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

            //        }
            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            ShowDetails = false,
            //            NameofRegion = subitem.Region.Name,// subitem.Name,
            //            serial = serialRegion,

            //            CustomerList = address.Where(x => x.regionId == subitem.RegionId).ToList(), //mycustomerList.Where(x => x.RegionId== subitem.Id).ToList(),
            //            ItemCount = _pagedDataCustomers.Where(x => x.CompanyId == mycomp && x.RegionId == subitem.RegionId).Count()

            //            //CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp).ToList(),
            //            // CustomerList = mycustomerList.Where(x => x.RegionId == subitem.Id ).ToList(),

            //        });

            //    }


            //}
            //else if (CurrentUserRool == RoleConstants.StationRole)
            //{

            //    foreach (var subitem in _regionList)
            //    {
            //        serialRegion++;
            //        serialCustomer = 0;
            //        foreach (var itemaddress in _pagedDataCustomers.Where(x => x.StationId == mystion && x.RegionId == subitem.RegionId).ToList())
            //        {
            //            serialCustomer++;

            //            address.Add(new Address
            //            {
            //                Name = itemaddress.Name,
            //                serial = serialCustomer,

            //                regionId = itemaddress.RegionId,
            //                Phone = itemaddress.Phone,
            //                BottleNo = itemaddress.BottleNo,
            //                BottleType = itemaddress.BottleType.Name

            //            });
            //            //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

            //        }
            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            ShowDetails = false,
            //            NameofRegion = subitem.Region.Name,// subitem.Name,
            //            serial = serialRegion,

            //            //CustomerList = subitem.CustomerList.Where(x => x.StationId==mystion).ToList(),
            //            CustomerList = address.Where(x => x.regionId == subitem.RegionId).ToList(), //mycustomerList.Where(x => x.RegionId== subitem.Id).ToList(),

            //            ItemCount = _pagedDataCustomers.Where(x => x.StationId == mystion && x.RegionId == subitem.RegionId).Count()
            //        });

            //    }
            //}
            //else if (CurrentUserRool == RoleConstants.DriverRole)
            //{

            //    foreach (var subitem in _regionList)
            //    {
            //        serialRegion++;
            //        serialCustomer = 0;
            //        foreach (var itemaddress in _pagedDataCustomers.Where(x => x.DriverId == mydriver && x.RegionId == subitem.RegionId).ToList())
            //        {
            //            serialCustomer++;

            //            address.Add(new Address
            //            {
            //                Name = itemaddress.Name,
            //                serial = serialCustomer,

            //                regionId = itemaddress.RegionId,
            //                Phone = itemaddress.Phone,
            //                BottleNo = itemaddress.BottleNo,
            //                BottleType = itemaddress.BottleType.Name

            //            });
            //            //_snackBar.Add("itemaddress" + itemaddress.Name, Severity.Success);

            //        }
            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            ShowDetails = false,
            //            NameofRegion = subitem.Region.Name,// subitem.Name,
            //            serial = serialRegion,

            //            //CustomerList = subitem.CustomerList.Where(x => x.StationId==mystion).ToList(),
            //            CustomerList = address.Where(x => x.regionId == subitem.RegionId).ToList(), //mycustomerList.Where(x => x.RegionId== subitem.Id).ToList(),

            //            ItemCount = _pagedDataCustomers.Where(x => x.DriverId == mydriver && x.RegionId == subitem.RegionId).Count()
            //        });

            //    }
            //}
            //else
            //{
            //    foreach (var subitem in _regionList)
            //    {
            //        serialRegion++;
            //        serialCustomer = 0;
            //        people.Add(new CustomersByGroup
            //        {
            //            Id = subitem.Id,
            //            ShowDetails = false,
            //            NameofRegion = subitem.Region.Name,// subitem.Name,
            //            serial = serialRegion,
            //            //CustomerList = subitem.CustomerList.Where(x => x.CompanyId == mycomp && x.StationId == mystion&&x.DriverId==mydriver).ToList(),
            //            //    CustomerList = mycustomerList.Where(x => x.RegionId == subitem.Id && x.DriverId == mydriver).ToList(),

            //            ItemCount = _pagedDataCustomers.Where(x => x.DriverId == mydriver && x.RegionId == subitem.RegionId).Count()
            //        });

            //    }

            //}




            People = people;
        }

        private void ShowBtnPress(int nr)
        {
            var tmpPerson = People.First(f => f.Id == nr);
            tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
        }


        private async Task Reset()
        {
            _region = new GetAllPagedDriverRegionsResponse();
            await GetRegionssAsync();
        }
    }
}