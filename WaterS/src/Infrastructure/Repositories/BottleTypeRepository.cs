using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class BottleTypeRepository : IBottleTypeRepository
    {
        private readonly IRepositoryAsync<BottleType, int> _repository;

        public BottleTypeRepository(IRepositoryAsync<BottleType, int> repository)
        {
            _repository = repository;
        }
    }
}