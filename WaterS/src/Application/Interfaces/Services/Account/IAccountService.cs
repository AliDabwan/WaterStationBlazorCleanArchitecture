using System.Threading.Tasks;
using WaterS.Application.Interfaces.Common;
using WaterS.Application.Requests.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Interfaces.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}