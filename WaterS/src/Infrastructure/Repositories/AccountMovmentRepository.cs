using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class AccountMovmentRepository : IAccountMovmentRepository
    {
        private readonly IRepositoryAsync<AccTransMovment, int> _repository;

        public AccountMovmentRepository(IRepositoryAsync<AccTransMovment, int> repository)
        {
            _repository = repository;
        }
    }
}