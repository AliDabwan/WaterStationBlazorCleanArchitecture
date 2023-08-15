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
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Catalog.Talap;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;
using WaterS.Shared.Constants.Role;
//using WaterS.Client.Shared.Components;
namespace WaterS.Client.Pages.Catalog
{
    public partial class TalapsAllReport
    {
        [Inject] private ITalapManager TalapManager { get; set; }
        //private IExcelService _excelService;

        
        //[CascadingParameter] private HubConnection HubConnection { get; set; }
        private MudDateRangePicker _dateRangePicker;
        private DateRange _dateRange;

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


        public bool BYdate { get; set; } = false;
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
               

            }
        }







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



        //private bool Search(AuditResponse response)
        //{
        //    var result = false;

        //    // check Search String
        //    if (string.IsNullOrWhiteSpace(_searchString)) result = true;
        //    if (!result)
        //    {
        //        if (response.TableName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        //        {
        //            result = true;
        //        }
        //        if (_searchInOldValues &&
        //            response.OldValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        //        {
        //            result = true;
        //        }
        //        if (_searchInNewValues &&
        //            response.NewValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        //        {
        //            result = true;
        //        }
        //    }

        //    // check Date Range
        //    if (_dateRange?.Start == null && _dateRange?.End == null) return result;
        //    if (_dateRange?.Start != null && response.DateTime < _dateRange.Start)
        //    {
        //        result = false;
        //    }
        //    if (_dateRange?.End != null && response.DateTime > _dateRange.End + new TimeSpan(0, 11, 59, 59, 999))
        //    {
        //        result = false;
        //    }

        //    return result;
        //}


        public string talapStatue { get; set; } = "all";


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

            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

          
            var request = BYdate?


                new GetAllPagedTalapsRequest { PageSize = pageSize, PageNumber = pageNumber + 1,CompanyId=mycomp,StationId=mystion,DriverId=mydriver,CustomerId=mycustomer
                ,Statue= talapStatue,
              DateFrom=  _dateRange?.Start.Value.ToString("yyyy-MM-dd"),
              DateTo =_dateRange?.End.Value.ToString("yyyy-MM-dd")
                , SearchString = _searchString, Orderby = orderings }
                :
                 new GetAllPagedTalapsRequest
                 {
                     PageSize = pageSize,
                     PageNumber = pageNumber + 1,
                     CompanyId = mycomp,
                     StationId = mystion,
                     DriverId = mydriver,
                     CustomerId = mycustomer,
                     Statue = talapStatue,
                 
                
                     SearchString = _searchString,
                     Orderby = orderings
                 }
                ;
           
            
            
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

                _pagedData = response.Data;



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
        private void Reload()
        {
            _table.ReloadServerData();
        }
        private void Onclose()
        {
            BYdate = true;
            _dateRangePicker.Close();
            OnSearch("");
        }

        private void DoReport()
        {
            var searchValue = string.IsNullOrEmpty(_searchString) ? "null" : _searchString;
            if (_pagedData.Count()<=0)
            {
                _snackBar.Add("لاتوجد بيانات لعرضها");
                return;
            }
            if (BYdate )
            {
                _navigationManager.NavigateTo($"/talapallreport/{_dateRange?.Start.Value.ToString("yyyy-MM-dd")}/" +
                    $"{_dateRange?.End.Value.ToString("yyyy-MM-dd")}/{talapStatue}/{searchValue}");

            }
            else
            {
                _navigationManager.NavigateTo($"/talapallreport/{talapStatue}/{searchValue}");

            }
        }
        private async Task ExportToExcel()
        {
            var response = await TalapManager.ExportToExcelAsync(_searchString);
            if (_pagedData!=null)
            {


            //    var data = await _excelService.ExportAsync(_pagedData, mappers: new Dictionary<string, Func<GetAllPagedTalapsResponse, object>>
            //{
            //    { _localizer["Id"], item => item.Id },
            //    { _localizer["Company Name"], item => item.Company.Name },
            //    { _localizer["Station Name"], item => item.Station.Name },
            //    { _localizer["Driver Name"], item => item.Driver.Name },
            //    { _localizer["Region Name"], item => item.Region.Name },
            //                    { _localizer["الزبون"], item => item.CustomerName },
            //    { _localizer["الحالة"], item => item.TalapStatueAr },
            //    { _localizer["الخزان"], item => item.Customer.BottleNo },
            //    { _localizer["التاريخ"], item => item.TalapDate }
            //}, sheetName: _localizer["الطلبات"]);





                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,//response.Data,
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