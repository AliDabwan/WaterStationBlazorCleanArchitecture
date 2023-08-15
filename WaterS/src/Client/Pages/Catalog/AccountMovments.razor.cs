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
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.AccountMovment;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class AccountMovments
    {
        [Inject] private IAccountMovmentManager AccountMovmentManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllAccountMovmentsResponse> _brandList = new();
        private GetAllAccountMovmentsResponse _brand = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

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
        private int mycustomer { get; set; } = 0;
        private int myAccountId { get; set; } = 0;

        private string CustomerRoll { get; set; } = "Customer";
        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AccountName.Create)).Succeeded;
            _canEditBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AccountName.Edit)).Succeeded;
            _canDeleteBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AccountName.Delete)).Succeeded;
            _canExportBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AccountName.Export)).Succeeded;
            _canSearchBrands = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AccountName.Search)).Succeeded;




            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();

                var myUser = await _userManager.GetAsync(CurrentUserId);
                //CurrentUserId = user.GetUserId();
                CurrentUserRool = myUser.Data.KindType;
                mycomp = myUser.Data.KindId;
                mystion = myUser.Data.StationId;
                mydriver = myUser.Data.DriverId;
                mycustomer = myUser.Data.CustomerId;
                myAccountId = myUser.Data.AccountId;
            }




            await GetBottleTypessAsync();
            _loaded = true;

            //try
            //{
                HubConnection = HubConnection.TryInitialize(_navigationManager);
                if (HubConnection.State == HubConnectionState.Disconnected)
                {
                    await HubConnection.StartAsync();
                }
            //}
            //catch 
            //{

            //    return;
            //}
         
        }
        private List<GetAllAccountMovmentsResponse> _MovmentsData;

        public string CreditAccountBalance { get; set; } = "0";
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal AccountBalance { get; set; }
        private async Task GetBottleTypessAsync()
        {
            var requestAcc = new GetAllPagedAccountMovmentsRequest { PageSize = 10000000, PageNumber = 0 + 1, AccountId = myAccountId, SearchString = "", Orderby = null };

            var response = await AccountMovmentManager.GetAccountMovmentsAsync(requestAcc);
            if (response.Succeeded)
            {

               

                    _brandList = response.Data;



                //var transMovmnt = _brandList;// await AccountMovmentManager.GetAllAsync();
                //_MovmentsData = transMovmnt.Data.Where(x => x.AccountsId == myAccountId).ToList();
                _MovmentsData = _brandList;

                DebitAmount = _MovmentsData.Sum(x => x.DebitAmmount);
                CreditAmount = _MovmentsData.Sum(x => x.CreditAmmount);
                AccountBalance = (DebitAmount - CreditAmount);
                CreditAccountBalance = (AccountBalance).ToString("N0");


                StateHasChanged();




            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await AccountMovmentManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task ExportToExcel()
        {
            var requestAcc = new GetAllPagedAccountMovmentsRequest { PageSize = 10000000, PageNumber = 0 + 1, AccountId = myAccountId, SearchString = _searchString, Orderby = null };

            var response = await AccountMovmentManager.GetAccountMovmentsAsync(requestAcc);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(AccountMovments).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
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

        //private async Task InvokeModal(int id = 0)
        //{
        //    var parameters = new DialogParameters();
        //    if (id != 0)
        //    {
        //        _brand = _brandList.FirstOrDefault(c => c.Id == id);
        //        if (_brand != null)
        //        {
        //            parameters.Add(nameof(AddEditBottleTypeModal.AddEditBottleTypeModel), new AddEditBottleTypeCommand
        //            {
        //                Id = _brand.Id,
        //                Name = _brand.Name,
        //                Description = _brand.Description
        //            });
        //        }
        //    }
        //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        //    var dialog = _dialogService.Show<AddEditBottleTypeModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        //    var result = await dialog.Result;
        //    if (!result.Cancelled)
        //    {
        //        await Reset();
        //    }
        //}

        private async Task Reset()
        {
            _brand = new GetAllAccountMovmentsResponse();
            await GetBottleTypessAsync();
        }

        private bool Search(GetAllAccountMovmentsResponse brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.Note?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
          
            return false;
        }
    }
}