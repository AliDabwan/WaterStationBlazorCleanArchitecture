using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountNames.Queries.GetById
{
    public class GetAccountNameByIdQuery : IRequest<Result<GetAccountNameByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetProductByIdQueryHandler : IRequestHandler<GetAccountNameByIdQuery, Result<GetAccountNameByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAccountNameByIdResponse>> Handle(GetAccountNameByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<Accounts>().GetByIdAsync(query.Id);
            var mappedAccountName = _mapper.Map<GetAccountNameByIdResponse>(brand);
            return await Result<GetAccountNameByIdResponse>.SuccessAsync(mappedAccountName);
        }
    }
}