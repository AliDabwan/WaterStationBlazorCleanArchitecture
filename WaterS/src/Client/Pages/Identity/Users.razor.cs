using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Responses.Identity;
using WaterS.Client.Extensions;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Client.Pages.Identity
{
    public partial class Users
    {
        private List<UserResponse> _userList = new();
        private UserResponse _user = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;
        private string CurrentUserId { get; set; }
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateUsers;
        private bool _canSearchUsers;
        private bool _canExportUsers;
        private bool _canViewRoles;
        private bool _canDelete;

        private bool _loaded;

        public string OwnerRoll { get; set; } = "Administrator";
        public string AdminRoll { get; set; } = "Admin";
        public string myRoll { get; set; } = "222";
        public string myManagerRoll { get; set; } = "Manager";
        public string myStationRoll { get; set; } = "Station";
        public string myDriver { get; set; } = "Driver";
        public string try1 { get; set; } = "Driver";
        public string try2 { get; set; } = "Driver";

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
            }
                _currentUser = await _authenticationManager.CurrentUser();
            _canCreateUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Create)).Succeeded;
            _canSearchUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Search)).Succeeded;
            _canExportUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Export)).Succeeded;
            _canViewRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View)).Succeeded;
            _canDelete = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Delete)).Succeeded;

            await GetUsersAsync();
            _loaded = true;
        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }





        public  async Task<IEnumerable<UserResponse>> FindByNameAsync(UserManager< UserResponse> um, string name)
        {
            return  um?.Users?.Where(x => x.UserName == name).ToList();
        }

        private async Task GetUsersAsync()
        {
            var response = await _userManager.GetAllAsync();

            //IEnumerable<UserResponse> x= response.Data.ToList();
            if (response.Succeeded)
            {

                var myUser = await _userManager.GetAsync(CurrentUserId);
                //try1 = myUser.Data.UserName; //_currentUser.Identity.Name;
                //try2 = myUser.Data.KindType; ;//(string.IsNullOrEmpty(myUser.KindTypeAr) ? "ss" : myUser.KindTypeAr) + " " + (myUser.KindId == 0 ? 0 : myUser.KindId);

                if (_currentUser.IsInRole("Administrator"))
                {
                    myRoll = "Administrator";


                    _userList = response.Data.ToList();

                }
                else if (_currentUser.IsInRole("Admin"))
                {
                    myRoll = "Admin";

                    _userList = response.Data.Where(x=>x.KindType!= OwnerRoll ).ToList();

                }
                else if (_currentUser.IsInRole("Manager"))
                {
                    myRoll = "Manager";


                    _userList = response.Data.Where(x=>(x.KindType!= OwnerRoll && !string.IsNullOrEmpty(x.KindType) && x.KindType!= myRoll && 
                    x.KindType != AdminRoll) && x.KindId== myUser.Data.KindId).ToList();

                }
                else if (_currentUser.IsInRole("Station"))
                {
                    myRoll = "Station";


                    _userList = response.Data.Where(x => (x.KindType != myRoll && x.KindType != OwnerRoll &&
                    x.KindType != AdminRoll && x.KindType != myManagerRoll && !string.IsNullOrEmpty(x.KindType)) && x.KindId == myUser.Data.KindId).ToList();
                }
                else if (_currentUser.IsInRole("Driver"))
                {
                    myRoll = "Driver";
                    _userList = response.Data.Where(x => (x.KindType != myRoll && x.KindType != OwnerRoll &&
                 x.KindType != AdminRoll && x.KindType != myManagerRoll && !string.IsNullOrEmpty(x.KindType)) && x.KindId == myUser.Data.KindId && x.DriverId == myUser.Data.DriverId).ToList();
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

        private bool Search(UserResponse user)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (user.FirstName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.LastName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (user.UserName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }
        private async Task Delete(string userId)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, userId)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var myuser = await userManager.FindByIdAsync(userId);

                if (myuser == null)
                {
                      _snackBar.Add(_localizer["لا يوجد يوزر خاص بهذا الحساب"]);
                    return;
                }
                var deletedUserRoll = await userManager.RemoveFromRoleAsync(myuser, "Customer");
                if (deletedUserRoll.Succeeded)
                {
                    var deletedUser = await userManager.DeleteAsync(myuser);
                    if (deletedUser.Succeeded)
                    {

                        _snackBar.Add( _localizer["User Deleted"]);

                    }
                    else
                    {
                        _snackBar.Add(_localizer["خطا في حذف الحساب"]);
                        return;

                    }


                }
                else
                {
                    _snackBar.Add(_localizer["@خطا في حذف الحساب"]);
                    return;

                }
            }
        }
        private async Task ExportToExcel()
        {
            var base64 = await _userManager.ExportToExcelAsync(_searchString);
            await _jsRuntime.InvokeVoidAsync("Download", new
            {
                ByteArray = base64,
                FileName = $"{nameof(Users).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
            _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                ? _localizer["Users exported"]
                : _localizer["Filtered Users exported"], Severity.Success);
        }

        private async Task InvokeModal()
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<RegisterUserModal>(_localizer["Register New User"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await GetUsersAsync();
            }
        }

        private void ViewProfile(string userId)
        {
            _navigationManager.NavigateTo($"/user-profile/{userId}");
        }

        private void ManageRoles(string userId, string email)
        {
            if (email == "dev.alidabwan@gmail.comm") _snackBar.Add(_localizer["Not Allowed."], Severity.Error);
            else _navigationManager.NavigateTo($"/identity/user-roles/{userId}");
        }
    }
}