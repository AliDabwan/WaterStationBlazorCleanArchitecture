using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.Brands.Commands.AddEdit;
using WaterS.Application.Features.Brands.Queries.GetAll;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Brand
{
    public interface IBrandManager : IManager
    {
        Task<IResult<List<GetAllBrandsResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditBrandCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}