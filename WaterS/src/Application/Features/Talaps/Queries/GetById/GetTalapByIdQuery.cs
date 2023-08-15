using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Queries;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Talaps.Queries.GetTalapById
{
    public class GetTalapByIdQuery : IRequest<Result<GetTalapByIdResponse>>
    {
        public int Id { get; set; }

        public GetTalapByIdQuery(int TalapId)
        {
            Id = TalapId;
        }
    }

    internal class GetTalapByIdQueryHandler : IRequestHandler<GetTalapByIdQuery, Result<GetTalapByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper mapper;

        public GetTalapByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<GetTalapByIdResponse>> Handle(GetTalapByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<Talap>().Entities.Where(p => p.Id == request.Id).Include(x => x.Driver).Include(x => x.Customer).FirstOrDefaultAsync(cancellationToken);
            var mappedBrand = mapper.Map<GetTalapByIdResponse>(data);
            return await Result<GetTalapByIdResponse>.SuccessAsync(data: mappedBrand);
        }
    }
}