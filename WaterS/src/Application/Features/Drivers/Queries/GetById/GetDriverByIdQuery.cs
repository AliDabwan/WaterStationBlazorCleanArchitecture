using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Features.Drivers.Queries;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Drivers.Queries.GetDriverById
{
    public class GetDriverByIdQuery : IRequest<Result<GetDriverByIdResponse>>
    {
        public int Id { get; set; }

        public GetDriverByIdQuery(int DriverId)
        {
            Id = DriverId;
        }
    }

    internal class GetDriverByIdQueryHandler : IRequestHandler<GetDriverByIdQuery, Result<GetDriverByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper mapper;

        public GetDriverByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<GetDriverByIdResponse>> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().Entities.Where(p => p.Id == request.Id).Include(x=>x.myCompany).Include(x => x.myStation).FirstOrDefaultAsync(cancellationToken);
            var mappedBrand = mapper.Map<GetDriverByIdResponse>(data);
            return await Result<GetDriverByIdResponse>.SuccessAsync(data: mappedBrand);
        }
    }
}