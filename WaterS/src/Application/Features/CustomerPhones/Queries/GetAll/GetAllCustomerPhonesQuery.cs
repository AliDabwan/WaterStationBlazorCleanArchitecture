using AutoMapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.CustomerPhones.Queries.GetAll
{
    public class GetAllCustomerPhonesQuery : IRequest<Result<List<GetAllCustomerPhonesResponse>>>
    {
        public GetAllCustomerPhonesQuery()
        {
        }
    }

    internal class GetAllCustomerPhonesCachedQueryHandler : IRequestHandler<GetAllCustomerPhonesQuery, Result<List<GetAllCustomerPhonesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllCustomerPhonesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllCustomerPhonesResponse>>> Handle(GetAllCustomerPhonesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<CustomerPhone, GetAllCustomerPhonesResponse>> expression = e => new GetAllCustomerPhonesResponse
            {
                Id = e.Id,
                PhoneNumber = e.PhoneNumber,
               
                CompanyId = e.CompanyId,
                StationId = e.StationId,
                DriverId = e.DriverId,
                CustomerId = e.CustomerId,
                //Company = e.Company,
                //Station = e.Station,
                //Driver = e.Driver,
                AccountId=e.Customer.AccountId,
                Customer = e.Customer,
                Description = e.Description
              
            };
            var CustomersFilterSpec = new CustomerPhoneFilterSpecification("");

          var getAllCustomerPhones = _unitOfWork.Repository<CustomerPhone>()
            .Entities.Specify(CustomersFilterSpec).Select(expression).ToList();
          
            
            //var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllBottletypesCacheKey, getAllCustomerPhones);
            //var mappedCustomerPhones = _mapper.Map<List<GetAllCustomerPhonesResponse>>(getAllCustomerPhones);
            return await Result<List<GetAllCustomerPhonesResponse>>.SuccessAsync(getAllCustomerPhones);
        }
    }
}