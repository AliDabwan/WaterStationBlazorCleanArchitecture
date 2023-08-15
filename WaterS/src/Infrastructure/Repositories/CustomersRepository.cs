using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly IRepositoryAsync<Customer, int> _repository;

        public CustomersRepository(IRepositoryAsync<Customer, int> repository)
        {
            _repository = repository;
        }
    }
}