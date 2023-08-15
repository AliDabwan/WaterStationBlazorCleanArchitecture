using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Identity.RoleClaims
{
    public interface IRoleClaimManager : IManager
    {
        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsAsync();

        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId);

        Task<IResult<string>> SaveAsync(RoleClaimRequest role);

        Task<IResult<string>> DeleteAsync(string id);
    }
}