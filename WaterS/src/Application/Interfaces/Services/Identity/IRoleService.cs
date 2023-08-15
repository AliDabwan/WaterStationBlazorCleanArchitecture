using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Common;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Interfaces.Services.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<List<RoleResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleResponse>> GetByIdAsync(string id);

        Task<Result<string>> SaveAsync(RoleRequest request);

        Task<Result<string>> DeleteAsync(string id);

        Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId);

        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
    }
}