using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Identity.Roles;
using WaterS.Client.Infrastructure.Settings;
using WaterS.Shared.Constants.Application;

namespace WaterS.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string FirstName { get; set; }
        private string NameRoll { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private string CurrentUserRool { get; set; }
        private string CurrentUserRoolAr { get; set; }

        private int CompanyId { get; set; }
        private int StationId { get; set; }
        private int DriverId { get; set; }
        private int CustomerId { get; set; }


        private char FirstLetterOfName { get; set; }
        private string OwnerRoll { get; set; } = "Administrator";
        private string AdminRoll { get; set; } = "Admin";
        private string ManagerRoll { get; set; } = "Manager";
        private string StationRoll { get; set; } = "Station";
        private string DriverRoll { get; set; } = "Driver";
        private string CustomerRoll { get; set; } = "Customer";


        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
                FirstName = user.GetFirstName();

                //_snackBar.Add("23333333333333333333333333333333333333333333333333333333333333333333333");

                if (FirstName.Length > 0)
                {
                    FirstLetterOfName = FirstName[0];
                }
                //SecondName = user.GetLastName();
                //Email = user.GetEmail();

                try
                {
                    var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);

                    if (imageResponse.Succeeded)
                    {
                        ImageDataUrl = imageResponse.Data;
                    }

                }
                catch
                {

                    return;
                }



                try
                {


                    var currentUserResult = await _userManager.GetAsync(CurrentUserId);
                    if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                    {
                        _snackBar.Add(localizer["You are logged out because the user with your Token has been deleted."], Severity.Error);
                        await _authenticationManager.Logout();
                    }
                }
                catch
                {
                    return;
                    //_snackBar.Add(localizer["تم تسجيل الخروج يرجى اعادة الدخول مرة أخرى"], Severity.Error);
                    //await _authenticationManager.Logout();
                }



                try
                {
                    await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);

                }
                catch
                {

                    return;
                }
            }
        }

        private MudTheme _currentTheme;
        private bool _drawerOpen = true;
        private bool _rightToLeft = false;
        private async Task RightToLeftToggle()
        {
            var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;
            _drawerOpen = false;
        }
        private async Task DoRtl(bool rtl)
        {
            var isRtl = await _clientPreferenceManager.DoRtl(rtl);
         
        }

        protected override async Task OnInitializedAsync()
        {

            _currentTheme = BlazorHeroTheme.DefaultTheme;
            _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            _rightToLeft =true;// await _clientPreferenceManager.IsRTL();
            _interceptor.RegisterEvent();

            //_snackBar.Add("xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            //System.Threading.Thread.Sleep(5000);
           await LoadDataAsync();
            try
            {
                hubConnection = hubConnection.TryInitialize(_navigationManager);

            }
            catch 
            {

             
            }

            try
            {
                await hubConnection.StartAsync();
            }
            catch
            {


            }
            hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
            {
                if (CurrentUserId == receiverUserId)
                {
                    try
                    {
                        _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                    }
                    catch 
                    {


                    }
                    _snackBar.Add(message, Severity.Info, config =>
                    {
                        config.VisibleStateDuration = 10000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.Action = localizer["Chat?"];
                        config.ActionColor = Color.Primary;
                        config.Onclick = snackbar =>
                        {
                            _navigationManager.NavigateTo($"chat/{senderUserId}");
                            return Task.CompletedTask;
                        };
                    });

                }
            });


            hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
            {
                try
                {
                    var token = await _authenticationManager.TryForceRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(localizer["Refreshed Token."], Severity.Success);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch //(Exception ex)
                {
                   // Console.WriteLine(ex.Message);
                    _snackBar.Add(localizer["You are Logged Out."], Severity.Error);
                    await _authenticationManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            });


            hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
            {
                if (CurrentUserId != userId)
                {
                    var rolesResponse = await RoleManager.GetRolesAsync();
                    if (rolesResponse.Succeeded)
                    {
                        var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                        if (role != null)
                        {
                            var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
                            NameRoll = currentUserRolesResponse.Data.UserRoles.FirstOrDefault().RoleName;
                            if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                               
                                
                                _snackBar.Add(localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);

                                try
                                {
                                    await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
                                    await _authenticationManager.Logout();
                                    _navigationManager.NavigateTo("/login");
                                }
                                catch
                                {
                                    await _authenticationManager.Logout();
                                    _navigationManager.NavigateTo("/login");

                                    return;
                                }
                                
                                
                            
                            }
                        }
                    }
                }
            });
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{localizer["Logout Confirmation"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
                {nameof(Dialogs.Logout.HubConnection), hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.Show<Dialogs.Logout>(localizer["Logout"], parameters, options);
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task DarkMode()
        {
            bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
            _currentTheme = isDarkMode
                ? BlazorHeroTheme.DefaultTheme
                : BlazorHeroTheme.DarkTheme;
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
    }
}