using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.BottleTypes.Queries.GetById
{
    public class GetBottleTypeByIdQuery : IRequest<Result<GetBottleTypeByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetBottleTypeByIdQueryHandler : IRequestHandler<GetBottleTypeByIdQuery, Result<GetBottleTypeByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetBottleTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetBottleTypeByIdResponse>> Handle(GetBottleTypeByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<BottleType>().GetByIdAsync(query.Id);
            var mappedBrand = _mapper.Map<GetBottleTypeByIdResponse>(brand);
            return await Result<GetBottleTypeByIdResponse>.SuccessAsync(mappedBrand);
        }
    }
}