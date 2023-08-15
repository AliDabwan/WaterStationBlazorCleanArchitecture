using AutoMapper;
using WaterS.Application.Interfaces.Chat;
using WaterS.Application.Models.Chat;
using WaterS.Infrastructure.Models.Identity;

namespace WaterS.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory<IChatUser>, ChatHistory<BlazorHeroUser>>().ReverseMap();
        }
    }
}