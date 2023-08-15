using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Client.Infrastructure.Extensions;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Company
{
    public class CompanyManager : ICompanyManager
    {
        private readonly HttpClient _httpClient;

        public CompanyManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.CompaniesEndpoints.Export
                : Routes.CompaniesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.CompaniesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllCompaniesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.CompaniesEndpoints.GetAll);
            return await response.ToResult<List<GetAllCompaniesResponse>>();
        }

     

       
        public async Task<IResult<int>> SaveAsync(AddEditCompanyCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.CompaniesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}