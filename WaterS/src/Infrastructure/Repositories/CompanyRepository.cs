using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IRepositoryAsync<Company, int> _repository;

        public CompanyRepository(IRepositoryAsync<Company, int> repository)
        {
            _repository = repository;
        }
    }
}