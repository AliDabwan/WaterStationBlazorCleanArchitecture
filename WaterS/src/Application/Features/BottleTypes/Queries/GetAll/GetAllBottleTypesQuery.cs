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

namespace WaterS.Application.Features.BottleTypes.Queries.GetAll
{
    public class GetAllBottleTypesQuery : IRequest<Result<List<GetAllBottleTypesResponse>>>
    {
        public GetAllBottleTypesQuery()
        {
        }
    }

    internal class GetAllBottleTypesCachedQueryHandler : IRequestHandler<GetAllBottleTypesQuery, Result<List<GetAllBottleTypesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllBottleTypesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllBottleTypesResponse>>> Handle(GetAllBottleTypesQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<BottleType>>> getAllBottleTypes = () => _unitOfWork.Repository<BottleType>().GetAllAsync();
            var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllBottletypesCacheKey, getAllBottleTypes);
            var mappedBottleTypes = _mapper.Map<List<GetAllBottleTypesResponse>>(brandList);
            return await Result<List<GetAllBottleTypesResponse>>.SuccessAsync(mappedBottleTypes);
        }
    }
}