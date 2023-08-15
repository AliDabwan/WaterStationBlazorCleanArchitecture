using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Chat;
using WaterS.Application.Models.Chat;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
    }
}