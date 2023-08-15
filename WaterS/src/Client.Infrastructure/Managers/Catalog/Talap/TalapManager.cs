using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Talap
{
    public class TalapManager : ITalapManager
    {
        private readonly HttpClient _httpClient;

        public TalapManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.TalapsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.TalapsEndpoints.Export
                : Routes.TalapsEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<string>> GetTalapAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.TalapsEndpoints.GetTalap(id));
            return await response.ToResult<string>();
        }

        public async Task<PaginatedResult<GetAllPagedTalapsResponse>> GetTalapsAsync(GetAllPagedTalapsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.TalapsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, 
                request.CustomerId, request.CompanyId, request.StationId, request.DriverId,request.RegionId, request.Statue, request.DateFrom, request.DateTo,
                request.SearchString, request.Orderby));
            return await response.ToPaginatedResult<GetAllPagedTalapsResponse>();
        }
        public async Task<IResult<List<GetAllPagedTalapsResponse>>> GetListAsync()
        {
            var response = await _httpClient.GetAsync(Routes.TalapsEndpoints.GetAlls());
            return await response.ToResult<List<GetAllPagedTalapsResponse>>();
        }
        public async Task<PaginatedResult<GetAllPagedTalapsResponse>> GetTalapsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.TalapsEndpoints.GetAlls());
            return await response.ToPaginatedResult<GetAllPagedTalapsResponse>();
        }
        public async Task<IResult<int>> SaveAsync(AddEditTalapCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.TalapsEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}