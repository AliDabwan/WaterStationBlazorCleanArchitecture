using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Companies.Queries.GetById
{
    public class GetCompanyByIdQuery : IRequest<Result<GetCompanyByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<GetCompanyByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetCompanyByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetCompanyByIdResponse>> Handle(GetCompanyByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<Company>().GetByIdAsync(query.Id);
            var mappedBrand = _mapper.Map<GetCompanyByIdResponse>(brand);
            return await Result<GetCompanyByIdResponse>.SuccessAsync(mappedBrand);
        }
     
    }
}