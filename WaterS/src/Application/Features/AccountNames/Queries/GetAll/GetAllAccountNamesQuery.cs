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

namespace WaterS.Application.Features.AccountNames.Queries.GetAll
{
    public class GetAllAccountNamesQuery : IRequest<Result<List<GetAllAccountNamesResponse>>>
    {
        public GetAllAccountNamesQuery()
        {
        }
    }

    internal class GetAllAccountNamesCachedQueryHandler : IRequestHandler<GetAllAccountNamesQuery, Result<List<GetAllAccountNamesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllAccountNamesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<Result<List<GetAllAccountNamesResponse>>> Handle(GetAllAccountNamesQuery request, CancellationToken cancellationToken)
        {
               var TalapsFilterSpec = new AccountNameFilterSpecification("");
       
            //Func<Task<List<Accounts>>> getAllBottleTypes = () => _unitOfWork.Repository<Accounts>().Entities.Specify(TalapsFilterSpec).Select(expression).ToList();
            var getAllBottleTypes = _unitOfWork.Repository<Accounts>().Entities.ToList();

            //var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllAccountNamesCacheKey, getAllBottleTypes);
            var mappedBottleTypes = _mapper.Map<List<GetAllAccountNamesResponse>>(getAllBottleTypes);
            return await Result<List<GetAllAccountNamesResponse>>.SuccessAsync(mappedBottleTypes);
        }
        //public async Task<Result<List<GetAllAccountNamesResponse>>> Handle(GetAllAccountNamesQuery request, CancellationToken cancellationToken)
        //{
        //    Expression<Func<Accounts, GetAllAccountNamesResponse>> expression = e => new GetAllAccountNamesResponse
        //    {
        //        Id = e.Id,

        //        No = e.No,
        //        Name = e.Name,
        //        AccountType = e.AccountType,
        //        CategoryType = e.CategoryType,

        //        CompanyId = e.CompanyId,
        //        StationId = e.StationId,
        //        DriverId = e.DriverId,
        //        CustomerId = e.CustomerId,


        //    };
        //    var TalapsFilterSpec = new AccountNameFilterSpecification();

        //        var getAllAccountNames = _unitOfWork.Repository<Accounts>().Entities
        //      .Specify(TalapsFilterSpec)
        //           .Select(expression).ToList();

        //    //var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllAccountsCacheKey, getAllAccountNames);
        //    var mappedAccountNames = _mapper.Map<List<GetAllAccountNamesResponse>>(getAllAccountNames);
        //        return await Result<List<GetAllAccountNamesResponse>>.SuccessAsync(mappedAccountNames);

        //}
    }
}