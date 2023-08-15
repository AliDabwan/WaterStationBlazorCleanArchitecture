using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.Drivers.Commands.AddEdit;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Features.Drivers.Queries.GetDriverById;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Driver
{
    public class DriverManager : IDriverManager
    {
        private readonly HttpClient _httpClient;

        public DriverManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DriversEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.DriversEndpoints.Export
                : Routes.DriversEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<GetDriverByIdResponse>> GetDriverAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.DriversEndpoints.GetDriver(id));
            return await response.ToResult<GetDriverByIdResponse>();
        }

        public async Task<PaginatedResult<GetAllPagedDriversResponse>> GetDriversAsync(GetAllPagedDriversRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DriversEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.CompanyId, request.StationId, request.DriverId, request.withOutInclud, request.SearchString, request.Orderby));
            return await response.ToPaginatedResult<GetAllPagedDriversResponse>();
        }
        public async Task<PaginatedResult<GetAllPagedDriversResponse>> GetDriversAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DriversEndpoints.GetAlls());
            return await response.ToPaginatedResult<GetAllPagedDriversResponse>();
        }
        public async Task<IResult<int>> SaveAsync(AddEditDriverCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DriversEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}