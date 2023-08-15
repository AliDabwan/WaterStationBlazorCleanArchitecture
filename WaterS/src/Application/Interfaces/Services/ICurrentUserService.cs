using WaterS.Application.Interfaces.Common;

namespace WaterS.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        string UserName { get; }
        string UserRollId { get; }

        string UserRollName { get; }
    }
}