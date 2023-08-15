using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class TalapRepository : ITalapRepository
    {
        private readonly IRepositoryAsync<Talap, int> _repository;

        public TalapRepository(IRepositoryAsync<Talap, int> repository)
        {
            _repository = repository;
        }
    }
}