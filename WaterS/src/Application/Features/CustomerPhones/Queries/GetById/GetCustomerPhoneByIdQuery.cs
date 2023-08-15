using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.CustomerPhones.Queries.GetById
{
    public class GetCustomerPhoneByIdQuery : IRequest<Result<GetCustomerPhoneByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetCustomerPhoneByIdQueryHandler : IRequestHandler<GetCustomerPhoneByIdQuery, Result<GetCustomerPhoneByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetCustomerPhoneByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetCustomerPhoneByIdResponse>> Handle(GetCustomerPhoneByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<CustomerPhone>().GetByIdAsync(query.Id);
            var mappedBrand = _mapper.Map<GetCustomerPhoneByIdResponse>(brand);
            return await Result<GetCustomerPhoneByIdResponse>.SuccessAsync(mappedBrand);
        }
    }
}