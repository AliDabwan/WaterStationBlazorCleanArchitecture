using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class CustomerPhoneRepository : ICustomerPhoneRepository
    {
        private readonly IRepositoryAsync<CustomerPhone, int> _repository;

        public CustomerPhoneRepository(IRepositoryAsync<CustomerPhone, int> repository)
        {
            _repository = repository;
        }
    }
}