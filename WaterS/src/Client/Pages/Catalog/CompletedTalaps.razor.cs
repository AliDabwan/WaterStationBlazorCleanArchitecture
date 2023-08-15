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
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Talap;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;

namespace WaterS.Client.Pages.Catalog
{
    public partial class CompletedTalaps
    {
        [Inject] private ITalapManager TalapManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedTalapsResponse> _pagedData;
        private MudTable<GetAllPagedTalapsResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;
        private MudDateRangePicker _dateRangePicker;
        private DateRange _dateRange;
        private ClaimsPrincipal _currentUser;
        private bool _canCreateTalaps;
        private bool _canEditTalaps;
        private bool _canDeleteTalaps;
        private bool _canExportTalaps;
        private bool _canSearchTalaps;
        private bool _loaded;
        private string CurrentUserId { get; set; }
        private string CurrentUserRool { get; set; } = "x";
        private int mycomp { get; set; } = 0;
        private int mystion { get; set; } = 0;
        private int mydriver{ get; set; } = 0;
        private int mycustomer{ get; set; } = 0;
        private int myAccountId { get; set; } = 0;

        private string CustomerRoll { get; set; } = "Customer";
        private string DriverRoll { get; set; } = "Driver";

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();

            _canCreateTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Create)).Succeeded;
            _canEditTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Edit)).Succeeded;
            _canDeleteTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Delete)).Succeeded;
            _canExportTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Export)).Succeeded;
            _canSearchTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Search)).Succeeded;

            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }



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
           

        }






        public string talapdate { get; set; } = "Today";


        private async Task<TableData<GetAllPagedTalapsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllPagedTalapsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            //_snackBar.Add("CurrentUserRool= :" + CurrentUserRool);

            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            string dateeTo = "";
            string dateeFrom = "";


            if (talapdate == "Today")
            {
                BYdate = false;
                dateeFrom = DateTime.Now.Date.ToString("yyyy-MM-dd");
                dateeTo = DateTime.Now.Date.ToString("yyyy-MM-dd");

            }
            else if (talapdate == "Week")
            {
                BYdate = false;

                dateeFrom = DateTime.Now.Date.AddDays(-7).ToString("yyyy-MM-dd");
                dateeTo = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
            else if (talapdate == "Month")
            {
                BYdate = false;

                dateeFrom = DateTime.Now.Date.AddMonths(-1).ToString("yyyy-MM-dd");
                dateeTo = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
            else if (talapdate == "Two")
            {
                BYdate = true;
                dateeFrom = _dateRange.Start.Value.Date.ToString("yyyy-MM-dd");
                dateeTo = _dateRange.End.Value.Date.ToString("yyyy-MM-dd");

            }
            else
            {
                BYdate = false;

                dateeFrom = "";
                dateeTo = "";

            }

            var request = new GetAllPagedTalapsRequest { PageSize = pageSize, PageNumber = pageNumber + 1, CompanyId = mycomp, StationId = mystion, DriverId = mydriver, CustomerId = mycustomer, DateFrom = dateeFrom, DateTo = dateeTo, Statue = "Complete", SearchString = _searchString, Orderby = orderings };
            var response = await TalapManager.GetTalapsAsync(request);
        
            //_snackBar.Add("TotalCount= :" + response.TotalCount);

            if (response.Succeeded)
            {
                //_totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                _totalItems = response.TotalCount;

                _pagedData = response.Data;

                //if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                //{
                //var mm = response.Data;

                //_snackBar.Add("CurrentUserRool= :" + CurrentUserRool);

                //if (talapdate == "Today")
                //{
                //    BYdate = false;

                //    _pagedData = response.Data.Where(x => x.TalapStatue == "Complete" && (x.CreatedOn.Date >= DateTime.Now.Date && x.CreatedOn.Date <= DateTime.Now.Date));

                //}
                //else if (talapdate == "Week")
                //{
                //    BYdate = false;

                //    _pagedData = response.Data.Where(x => x.TalapStatue == "Complete" && (x.CreatedOn.Date >= DateTime.Now.Date.AddDays(-7) && x.CreatedOn.Date <= DateTime.Now.Date));

                //}
                //else if (talapdate == "Month")
                //{
                //    BYdate = false;

                //    _pagedData = response.Data.Where(x => x.TalapStatue == "Complete" && (x.CreatedOn.Date >= DateTime.Now.Date.AddMonths(-1) && x.CreatedOn.Date <= DateTime.Now.Date));

                //}
                //else if (talapdate == "Two")
                //{
                //    _pagedData = response.Data.Where(x => x.TalapStatue == "Complete" && (x.CreatedOn.Date >= _dateRange.Start.Value.Date && x.CreatedOn.Date <= _dateRange.End.Value.Date));

                //}
                //else
                //{
                //    BYdate = false;

                //    _pagedData = mm.Where(x => x.TalapStatue == "Complete").ToList();

                //}

                int serial = 0;

                foreach (var item in _pagedData)
                {
                    serial++;
                    item.serial = serial;
                }


                //if (_pagedData.Any())
                //{
                //    FillPeople();

                //}


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
            //if (!BYdate)
            //{
            //    talapdate = "all";

            //}
            _searchString = text;
            _table.ReloadServerData();
            StateHasChanged();
        }
        public bool BYdate { get; set; } = false;

        private void OnUndo()
        {

            _dateRangePicker.Close(false);
            talapdate = "all";
            StateHasChanged();

        }
        private void Onclose()
        {
            if (_dateRange?.Start.HasValue == false || _dateRange?.End.HasValue == false)
            {
                talapdate = "all";

            }
            else
            {

                talapdate = "Two";

            }
            _dateRangePicker.Close();
            OnSearch("");
        }

        private async Task ExportToExcel()
        {
            var response = await TalapManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Drivers).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Talaps exported"]
                    : _localizer["Filtered Drivers exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var Talap = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Talap != null)
                {
                    parameters.Add(nameof(AddEditTalapModal.AddEditTalapModel), new AddEditTalapCommand
                    {
                        Id = Talap.Id,
                 Comment = Talap.Comment,
                 BottleNo = Talap.BottleNo,
                 TalapDate = Talap.TalapDate,
                 
                 
                        No = Talap.No,
                        ServiceRate = Talap.ServiceRate,
                        TalapStatue = Talap.TalapStatue,
                        TalapStatueAr = Talap.TalapStatueAr,
                      

                        CustomerId = Talap.CustomerId,
                        DriverId = Talap.DriverId,
                        CompanyId = Talap.CompanyId,
                        StationId = Talap.StationId,
                        RegionId = Talap.RegionId,
                       
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditTalapModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }


        private async Task InvokeModalAccept(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var Talap = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Talap != null)
                {
                    parameters.Add(nameof(AddEditTalapModal.AddEditTalapModel), new AddEditTalapCommand
                    {
                        Id = Talap.Id,
                        Comment = Talap.Comment,
                        BottleNo = Talap.BottleNo,
                        TalapDate = Talap.TalapDate,
                        Price= Talap.Price,
                        DoneByAccountId=myAccountId,

                        No = Talap.No,
                        ServiceRate = Talap.ServiceRate,
                        TalapStatue = "Complete",
                        TalapStatueAr = "مكتمل",


                        CustomerId = Talap.CustomerId,
                        DriverId = Talap.DriverId,
                        CompanyId = Talap.CompanyId,
                        StationId = Talap.StationId,
                        RegionId = Talap.RegionId,

                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditTalapModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }

        private async Task InvokeModalUndo(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var Talap = _pagedData.FirstOrDefault(c => c.Id == id);
                if (Talap != null)
                {
                    parameters.Add(nameof(AddEditTalapModal.AddEditTalapModel), new AddEditTalapCommand
                    {
                        Id = Talap.Id,
                        Comment = Talap.Comment,
                        BottleNo = Talap.BottleNo,
                        TalapDate = Talap.TalapDate,
                        Price = 0,
                        Paid = 0,
                        DoneByAccountId = myAccountId,

                        No = Talap.No,
                        ServiceRate = Talap.ServiceRate,
                        TalapStatue = "Undo",
                        TalapStatueAr = "ملغي",


                        CustomerId = Talap.CustomerId,
                        DriverId = Talap.DriverId,
                        CompanyId = Talap.CompanyId,
                        StationId = Talap.StationId,
                        RegionId = Talap.RegionId,

                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditTalapModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
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
                var response = await TalapManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}