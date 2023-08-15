using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Companies.Commands.Delete
{
    public class DeleteCompanyCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<int>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<DeleteCompanyCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteCompanyCommandHandler(IUnitOfWork<int> unitOfWork, IProductRepository productRepository, IStringLocalizer<DeleteCompanyCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
        {
            //var isCompanyUsed = await _productRepository.IsCompanyUsed(command.Id);
            //if (!isCompanyUsed)
            //{
                var Company = await _unitOfWork.Repository<Company>().GetByIdAsync(command.Id);
                if (Company != null)
                {
                    await _unitOfWork.Repository<Company>().DeleteAsync(Company);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCompaniesCacheKey);
                    return await Result<int>.SuccessAsync(Company.Id, _localizer["Company Deleted"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Company Not Found!"]);
                }
            //}
            //else
            //{
            //    return await Result<int>.FailAsync(_localizer["Deletion Not Allowed"]);
            //}
        }
    }
}