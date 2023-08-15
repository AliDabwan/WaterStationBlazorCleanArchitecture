using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;
using WaterS.Application.Features.BottleTypes.Queries.GetAll;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.BottleType
{
    public class BottleTypeManager : IBottleTypeManager
    {
        private readonly HttpClient _httpClient;

        public BottleTypeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.BottleTypesEndpoints.Export
                : Routes.BottleTypesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.BottleTypesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllBottleTypesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.BottleTypesEndpoints.GetAll);
            return await response.ToResult<List<GetAllBottleTypesResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditBottleTypeCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.BottleTypesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}