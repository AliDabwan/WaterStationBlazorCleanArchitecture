using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Company;
using WaterS.Client.Infrastructure.Managers.Catalog.Talap;
using WaterS.Client.Infrastructure.Managers.Catalog.Station;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Role;
using WaterS.Client.Infrastructure.Managers.Catalog.Customer;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone;
using WaterS.Client.Infrastructure.Managers.Catalog.AccountMovment;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Features.CustomerPhones.Queries.GetAll;
using WaterS.Application.Requests.Catalog;
using WaterS.Application.Interfaces.Services;
using System.Globalization;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditTalapModal
    {
        [Inject] private ITalapManager TalapManager { get; set; }
        [Inject] private IStationManager StationManager { get; set; }
        [Inject] private ICompanyManager CompanyManager { get; set; }
        [Inject] private ICustomerManager CustomerManager { get; set; }
        [Inject] private ICustomerPhoneManager CustomerPhoneManager { get; set; }


        [Inject] private IAccountMovmentManager AccountMovmentManager { get; set; }

        [Parameter] public AddEditTalapCommand AddEditTalapModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllAccountMovmentsResponse> _MovmentsData;
        private List<GetAllCustomerPhonesResponse> filtered;

        private List<GetAllCompaniesResponse> mycompanies = new ();
        private List<GetAllPagedCustomersResponse> myCustomers = new ();
        private List<GetAllPagedStationsResponse> mystations = new();
        private List<GetAllPagedTalapsResponse> mydrivers = new();
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


        private async Task Confirm()
        {
            //_snackBar.Add("1", Severity.Success);

            string OkContent = "";

            string Oktext = "";
            string OktextWithPrice = "";

            if (AddEditTalapModel.TalapStatue== "Undo")
            {
                Oktext = "الغاء الطلب";
                   OkContent = _localizer["هل انت متأكد ! سيتم الغاء الطلب"];

            }
            else if (AddEditTalapModel.TalapStatue == "Complete")
            {
                Oktext = "توصيل الطلب";

                if (AddEditTalapModel.Paid > 0)
                {
                    OktextWithPrice = "هل انت متأكد ! سيتم توصيل الطلب وتم قبض مبلغ : " + AddEditTalapModel.Paid.ToString("N0");
                    OkContent = OktextWithPrice;
                }
                else
                {
                    OkContent = _localizer["هل انت متأكد ! سيتم توصيل الطلب"];
                }
            }
            else if (AddEditTalapModel.TalapStatue == "Running")
            {
                Oktext = "إضافة الطلب";

                
                    OktextWithPrice = "هل انت متأكد ! سيتم إضافة طلب جديد للزبون : " ;
                    OkContent = OktextWithPrice;
              
            }

            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.OkConfirmation.ContentText),OkContent}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.OkConfirmation>(Oktext, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                //var myCustomer = await CustomerManager.GetCustomerAsync(AddEditTalapModel.CustomerId);
                //if (myCustomer.Succeeded)
                //{

                //    var mycustomerData = myCustomer.Data;
                //    AddEditCustomerModel

                //}
              



                  await SaveAsync(); 
              
            }
        }

        //private bool _loaded;
        AddEditCustomerCommand AddEditCustomerModel { get; set; } = new();
        private async Task SaveAsync()
        {
            //_snackBar.Add("1", Severity.Success);
            if (_processingSubmit)
            {
                return;
            }

            if (AddEditTalapModel.CustomerId == 0)
            {
                _snackBar.Add("يجب إختيار الزبون", Severity.Warning);
                return;
            }
            if (AddEditTalapModel.BottleNo == 0)
            {
                _snackBar.Add("يجب تحديد رقم الخزان", Severity.Warning);
                return;
            }
            _processingSubmit = true;


            var response = await TalapManager.SaveAsync(AddEditTalapModel);
            //await Task.Delay(2000);

            //_snackBar.Add(response.Data.ToString(), Severity.Success);

            if (response.Succeeded)
            {
                _processingSubmit = false;

                _snackBar.Add(response.Messages[0], Severity.Success);
                try
                {
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateHome);

                    //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                }
                catch
                {

                    MudDialog.Close();

                    return;
                }
             

                MudDialog.Close();
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

            //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
            //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateHome);

        }
        private int selectedItemMyCompany { get; set; }
        private int CheckSelected
        {
            get
            {
                return selectedItemMyCompany;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                selectedEventArgs.Value = value;
                OnChangeSelected(selectedEventArgs);
            }
        }
        private async void OnChangeSelected(ChangeEventArgs e)
        {
          

            if (e.Value.ToString() != string.Empty)
            {
                selectedItemMyCompany = (int)e.Value;
                mycomp = selectedItemMyCompany;
                mystion = 0;
                AddEditTalapModel.CompanyId = mycomp;
                await LoadStationssAsync();
            }
        }
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
        public string CreditAccountBalance { get; set; } = "0";
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal AccountBalance { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //_loaded = false;

          


            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
                //_snackBar.Add("CurrentUserId" + CurrentUserId, Severity.Error);

                var myUser = await _userManager.GetAsync(CurrentUserId);
                //CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;
                //_snackBar.Add("CurrentUserRool" + CurrentUserRool, Severity.Error);



                if (AddEditTalapModel.Id == 0)
                {
                    mystion = myUser.Data.StationId;
                    mydriver = myUser.Data.DriverId;

                    mycomp = myUser.Data.KindId;
                    AddEditTalapModel.TalapStatue = "Running";
                    AddEditTalapModel.TalapStatueAr = "جاري الطلب";

                    if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                    {

                    }
                    else if (CurrentUserRool == RoleConstants.ManagerRole)
                    {
                        AddEditTalapModel.CompanyId = myUser.Data.KindId;

                    }
                    else if (CurrentUserRool == RoleConstants.StationRole)
                    {
                        AddEditTalapModel.CompanyId = myUser.Data.KindId;
                        AddEditTalapModel.StationId = myUser.Data.StationId;

                    }
                    else if (CurrentUserRool == RoleConstants.DriverRole)
                    {
                        AddEditTalapModel.CompanyId = myUser.Data.KindId;
                        AddEditTalapModel.StationId = myUser.Data.StationId;
                        AddEditTalapModel.DriverId = myUser.Data.DriverId;

                    }
                    else
                    {

                        AddEditTalapModel.CompanyId = myUser.Data.KindId;
                        AddEditTalapModel.StationId = myUser.Data.StationId;
                        AddEditTalapModel.DriverId = myUser.Data.DriverId;
                        AddEditTalapModel.CustomerId = myUser.Data.CustomerId;

                    }

                }
                else
                {

                    mycomp = AddEditTalapModel.CompanyId;
                    mystion = AddEditTalapModel.StationId;
                    mydriver = AddEditTalapModel.DriverId;
                    mycustomer = AddEditTalapModel.CustomerId;

                    if (AddEditTalapModel.TalapStatue == "Complete")
                    {
                        //mudAmmountRef.SelectAsync();
                        AddEditTalapModel.Paid = AddEditTalapModel.Price;
                    }
                  

                    }


                //_snackBar.Add("mycustomer" + mycustomer, Severity.Error);





            }
            else
            {
                Cancel();
            }

            _loadedExp = true;

            await LoadDataAsync();


            //await LoadstationsAsync();
            try
            {
                //await HubConnection.StopAsync();

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
        private MudNumericField<decimal> mudPaidRef;

        public string mysearchedText { get; set; }
        public string myCustomerLabel { get; set; }
        public string myCustomerPhone{ get; set; }

        public string myStationLabel { get; set; }
        public string myDriverLabel { get; set; }
        public string myLastDateLabel { get; set; }
        private bool _processingSubmit = false;

        private bool isVisibleExp;
        private bool _loadedExp;

        public async void OpenOverlayExp()
        {
            isVisibleExp = true;
            await Task.Delay(100);
            isVisibleExp = false;
            _loadedExp = true;
            StateHasChanged();
        }
        public void ResetExampleExp()
        {
            _loadedExp = false;
        }
        private string _searchBy = "TBottleNo";

        [Inject] private IDriverRegionManager DriverRegionManager { get; set; }

        private async void OnSearch(string text)
        {
            ResetExampleExp();

            try
            {
                if (_searchBy == "TBottleNo" || _searchBy == "Name")
                {


                    var request = new GetAllPagedCustomersRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion,SearchString= text, SearchBy = _searchBy, Orderby = null };

                    var response = await CustomerManager.GetCustomersAsync(request);

                    //_snackBar.Add("TotalCount= :" + response.TotalCount);

                    if (response.Succeeded)
                    {



                        var mycustomer = response.Data.FirstOrDefault();

                        AddEditTalapModel.CustomerId = mycustomer.Id;
                        AddEditTalapModel.BottleNo = mycustomer.BottleNo;


                        if (CurrentUserRool != RoleConstants.DriverRole)
                        {
                            AddEditTalapModel.DriverId = mycustomer.DriverId;
                            if (CurrentUserRool != RoleConstants.StationRole)
                            {
                                AddEditTalapModel.StationId = mycustomer.StationId;

                            }
                        }

                        var requestR = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion,DriverId=mydriver, SearchString = "", RegionId = mycustomer.RegionId, Orderby = null };
                        var responseRegion = await DriverRegionManager.GetDriverRegionsAsync(requestR);

                        if (responseRegion.Succeeded)
                        {
                            
                            var myreg = responseRegion.Data.FirstOrDefault(x => x.RegionId == mycustomer.RegionId && x.StationId == mycustomer.StationId);
                            //if (mycustomer.DriverId!= myreg.DriverId)
                            //{
                            //    _snackBar.Add("! يرجى تصحيح بيانات الزبون : " + mycustomer.BottleNo, Severity.Error);
                            //    OpenOverlayExp();

                            //    AddEditTalapModel.CustomerId = 0;
                            //    AddEditTalapModel.BottleNo = 0;
                            //    myCustomerLabel = "";
                            //    myLastDateLabel = "";
                            //    mysearchedText = "";
                            //    CreditAccountBalance = "";
                            //    AccountBalance = 0;
                            //    _processing = false;
                            //    return;

                            //}
                            myDriverLabel = myreg.Driver.Name;
                            AddEditTalapModel.DriverId = myreg.DriverId;
                        }
                        else
                        {
                            OpenOverlayExp();

                            AddEditTalapModel.CustomerId = 0;
                            AddEditTalapModel.BottleNo = 0;
                            myCustomerLabel = "";
                            myLastDateLabel = "";
                            mysearchedText = "";
                            CreditAccountBalance = "";
                            AccountBalance = 0;
                            _processing = false;
                            return;
                        }

                        myCustomerLabel = mycustomer.Name;
                        myCustomerPhone = mycustomer.Phone;
                        myLastDateLabel = mycustomer.LastFillDate.ToString("yyyy-MM-dd");
                        //_snackBar.Add("AccountId :"+ mycustomer.AccountId);

                        var requestAcc = new GetAllPagedAccountMovmentsRequest { PageSize = 0, PageNumber = 0 + 1, AccountId = mycustomer.AccountId, SearchString = "", Orderby = null };

                        var responseBalance = await AccountMovmentManager.GetAccountMovmentsAsync(requestAcc);
                        if (responseBalance.Succeeded)
                        {
                            _MovmentsData = responseBalance.Data;


                            DebitAmount = _MovmentsData.Sum(x => x.DebitAmmount);
                            CreditAmount = _MovmentsData.Sum(x => x.CreditAmmount);
                            AccountBalance = (DebitAmount - CreditAmount);
                            CreditAccountBalance = (AccountBalance).ToString("N0");

                            _processing = false;
                            OpenOverlayExp();
                        }
                        else
                        {
                            OpenOverlayExp();

                            CreditAccountBalance = "";
                            AccountBalance = 0;
                            _processing = false;
                        }


                    }
                    else

                    {
                        OpenOverlayExp();

                        _snackBar.Add("لم يتم الوصول لاي نتيحة من خلال البحث عن : " + text, Severity.Error);
                        AddEditTalapModel.CustomerId = 0;
                        AddEditTalapModel.BottleNo = 0;
                        myCustomerLabel = "";
                        myLastDateLabel = "";
                        mysearchedText = "";
                        CreditAccountBalance = "";
                        AccountBalance = 0;
                        _processing = false;
                        //foreach (var message in response.Messages)
                        //{
                        //    _snackBar.Add(message, Severity.Error);
                        //}
                    }



                }


                else
                {
                if (text.Length<9)
                {
                    _snackBar.Add("رقم الهاتف هذا خطأ ! يرجى كتابة الرقم بشكل صحيح  ", Severity.Error);
                    OpenOverlayExp();

                    AddEditTalapModel.CustomerId = 0;
                    AddEditTalapModel.BottleNo = 0;
                    CreditAccountBalance = "";
                    myCustomerLabel = "";
                    myCustomerPhone = "";
                    myLastDateLabel = "";
                    mysearchedText = "";
                    AccountBalance = 0;
                    _processing = false;
                    return;
                }


                var myPhones = await CustomerPhoneManager.GetAllAsync();
                    if (myPhones.Succeeded)
                    {
                        //_snackBar.Add("mycustomer" + myPhones.Data.Count(), Severity.Error) ;

                        if (text.Length > 6)
                        {
                            filtered = myPhones.Data.Where(x =>  x.StationId == mystion && x.PhoneNumber == text || x.Customer.Name == text || x.Customer.Phone == text).ToList();


                        }



                    if (filtered.Any())
                    {
                        //_snackBar.Add("filtered" + filtered.Count(), Severity.Error);

                        var mycustomer = filtered.FirstOrDefault();
                        AddEditTalapModel.CustomerId = mycustomer.CustomerId;
                        AddEditTalapModel.BottleNo = mycustomer.Customer.BottleNo;


                        if (CurrentUserRool != RoleConstants.DriverRole)
                        {
                            AddEditTalapModel.DriverId = mycustomer.DriverId;
                            if (CurrentUserRool != RoleConstants.StationRole)
                            {
                                AddEditTalapModel.StationId = mycustomer.StationId;

                            }
                        }
                            var requestR = new GetAllPagedDriverRegionsRequest { PageSize = 0, PageNumber = 0 + 1, CompanyId = mycomp, StationId = mystion,DriverId=mydriver, SearchString = "", RegionId = mycustomer.Customer.RegionId, Orderby = null };
                            var responseRegion = await DriverRegionManager.GetDriverRegionsAsync(requestR);

                            if (responseRegion.Succeeded)
                            {
                                var myreg = responseRegion.Data.FirstOrDefault(x => x.RegionId == mycustomer.Customer.RegionId && x.StationId == mycustomer.StationId);

                                //if (mycustomer.DriverId != myreg.DriverId)
                                //{
                                //    _snackBar.Add("!# يرجى تصحيح بيانات الزبون : " + mycustomer.Customer.BottleNo, Severity.Error);
                                //    OpenOverlayExp();

                                //    AddEditTalapModel.CustomerId = 0;
                                //    AddEditTalapModel.BottleNo = 0;
                                //    myCustomerLabel = "";
                                //    myLastDateLabel = "";
                                //    mysearchedText = "";
                                //    CreditAccountBalance = "";
                                //    AccountBalance = 0;
                                //    _processing = false;
                                //    return;

                                //}
                                myDriverLabel = myreg.Driver.Name;
                                AddEditTalapModel.DriverId = myreg.DriverId;
                            }
                            else
                            {
                                OpenOverlayExp();

                                AddEditTalapModel.CustomerId = 0;
                                AddEditTalapModel.BottleNo = 0;
                                myCustomerLabel = "";
                                myLastDateLabel = "";
                                mysearchedText = "";
                                CreditAccountBalance = "";
                                AccountBalance = 0;
                                _processing = false;
                                return;
                            }

                            myCustomerLabel = mycustomer.Customer.Name;
                        myCustomerPhone = mycustomer.Customer.Phone;
                        myLastDateLabel = mycustomer.Customer.LastFillDate.ToString("yyyy-MM-dd");
                        //_snackBar.Add("AccountId :"+ mycustomer.AccountId);

                        var requestAcc = new GetAllPagedAccountMovmentsRequest { PageSize = 0, PageNumber = 0 + 1, AccountId = mycustomer.AccountId, SearchString = "", Orderby = null };

                        var response = await AccountMovmentManager.GetAccountMovmentsAsync(requestAcc);

                        if (response.Succeeded)
                        {



                            _MovmentsData = response.Data;


                            DebitAmount = _MovmentsData.Sum(x => x.DebitAmmount);
                            CreditAmount = _MovmentsData.Sum(x => x.CreditAmmount);
                            AccountBalance = (DebitAmount - CreditAmount);
                            CreditAccountBalance = (AccountBalance).ToString("N0");

                            _processing = false;
                            OpenOverlayExp();
                        }
                        else
                        {
                            OpenOverlayExp();

                            CreditAccountBalance = "";
                            AccountBalance = 0;
                            _processing = false;

                        }
                        //StateHasChanged();

                    }
                    else
                    {
                        OpenOverlayExp();

                        AddEditTalapModel.CustomerId = 0;
                        AddEditTalapModel.BottleNo = 0;
                        CreditAccountBalance = "";
                        myCustomerLabel = "";
                        myCustomerPhone = "";
                        myLastDateLabel = "";
                        mysearchedText = "";
                        AccountBalance = 0;
                        _snackBar.Add("لاتوجد بيانات مطابقة- ", Severity.Warning);
                        _processing = false;
                    }





                        //_snackBar.Add("filtered 1" + filtered.Count(), Severity.Error);

                    }
                    else
                    {
                        OpenOverlayExp();

                        _snackBar.Add("لم يتم العثور على  تطابق ", Severity.Error);
                        AddEditTalapModel.CustomerId = 0;
                        AddEditTalapModel.BottleNo = 0;
                        myCustomerLabel = "";
                        myLastDateLabel = "";
                        mysearchedText = "";
                        CreditAccountBalance = "";
                        AccountBalance = 0;
                        _processing = false;
                    }
                }



        }
                catch
            {
                OpenOverlayExp();

        AddEditTalapModel.CustomerId = 0;
                AddEditTalapModel.BottleNo = 0;
                myCustomerLabel = "";
                myLastDateLabel = "";
                mysearchedText = "";
                myCustomerPhone = "";

                CreditAccountBalance = "";
                AccountBalance = 0;
                _processing = false;
                _snackBar.Add("لاتوجد بيانات مطابقة# ", Severity.Normal);
                return;
            }


}
        private bool _processing = false;

        async Task ProcessSomething()
        {
            if (string.IsNullOrEmpty(mysearchedText))
            {
                _snackBar.Add("يجب إدخال رقم الخزان او جوال الزبون  ", Severity.Error);

                return;
            }
            _processing = true;
            
            //await Task.Delay(3000);
            OnSearch(mysearchedText);
            //_processing = false;
        }
        private async Task LoadcompaniesAsync()
        {
            var mydata = await CompanyManager.GetAllAsync();
            if (mydata.Succeeded)
            {

                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    mycompanies = mydata.Data;


                }
                else

                {
                    mycompanies = mydata.Data.Where(x => x.Id == mycomp).ToList();

                }
            }
        }
        //private async Task LoadCustomers()
        //{
        //    var mydata = await CustomerManager.GetCustomersAsync();
        //    if (mydata.Succeeded)
        //    {

        //        if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
        //        {
        //            myCustomers = mydata.Data;

        //        }
        //        else if (CurrentUserRool == RoleConstants.ManagerRole )
        //        {
        //            myCustomers = mydata.Data.Where(x => x.CompanyId == mycomp).ToList();

        //        }
        //        else if (CurrentUserRool == RoleConstants.StationRole)
        //        {
        //            myCustomers = mydata.Data.Where(x => x.CompanyId == mycomp && x.StationId==mystion).ToList();

        //        }
        //        else if (CurrentUserRool == RoleConstants.DriverRole)
        //        {
        //            myCustomers = mydata.Data.Where(x => x.StationId == mystion && x.DriverId == mydriver).ToList();

        //        }

        //    }
        //}
        private async Task LoadDataAsync()
        {
            //_loaded = true;
            //await LoadCustomers();
            await Task.CompletedTask;
        }
    }
}