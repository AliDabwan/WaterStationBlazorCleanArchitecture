using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class AccTransRepository : IAccTransRepository
    {
        private readonly IRepositoryAsync<AccTrans, int> _repository;

        public AccTransRepository(IRepositoryAsync<AccTrans, int> repository)
        {
            _repository = repository;
        }
    }
}