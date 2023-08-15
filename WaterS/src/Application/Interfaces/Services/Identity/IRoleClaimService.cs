using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Common;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}