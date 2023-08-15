using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Identity.Users
{
    public interface IUserManager : IManager
    {
        Task<IResult<List<UserResponse>>> GetAllAsync();

        Task<IResult<UserResponse>> GetAllAsyncByUser(string userName);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult<UserResponse>> GetAsync(string userId);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);

        Task<IResult> RegisterUserAsync(RegisterRequest request);

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}