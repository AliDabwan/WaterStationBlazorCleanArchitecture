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
    public partial class Talaps
    {
        [Inject] private ITalapManager TalapManager { get; set; }

        //[CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllPagedTalapsResponse> _pagedData;
        private MudTable<GetAllPagedTalapsResponse> _table;
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

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
        private string StationRool { get; set; } = "Station";
        private string CompanyRool { get; set; } = "Manager";
        private string DriverRoll { get; set; } = "Driver";

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();

            _canCreateTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Create)).Succeeded;
            _canEditTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Edit)).Succeeded;
            _canDeleteTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Delete)).Succeeded;
            _canExportTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Export)).Succeeded;
            _canSearchTalaps = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Talaps.Search)).Succeeded;


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
            _loaded = true;



            //HubConnection = HubConnection.TryInitialize(_navigationManager);
            //if (HubConnection.State == HubConnectionState.Disconnected)
            //{
            //    await HubConnection.StartAsync();
            //}



        }





        public string talapStatue { get; set; } = "all";
        public string talapdate { get; set; } = "Today";
        public DateTime talapdateTo { get; set; } 



        private async Task<TableData<GetAllPagedTalapsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            StateHasChanged();

            return new TableData<GetAllPagedTalapsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            if (string.IsNullOrEmpty(state.SortLabel))
            {
                state.SortLabel = "Id";
                state.SortDirection = SortDirection.Descending;
            }
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }


            string dateeTo = "";
            string dateeFrom = "";

            if (talapdate == "Today")
            {
                dateeFrom = DateTime.Now.Date.ToString("yyyy-MM-dd");
                dateeTo = DateTime.Now.Date.ToString("yyyy-MM-dd");

            }
            else if (talapdate == "Week")
            {

                dateeFrom = DateTime.Now.Date.AddDays(-7).ToString("yyyy-MM-dd");
                dateeTo = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
            else if (talapdate == "Month")
            {

                dateeFrom = DateTime.Now.Date.AddMonths(-1).ToString("yyyy-MM-dd");
                dateeTo = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
          
            else
            {

                dateeFrom = "";
                dateeTo = "";

            }



            var request = new GetAllPagedTalapsRequest { PageSize = pageSize, PageNumber = pageNumber + 1,CompanyId=mycomp,StationId=mystion,DriverId=mydriver,CustomerId=mycustomer,
                
                DateFrom = dateeFrom,
                DateTo = dateeTo,
                Statue = talapStatue, SearchString = _searchString, Orderby = orderings };
           
            
            
            var response = await TalapManager.GetTalapsAsync(request);
            //var response2 = await TalapManager.GetTalapsAsync();


            if (response.Succeeded)
            {
                _currentPage = response.CurrentPage;
                _totalItems = response.TotalCount;

                //_pagedData = response.Data;
                //_snackBar.Add(_pagedData.Count().ToString(), Severity.Error);

                //if (CurrentUserRool == RoleConstants.AdministratorRole || CurrentUserRool == RoleConstants.AdminRole)
                //{



                //if (talapdate== "Today")
                //{
                //    _pagedData = response.Data.Where(x => (x.CreatedOn.Date >= DateTime.Now.Date && x.CreatedOn.Date <= DateTime.Now.Date));

                //}
                //else if (talapdate == "Week")
                //{
                //    _pagedData = response.Data.Where(x => (x.CreatedOn.Date >= DateTime.Now.Date.AddDays(-7) && x.CreatedOn.Date <= DateTime.Now.Date));

                //}
                //else if (talapdate == "Month")
                //{
                //    _pagedData = response.Data.Where(x => (x.CreatedOn.Date >= DateTime.Now.Date.AddMonths(-1) && x.CreatedOn.Date <= DateTime.Now.Date));

                //}
                //else
                //{
                    _pagedData = response.Data;

                //}
                //_totalItems = _pagedData.Count();
                int serial = 0;

                foreach (var item in _pagedData)
                {
                    serial++;
                    item.serial = serial;
                }

                //}
                //else if(CurrentUserRool == RoleConstants.ManagerRole)
                //{

                //    _pagedData = response.Data.Where(x => x.CompanyId == mycomp).ToList();
                //    _totalItems = _pagedData.Count();

                //}
                //else if (CurrentUserRool == RoleConstants.StationRole)
                //{

                //    _pagedData = response.Data.Where(x => x.CompanyId == mycomp && x.StationId == mystion).ToList();
                //    _totalItems = _pagedData.Count();

                //}
                //else if (CurrentUserRool == RoleConstants.DriverRole)
                //{

                //    _pagedData = response.Data.Where(x => x.CompanyId == mycomp && x.StationId == mystion && x.DriverId == mydriver).ToList();
                //    _totalItems = _pagedData.Count();

                //}
                //else
                //{
                //    //_snackBar.Add("mycomp="+ mycomp+ " - mystion=" + mystion+ " - DriverId=" + mydriver + " - mystion=" + mycustomer, Severity.Error);
                //    _pagedData = response.Data.Where(x =>  x.CustomerId == mycustomer).ToList();
                //    _totalItems = _pagedData.Count();

                //    //_pagedData = response.Data.Where(x => x.CompanyId == mycomp && x.StationId == mystion && x.DriverId == mydriver && x.CustomerId == mycustomer).ToList();

                //    //_snackBar.Add(_pagedData.Count().ToString(), Severity.Error);

                //}




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
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportToExcel()
        {
            //var response = await TalapManager.ExportToExcelAsync(_searchString);
            if (_pagedData!=null)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = _pagedData,//response.Data,
                    FileName = $"{nameof(Talaps).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Talaps exported"]
                    : _localizer["Filtered Talaps exported"], Severity.Success);
            }
            else
            {
               
                    _snackBar.Add("لايوجد سجلات للتصدير", Severity.Error);
                
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
                //_snackBar.Add("close");

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
                        Price = Talap.Price,
                        DoneByAccountId = myAccountId,
                        DoneByName = CurrentUserId,
                        No = Talap.No,
                        ServiceRate = Talap.ServiceRate,
                        TalapStatue = "Complete",
                        TalapStatueAr = "مكتمل",


                        CustomerId = Talap.CustomerId,
                        DriverId = Talap.DriverId,
                        CompanyId = Talap.CompanyId,
                        StationId = Talap.StationId,
                        RegionId = Talap.RegionId,

                    }) ;
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
                        DoneByName = CurrentUserId,
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
                    //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
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