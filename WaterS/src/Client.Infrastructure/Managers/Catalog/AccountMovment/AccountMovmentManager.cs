using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Features.AccountMovments.Queries.GetById;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.AccountMovment
{
    public class AccountMovmentManager : IAccountMovmentManager
    {
        private readonly HttpClient _httpClient;

        public AccountMovmentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<PaginatedResult<GetAllAccountMovmentsResponse>> GetAccountMovmentsAsync(GetAllPagedAccountMovmentsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.AccountMovmentsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.AccountId, request.SearchString, request.Orderby));
            return await response.ToPaginatedResult<GetAllAccountMovmentsResponse>();
        }
        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.AccountMovmentsEndpoints.Export
                : Routes.AccountMovmentsEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }
        public async Task<IResult<string>> GetAccountBalanceAsync(int accountid )
        {
            var response = await _httpClient.GetAsync( Routes.AccountMovmentsEndpoints.GetAccountBalance(accountid));
            return await response.ToResult<string>();
        }

        public async Task<IResult<GetAccountMovmentByIdResponse>> GetAccountMovmentsAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.AccountMovmentsEndpoints.GetAccountMovments(id));
            return await response.ToResult<GetAccountMovmentByIdResponse>();
        }
        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.AccountMovmentsEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }
     
        public async Task<IResult<List<GetAllAccountMovmentsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.AccountMovmentsEndpoints.GetAll);
            return await response.ToResult<List<GetAllAccountMovmentsResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditAccountMovmentCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AccountMovmentsEndpoints.Save, request);

            if (response.IsSuccessStatusCode)
            {
                return await response.ToResult<int>();

            }else
            {
                return await Result<int>.FailAsync(response.StatusCode.ToString()+ "لم يتم الحفظ هناك خطا");

            }
        }
    }
}