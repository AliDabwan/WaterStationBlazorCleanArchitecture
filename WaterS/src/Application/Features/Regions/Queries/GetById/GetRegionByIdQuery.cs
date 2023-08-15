using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Regions.Queries.GetById
{
    public class GetRegionByIdQuery : IRequest<Result<GetRegionByIdResponse>>
    {
        public int Id { get; set; }
        public GetRegionByIdQuery(int RegionId)
        {
            Id = RegionId;
        }
    }

    internal class GetRegionByIdQueryHandler : IRequestHandler<GetRegionByIdQuery, Result<GetRegionByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetRegionByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetRegionByIdResponse>> Handle(GetRegionByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<Region>().GetByIdAsync(query.Id);
            var mappedBrand = _mapper.Map<GetRegionByIdResponse>(brand);
            return await Result<GetRegionByIdResponse>.SuccessAsync(mappedBrand);
        }
    }
}