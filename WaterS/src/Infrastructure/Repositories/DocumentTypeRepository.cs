using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Misc;

namespace WaterS.Infrastructure.Repositories
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly IRepositoryAsync<DocumentType, int> _repository;

        public DocumentTypeRepository(IRepositoryAsync<DocumentType, int> repository)
        {
            _repository = repository;
        }
    }
}