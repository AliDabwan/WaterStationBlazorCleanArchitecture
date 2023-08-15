using System.Threading.Tasks;
using WaterS.Application.Interfaces.Common;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}