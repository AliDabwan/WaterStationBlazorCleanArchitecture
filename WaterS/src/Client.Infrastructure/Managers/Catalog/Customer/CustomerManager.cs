using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Application.Features.Customers.Queries.GetCustomerById;
using WaterS.Application.Requests.Catalog;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Customer
{
    public class CustomerManager : ICustomerManager
    {
        private readonly HttpClient _httpClient;

        public CustomerManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.CustomersEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> UnDeleteAsync(int id)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.CustomersEndpoints.UnDelete(id),id);
            return await response.ToResult<int>();
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.CustomersEndpoints.Export
                : Routes.CustomersEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<GetCustomerByIdResponse>> GetCustomerAsync(int id)
        {
            var response = await _httpClient.GetAsync(Routes.CustomersEndpoints.GetCustomer(id));
            return await response.ToResult<GetCustomerByIdResponse>();
        }

        public async Task<PaginatedResult<GetAllPagedCustomersResponse>> GetCustomersAsync(GetAllPagedCustomersRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.CustomersEndpoints.GetAllPaged(request.PageNumber, request.PageSize,
                request.CustomerId, request.CompanyId, request.StationId, request.DriverId, request.RegionId, request.withOutInclud, request.SearchString, request.SearchBy, request.Statue, request.Orderby));
            return await response.ToPaginatedResult<GetAllPagedCustomersResponse>();
        }
        public async Task<PaginatedResult<GetAllPagedCustomersResponse>> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync(Routes.CustomersEndpoints.GetAlls());
            return await response.ToPaginatedResult<GetAllPagedCustomersResponse>();
        }
        public async Task<IResult<int>> SaveAsync(AddEditCustomerCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.CustomersEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}