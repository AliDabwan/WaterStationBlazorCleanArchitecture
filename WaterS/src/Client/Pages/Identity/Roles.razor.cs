﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Identity.Roles;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Constants.Permission;

namespace WaterS.Client.Pages.Identity
{
    public partial class Roles
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<RoleResponse> _roleList = new();
        private RoleResponse _role = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateRoles;
        private bool _canEditRoles;
        private bool _canDeleteRoles;
        private bool _canSearchRoles;
        private bool _canViewRoleClaims;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Create)).Succeeded;
            _canEditRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Edit)).Succeeded;
            _canDeleteRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Delete)).Succeeded;
            _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;
            _canViewRoleClaims = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.View)).Succeeded;

            await GetRolesAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetRolesAsync()
        {
            var response = await RoleManager.GetRolesAsync();
            if (response.Succeeded)
            {
                if (_currentUser.IsInRole("Administrator"))
                {
                    _roleList = response.Data;

                }
                else
                {
                    _roleList = response.Data.Where(x => x.Name != "Administrator").ToList();

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

        private async Task Delete(string id)
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
                var response = await RoleManager.DeleteAsync(id);
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

        private async Task InvokeModal(string id = null)
        {
            var parameters = new DialogParameters();
            if (id != null)
            {
                _role = _roleList.FirstOrDefault(c => c.Id == id);
                if (_role != null)
                {
                    parameters.Add(nameof(RoleModal.RoleModel), new RoleRequest
                    {
                        Id = _role.Id,
                        Name = _role.Name,
                        Description = _role.Description
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<RoleModal>(id == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _role = new RoleResponse();
            await GetRolesAsync();
        }

        private bool Search(RoleResponse role)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }

        private void ManagePermissions(string roleId)
        {
            _navigationManager.NavigateTo($"/identity/role-permissions/{roleId}");
        }
    }
}