using System.Security.Claims;
using System.Threading.Tasks;
using WaterS.Application.Requests.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Identity.Authentication
{
    public interface IAuthenticationManager : IManager
    {
        Task<IResult> Login(TokenRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}