using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Stations.Commands.Delete
{
    public class DeleteStationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteStationCommandHandler : IRequestHandler<DeleteStationCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteStationCommandHandler> _localizer;

        public DeleteStationCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteStationCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteStationCommand command, CancellationToken cancellationToken)
        {
            var Station = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().GetByIdAsync(command.Id);
            if (Station != null)
            {
                await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().DeleteAsync(Station);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(Station.Id, _localizer["Station Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Station Not Found!"]);
            }
        }
    }
}