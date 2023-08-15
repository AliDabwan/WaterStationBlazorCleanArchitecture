using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.DriverRegions.Queries.GetDriverRegion
{
    public class GetDriverRegionQuery : IRequest<Result<GetDriverRegioneResponse>>
    {
        public int Id { get; set; }

        public GetDriverRegionQuery(int Id)
        {
            Id = Id;
        }
    }

    internal class GetDriverRegionQueryHandler : IRequestHandler<GetDriverRegionQuery, Result<GetDriverRegioneResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper mapper;

        public GetDriverRegionQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<GetDriverRegioneResponse>> Handle(GetDriverRegionQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<DriverRegion>().Entities.Where(p => p.RegionId == request.Id).Include(x=>x.Region).Include(x=>x.Driver).FirstOrDefaultAsync(cancellationToken);
            var mappedBrand = mapper.Map<GetDriverRegioneResponse>(data);
            return await Result<GetDriverRegioneResponse>.SuccessAsync(data: mappedBrand);
        }
    }
}