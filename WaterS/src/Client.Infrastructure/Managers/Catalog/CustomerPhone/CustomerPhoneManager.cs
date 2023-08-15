using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;
using WaterS.Application.Features.CustomerPhones.Queries.GetAll;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone
{
    public class CustomerPhoneManager : ICustomerPhoneManager
    {
        private readonly HttpClient _httpClient;

        public CustomerPhoneManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.CustomerPhonesEndpoints.Export
                : Routes.CustomerPhonesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.CustomerPhonesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllCustomerPhonesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.CustomerPhonesEndpoints.GetAll);
            return await response.ToResult<List<GetAllCustomerPhonesResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditCustomerPhoneCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.CustomerPhonesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}