using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Identity.Roles
{
    public interface IRoleManager : IManager
    {
        Task<IResult<List<RoleResponse>>> GetRolesAsync();

        Task<IResult<string>> SaveAsync(RoleRequest role);

        Task<IResult<string>> DeleteAsync(string id);

        Task<IResult<PermissionResponse>> GetPermissionsAsync(string roleId);

        Task<IResult<string>> UpdatePermissionsAsync(PermissionRequest request);
    }
}