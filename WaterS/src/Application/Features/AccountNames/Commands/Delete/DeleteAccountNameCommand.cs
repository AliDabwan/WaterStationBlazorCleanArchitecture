using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountNames.Commands.Delete
{
    public class DeleteAccountNameCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteAccountNameCommandHandler : IRequestHandler<DeleteAccountNameCommand, Result<int>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<DeleteAccountNameCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteAccountNameCommandHandler(IUnitOfWork<int> unitOfWork, IProductRepository productRepository, IStringLocalizer<DeleteAccountNameCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteAccountNameCommand command, CancellationToken cancellationToken)
        {
           
                var brand = await _unitOfWork.Repository<Accounts>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    await _unitOfWork.Repository<Accounts>().DeleteAsync(brand);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountsCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["AccountName Deleted"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["AccountName Not Found!"]);
                }
           
        }
    }
}