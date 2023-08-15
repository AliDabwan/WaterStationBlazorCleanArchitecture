using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Talaps.Commands.Delete
{
    public class DeleteTalapCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteTalapCommandHandler : IRequestHandler<DeleteTalapCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteTalapCommandHandler> _localizer;

        public DeleteTalapCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteTalapCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteTalapCommand command, CancellationToken cancellationToken)
        {
            var Talap = await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().GetByIdAsync(command.Id);
            if (Talap != null)
            {
                await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().DeleteAsync(Talap);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(Talap.Id, _localizer["Talap Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Talap Not Found!"]);
            }
        }
    }
}