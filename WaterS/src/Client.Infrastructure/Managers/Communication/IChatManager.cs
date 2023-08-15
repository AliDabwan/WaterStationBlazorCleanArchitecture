using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Chat;
using WaterS.Application.Models.Chat;
using WaterS.Application.Responses.Identity;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Communication
{
    public interface IChatManager : IManager
    {
        Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
    }
}