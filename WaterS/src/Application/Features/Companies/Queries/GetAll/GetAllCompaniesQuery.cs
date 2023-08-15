using AutoMapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Companies.Queries.GetAll
{
    public class GetAllCompaniesQuery : IRequest<Result<List<GetAllCompaniesResponse>>>
    {
        public GetAllCompaniesQuery()
        {
        }
    }

    internal class GetAllCompaniesCachedQueryHandler : IRequestHandler<GetAllCompaniesQuery, Result<List<GetAllCompaniesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllCompaniesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllCompaniesResponse>>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Company>>> getAllCompanies = () => _unitOfWork.Repository<Company>().GetAllAsync();
            var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllCompaniesCacheKey, getAllCompanies);
            var mappedCompanies = _mapper.Map<List<GetAllCompaniesResponse>>(brandList);
            return await Result<List<GetAllCompaniesResponse>>.SuccessAsync(mappedCompanies);
        }
    }
}