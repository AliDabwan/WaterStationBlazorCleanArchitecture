using AutoMapper;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

namespace WaterS.Application.Features.Regions.Queries.GetAll
{
    public class GetAllRegionsQuery : IRequest<Result<List<GetAllRegionsResponse>>>
    {
        public GetAllRegionsQuery()
        {
        }
    }

    internal class GetAllRegionsCachedQueryHandler : IRequestHandler<GetAllRegionsQuery, Result<List<GetAllRegionsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllRegionsCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllRegionsResponse>>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Region, GetAllRegionsResponse>> expression = e => new GetAllRegionsResponse
            {
                Id = e.Id,
                Name = e.Name,
                StationId = e.StationId,
                Station = e.Station,

                CustomerList = e.CustomerList

            };

            var DriversFilterSpec = new RegionFilterSpecification("");


            var getAllRegions =await _unitOfWork.Repository<Region>().Entities.Specify(DriversFilterSpec).
              Select(expression).ToListAsync();



            //var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllRegionsCacheKey, getAllRegions);
            //var mappedRegions = _mapper.Map<List<GetAllRegionsResponse>>(getAllRegions);




            return await Result<List<GetAllRegionsResponse>>.SuccessAsync(getAllRegions);

            //Func<Task<List<Region>>> getAllBrands = () => _unitOfWork.Repository<Region>().GetAllAsync();
            //var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllBrandsCacheKey, getAllBrands);
            //var mappedBrands = _mapper.Map<List<GetAllBrandsResponse>>(brandList);
            //return await Result<List<GetAllBrandsResponse>>.SuccessAsync(mappedBrands);



        }
    }
}