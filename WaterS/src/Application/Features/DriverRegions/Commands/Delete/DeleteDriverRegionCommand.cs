using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.DriverRegions.Commands.Delete
{
    public class DeleteDriverRegionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteDriverRegionCommandHandler : IRequestHandler<DeleteDriverRegionCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteDriverRegionCommandHandler> _localizer;

        public DeleteDriverRegionCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteDriverRegionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteDriverRegionCommand command, CancellationToken cancellationToken)
        {
            var DriverRegion = await _unitOfWork.Repository<DriverRegion>().GetByIdAsync(command.Id);
            if (DriverRegion != null)
            {
                await _unitOfWork.Repository<DriverRegion>().DeleteAsync(DriverRegion);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(DriverRegion.Id, _localizer["DriverRegion Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["DriverRegion Not Found!"]);
            }
        }
    }
}