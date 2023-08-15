using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.Regions.Commands.AddEdit;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.Regions.Queries.GetById;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Region
{
    public class RegionManager : IRegionManager
    {
        private readonly HttpClient _httpClient;

        public RegionManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.RegionsEndpoints.Export
                : Routes.RegionsEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.RegionsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllRegionsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.RegionsEndpoints.GetAll);
            return await response.ToResult<List<GetAllRegionsResponse>>();
        }
        public async Task<IResult<GetRegionByIdResponse>> GetRegionAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.RegionsEndpoints.GetRegion(id));
            return await response.ToResult<GetRegionByIdResponse>();
        }
        public async Task<IResult<int>> SaveAsync(AddEditRegionCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.RegionsEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}