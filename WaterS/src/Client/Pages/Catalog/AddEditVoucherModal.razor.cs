using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;

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
using WaterS.Client.Infrastructure.Managers.Catalog.AccountMovment;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Features.AccountNames.Queries.GetAll;
using WaterS.Client.Infrastructure.Managers.Catalog.AccountName;
using WaterS.Application.Requests.Catalog;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AddEditVoucherModal
    {
        [Inject] private IAccountMovmentManager AccountMovmentManager { get; set; }
        [Inject] private IAccountNameManager AccountNameManager { get; set; }

        [Inject] private ITalapManager TalapManager { get; set; }
        [Inject] private IStationManager StationManager { get; set; }
        [Inject] private ICompanyManager CompanyManager { get; set; }
        [Inject] private ICustomerManager CustomerManager { get; set; }
        [Parameter] public AddEditAccountMovmentCommand AddEditVoucherModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }
            
        
        private List<GetAllCompaniesResponse> mycompanies = new ();
        private List<GetAllPagedCustomersResponse> myCustomers = new ();
        private List<GetAllAccountNamesResponse> myAccounts = new();

        private List<GetAllPagedStationsResponse> mystations = new();
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
        private int myAccountid { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    
        public void Cancel()
        {
            MudDialog.Cancel();
        }



        private async Task Confirm()
        {

            if (AddEditVoucherModel.creditAccountId == 0)
            {
                _snackBar.Add("يجب إختيار الحساب", Severity.Warning);
                return;
            }
            if (AddEditVoucherModel.Ammount <= 0)
            {
                _snackBar.Add("لايمكن ان يكون المبلغ اقل من او يساوي صفر", Severity.Warning);
                return;
            }


            AddEditVoucherModel.DebitAmmount = AddEditVoucherModel.Ammount;
            AddEditVoucherModel.CreditAmmount = AddEditVoucherModel.Ammount;

            string OkContent = "";

            string Oktext = "";
           
                Oktext = "تأكيد القبض";

                OkContent = _localizer["هل انت متأكد ! سيتم قبض مبلغ"+ AddEditVoucherModel.Ammount.ToString("N0")];

            


            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.OkConfirmation.ContentText),OkContent}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.OkConfirmation>(_localizer[Oktext], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                 await SaveAsync(); 
              
            }
        }


        private async Task SaveAsync()
        {

            var textNoteDebit = "";
            var textNoteCerdit = "";

            if (CurrentUserRool == RoleConstants.ManagerRole)
            {

                textNoteDebit = "قبض من المحطة : ";
                textNoteCerdit = "دفع الى شركة : "+ myaccName;
            }
            else if (CurrentUserRool == RoleConstants.StationRole)
            {
                textNoteDebit = "قبض من : ";
                textNoteCerdit = "دفع الى : " + myaccName;
            }
            else if (CurrentUserRool == RoleConstants.DriverRole)
            {
                textNoteDebit = "قبض من الزبون : ";
                textNoteCerdit = "دفع الى : " + myaccName;
            }
            AddEditVoucherModel.NoteDebit = textNoteDebit;
            AddEditVoucherModel.NoteCredit = textNoteCerdit;

            var response = await AccountMovmentManager.SaveAsync(AddEditVoucherModel);


            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                try
                {
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateHome);

                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                }
                catch 
                {
                    //MudDialog.Close();

                    return;
                }
             
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


        private int selectedCreditAccountId { get; set; }
        private int  CheckselectedCreditAccountId
        {
            get
            {
                return selectedCreditAccountId;
            }
            set
            {
                ChangeEventArgs selectedEventArgs = new ChangeEventArgs();
                selectedEventArgs.Value = value;
                OnChangeSelected(selectedEventArgs);
            }
        }
        private List<GetAllAccountMovmentsResponse> _MovmentsData;

        public string CreditAccountBalance { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }

        private async void OnChangeSelected(ChangeEventArgs e)
        {


            if (e.Value.ToString() != string.Empty)
            {
                selectedCreditAccountId = (int)e.Value;
                AddEditVoucherModel.creditAccountId = selectedCreditAccountId;
                //var transMovmnt = await AccountMovmentManager.GetAllAsync();
                //_MovmentsData = transMovmnt.Data.Where(x => x.AccountsId == selectedCreditAccountId).ToList();

                var requestAcc = new GetAllPagedAccountMovmentsRequest { PageSize = 10000000, PageNumber = 0 + 1, AccountId = selectedCreditAccountId, SearchString = "", Orderby = null };

                var response = await AccountMovmentManager.GetAccountMovmentsAsync(requestAcc);
                _MovmentsData = response.Data;

                DebitAmount = _MovmentsData.Sum(x=>x.DebitAmmount);
                CreditAmount = _MovmentsData.Sum(x => x.CreditAmmount);
                CreditAccountBalance = (DebitAmount - CreditAmount).ToString("N0");

                //_snackBar.Add("CreditAccountBalance" + CreditAccountBalance, Severity.Error);
                StateHasChanged();
                mudAmmountRef.SelectAsync();
            }
        }
        private MudNumericField<decimal> mudAmmountRef;
        private string myaccName { get; set; }
        protected override async Task OnInitializedAsync()
        {

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

                AddEditVoucherModel.debitAccountId = myUser.Data.AccountId;
                AddEditVoucherModel.userId = CurrentUserId;


                if (AddEditVoucherModel.Id == 0)
                {
                    mystion = myUser.Data.StationId;
                    mycomp = myUser.Data.KindId;
                    mystion = myUser.Data.StationId;
                    mydriver = myUser.Data.DriverId;
                    myAccountid = myUser.Data.AccountId;
                    AddEditVoucherModel.debitAccountId = myAccountid;
                    myaccName = myUser.Data.FirstName;

                    AddEditVoucherModel.refId = 1;
                    AddEditVoucherModel.CreditAmmount = 0;
                    AddEditVoucherModel.DebitAmmount = 0;

                    if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                    {

                    }
                    else if (CurrentUserRool == RoleConstants.ManagerRole)
                    {
                        AddEditVoucherModel.CompanyId = myUser.Data.KindId;

                    }
                    else if (CurrentUserRool == RoleConstants.StationRole)
                    {
                        AddEditVoucherModel.CompanyId = myUser.Data.KindId;
                        AddEditVoucherModel.StationId = myUser.Data.StationId;

                    }
                    else if (CurrentUserRool == RoleConstants.DriverRole)
                    {
                        AddEditVoucherModel.CompanyId = myUser.Data.KindId;
                        AddEditVoucherModel.StationId = myUser.Data.StationId;

                    }
                    else
                    {

                        AddEditVoucherModel.CompanyId = myUser.Data.KindId;
                        AddEditVoucherModel.StationId = myUser.Data.StationId;
                      

                    }
                    mudAmmountRef.SelectAsync();
                }
                else
                {

                    mycomp = AddEditVoucherModel.CompanyId;
                    mystion = AddEditVoucherModel.StationId;
                    selectedCreditAccountId = AddEditVoucherModel.creditAccountId;
                    myAccountid = AddEditVoucherModel.debitAccountId;
                }


                //_snackBar.Add("mycustomer" + mycustomer, Severity.Error);





            }
            else
            {
                Cancel();
            }
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
                //_snackBar.Add("no prop" + mycustomer, Severity.Success);

            }
            catch (Exception ex)
            {
                //_snackBar.Add("prop proppppppppppppppppppppppppppppppp"+ ex.Message , Severity.Error);

                return;
            }
          
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
        private async Task LoadCustomers()
        {
            var mydata = await CustomerManager.GetCustomersAsync();
            if (mydata.Succeeded)
            {

                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    myCustomers = mydata.Data;

                }
                else if (CurrentUserRool == RoleConstants.ManagerRole )
                {
                    myCustomers = mydata.Data.Where(x => x.CompanyId == mycomp).ToList();

                }
                else if (CurrentUserRool == RoleConstants.StationRole)
                {
                    myCustomers = mydata.Data.Where(x => x.CompanyId == mycomp&&x.StationId==mystion).ToList();

                }
                else if (CurrentUserRool == RoleConstants.DriverRole)
                {
                    myCustomers = mydata.Data.Where(x => x.DriverId == mydriver ).ToList();

                }

            }
        }

        private async Task LoadAccounts()
        {
            var mydata = await AccountNameManager.GetAllAsync();
            //_snackBar.Add("  :"+mydata.Data.Count(), Severity.Error);

            if (mydata.Succeeded)
            {

                if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                {
                    myAccounts = mydata.Data;

                }
                else if (CurrentUserRool == RoleConstants.ManagerRole)
                {
                    myAccounts = mydata.Data.Where(x => x.CompanyId == mycomp && x.CategoryType == "محطة").ToList();

                }
                else if (CurrentUserRool == RoleConstants.StationRole)
                {
                    myAccounts = mydata.Data.Where(x => x.StationId == mystion && x.CategoryType == "سائق").ToList();

                }
                else if (CurrentUserRool == RoleConstants.DriverRole)
                {
                    myAccounts = mydata.Data.Where(x => x.DriverId == mydriver && x.CategoryType== "زبون").ToList();

                }

            }
        }
       
        private async Task LoadDataAsync()
        {
            await LoadAccounts();


            await Task.CompletedTask;
        }
    }
}