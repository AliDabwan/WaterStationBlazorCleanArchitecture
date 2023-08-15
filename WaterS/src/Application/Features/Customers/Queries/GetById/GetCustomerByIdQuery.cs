using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Features.Customers.Queries;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<Result<GetCustomerByIdResponse>>
    {
        public int Id { get; set; }

        public GetCustomerByIdQuery(int CustomerId)
        {
            Id = CustomerId;
        }
    }

    internal class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper mapper;

        public GetCustomerByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<GetCustomerByIdResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {

            var data = await _unitOfWork.Repository<Customer>().Entities.Where(p => p.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var mappedBrand = mapper.Map<GetCustomerByIdResponse>(data);
            return await Result<GetCustomerByIdResponse>.SuccessAsync(data: mappedBrand);
        }
    }
}