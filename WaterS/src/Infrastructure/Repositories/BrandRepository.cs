using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IRepositoryAsync<Brand, int> _repository;

        public BrandRepository(IRepositoryAsync<Brand, int> repository)
        {
            _repository = repository;
        }
    }
}