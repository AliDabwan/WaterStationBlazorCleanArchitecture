using System.Threading.Tasks;
using WaterS.Application.Features.Dashboards.Queries.GetData;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();

    }
}