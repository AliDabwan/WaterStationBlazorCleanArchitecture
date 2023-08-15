using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Features.Stations.Queries;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Stations.Queries.GetStationById
{
    public class GetStationByIdQuery : IRequest<Result<GetStationByIdResponse>>
    {
        public int Id { get; set; }

        public GetStationByIdQuery(int StationId)
        {
            Id = StationId;
        }
    }

    internal class GetStationByIdQueryHandler : IRequestHandler<GetStationByIdQuery, Result<GetStationByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper mapper;

        public GetStationByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<GetStationByIdResponse>> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<Station>().Entities.Where(p => p.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var mappedBrand = mapper.Map<GetStationByIdResponse>(data);
            return await Result<GetStationByIdResponse>.SuccessAsync(data: mappedBrand);
        }
    }
}