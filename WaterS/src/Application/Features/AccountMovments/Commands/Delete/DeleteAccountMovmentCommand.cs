using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountMovments.Commands.Delete
{
    public class DeleteAccountMovmentCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteAccountMovmentCommandHandler : IRequestHandler<DeleteAccountMovmentCommand, Result<int>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<DeleteAccountMovmentCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteAccountMovmentCommandHandler(IUnitOfWork<int> unitOfWork, IProductRepository productRepository, IStringLocalizer<DeleteAccountMovmentCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteAccountMovmentCommand command, CancellationToken cancellationToken)
        {
            //var isBrandUsed = await _productRepository.IsBrandUsed(command.Id);
            //if (!isBrandUsed)
            //{
                var brand = await _unitOfWork.Repository<AccTransMovment>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    await _unitOfWork.Repository<AccTransMovment>().DeleteAsync(brand);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountMovmentsCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Deleted"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Brand Not Found!"]);
                }
            //}
            //else
            //{
            //    return await Result<int>.FailAsync(_localizer["Deletion Not Allowed"]);
            //}
        }
    }
}