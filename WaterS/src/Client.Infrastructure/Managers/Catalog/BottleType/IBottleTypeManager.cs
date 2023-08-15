using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;
using WaterS.Application.Features.BottleTypes.Queries.GetAll;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.BottleType
{
    public interface IBottleTypeManager : IManager
    {
        Task<IResult<List<GetAllBottleTypesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditBottleTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}