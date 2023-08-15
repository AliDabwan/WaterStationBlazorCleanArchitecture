using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Features.DriverRegions.Queries.GetDriverRegion;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion
{
    public class DriverRegionManager : IDriverRegionManager
    {
        private readonly HttpClient _httpClient;

        public DriverRegionManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DriverRegionsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.DriverRegionsEndpoints.Export
                : Routes.DriverRegionsEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        //public async Task<IResult<string>> GetDriverRegionAsync(int id)
        //{
        //    var response = await _httpClient.GetAsync(Routes.DriverRegionsEndpoints.GetDriverRegion(id));
        //    return await response.ToResult<string>();
        //}
     

        public async Task<PaginatedResult<GetAllPagedDriverRegionsResponse>> GetDriverRegionsAsync(GetAllPagedDriverRegionsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DriverRegionsEndpoints.GetAllPaged(request.PageNumber, request.PageSize,request.CompanyId,request.StationId 
                , request.DriverId,request.RegionId, request.SearchString, request.Orderby));
            return await response.ToPaginatedResult<GetAllPagedDriverRegionsResponse>();
        }

        public async Task<PaginatedResult<GetAllPagedDriverRegionsResponse>> GetDriverRegionsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DriverRegionsEndpoints.GetAll);
            return await response.ToPaginatedResult<GetAllPagedDriverRegionsResponse>();
        }
        public async Task<IResult<int>> SaveAsync(AddEditDriverRegionCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DriverRegionsEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}