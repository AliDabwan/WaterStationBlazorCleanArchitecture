using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountMovments.Queries.GetById
{
    public class GetAccountMovmentByIdQuery : IRequest<Result<GetAccountMovmentByIdResponse>>
    {
        public GetAccountMovmentByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    internal class GetAccountMovmentByIdQueryHandler : IRequestHandler<GetAccountMovmentByIdQuery, Result<GetAccountMovmentByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountMovmentByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }   

        public async Task<Result<GetAccountMovmentByIdResponse>> Handle(GetAccountMovmentByIdQuery query, CancellationToken cancellationToken)
        {
            //var brand = await _unitOfWork.Repository<AccTransMovment>().GetByIdAsync(query.Id);

            var data = await _unitOfWork.Repository<AccTransMovment>().Entities.Where(p => p.AccountsId == query.Id).FirstOrDefaultAsync(cancellationToken);

            var mappedBrand = _mapper.Map<GetAccountMovmentByIdResponse>(data);
            return await Result<GetAccountMovmentByIdResponse>.SuccessAsync(mappedBrand);
        }
    }
}