using System.Net.Http;
using System.Threading.Tasks;
using WaterS.Application.Features.Dashboards.Queries.GetData;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Dashboard
{
    public class DashboardManager : IDashboardManager
    {
        private readonly HttpClient _httpClient;

        public DashboardManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<DashboardDataResponse>> GetDataAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DashboardEndpoints.GetData);
            var data = await response.ToResult<DashboardDataResponse>();
            return data;
        }
       
    }
}