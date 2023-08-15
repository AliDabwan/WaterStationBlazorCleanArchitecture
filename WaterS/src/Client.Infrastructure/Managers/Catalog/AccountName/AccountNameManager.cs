using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountNames.Commands.AddEdit;
using WaterS.Application.Features.AccountNames.Queries.GetAll;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.AccountName
{
    public class AccountNameManager : IAccountNameManager
    {
        private readonly HttpClient _httpClient;

        public AccountNameManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.AccountNamesEndpoints.Export
                : Routes.AccountNamesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.AccountNamesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllAccountNamesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.AccountNamesEndpoints.GetAll);
            return await response.ToResult<List<GetAllAccountNamesResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditAccountNameCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AccountNamesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}