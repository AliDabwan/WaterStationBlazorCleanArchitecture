using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Regions.Commands.Delete
{
    public class DeleteRegionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteRegionCommandHandler : IRequestHandler<DeleteRegionCommand, Result<int>>
    {
        //private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<DeleteRegionCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteRegionCommandHandler(IUnitOfWork<int> unitOfWork,IStringLocalizer<DeleteRegionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            //_productRepository = productRepository;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteRegionCommand command, CancellationToken cancellationToken)
        {
            //var isBrandUsed = await _productRepository.IsBrandUsed(command.Id);
            //if (!isBrandUsed)
            //{
                var brand = await _unitOfWork.Repository<Region>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    await _unitOfWork.Repository<Region>().DeleteAsync(brand);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllRegionsCacheKey);
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