using System.Threading.Tasks;

namespace WaterS.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<bool> IsBrandUsed(int brandId);
    }
}