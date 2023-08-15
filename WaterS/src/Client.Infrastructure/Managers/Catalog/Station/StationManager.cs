using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.Stations.Commands.AddEdit;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Application.Features.Stations.Queries.GetStationById;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Station
{
    public class StationManager : IStationManager
    {
        private readonly HttpClient _httpClient;

        public StationManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.StationsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.StationsEndpoints.Export
                : Routes.StationsEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }




        public async Task<IResult<GetStationByIdResponse>> GetStationAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.StationsEndpoints.GetStation(id));
            return await response.ToResult<GetStationByIdResponse>();
        }

        public async Task<PaginatedResult<GetAllPagedStationsResponse>> GetStationsAsync(GetAllPagedStationsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.StationsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.Orderby));
            return await response.ToPaginatedResult<GetAllPagedStationsResponse>();
        }
        public async Task<PaginatedResult<GetAllPagedStationsResponse>> GetStationsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.StationsEndpoints.GetAlls());
            return await response.ToPaginatedResult<GetAllPagedStationsResponse>();
        }
        public async Task<IResult<int>> SaveAsync(AddEditStationCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.StationsEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}