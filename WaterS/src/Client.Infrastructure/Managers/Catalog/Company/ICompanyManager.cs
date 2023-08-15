using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Company
{
    public interface ICompanyManager : IManager
    {
        Task<IResult<List<GetAllCompaniesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditCompanyCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}