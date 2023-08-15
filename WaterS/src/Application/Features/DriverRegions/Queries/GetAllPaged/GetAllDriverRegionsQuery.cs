using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.DriverRegions.Queries.GetAllPaged
{
    public class GetAllDriverRegionsQuery : IRequest<PaginatedResult<GetAllPagedDriverRegionsResponse>>
    {
        public  int stationId;
        public int driverId;
        public int companyId;
        public int regionId;

        
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDriverRegionsQuery(int pageNumber, int pageSize,int companyId, int stationId, int driverId,int regionId, string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            this.stationId = stationId;
            this.driverId = driverId;
            this.companyId = companyId;
            this.regionId = regionId;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllDriverRegionsQueryHandler : IRequestHandler<GetAllDriverRegionsQuery, PaginatedResult<GetAllPagedDriverRegionsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllDriverRegionsQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllPagedDriverRegionsResponse>> Handle(GetAllDriverRegionsQuery request, CancellationToken cancellationToken)
        {
            //if (request.PageSize==0)
            //{
            //    request.PageSize = 1000000;
            //}

            Expression<Func<DriverRegion, GetAllPagedDriverRegionsResponse>> expression = e => new GetAllPagedDriverRegionsResponse
            {
                Id = e.Id,
                RegionId = e.RegionId,
                DriverId = e.DriverId,
                CompanyId = e.CompanyId,
                StationId = e.StationId,
                Driver = e.Driver,
                Region = e.Region,
                Station = e.Station,
                Company = e.Company,
                ShowDetails = false,
                CustomerList = null,
                CustomerCounts = 0
                //CustomerList = e.CustomerList
            };
            var DriverRegionFilterSpec = new DriverRegionFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<DriverRegion>().Entities
               .Specify(DriverRegionFilterSpec)
               .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) 
               && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
               .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
          //      if (request.stationId == 0)
          //      {
          //          var data = await _unitOfWork.Repository<DriverRegion>().Entities
          //       .Specify(DriverRegionFilterSpec)
          //       .Select(expression)
          //       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
          //          return data;
          //      }
          //      else
          //      {
          //          if (request.driverId == 0)
          //          {
          //              var data = await _unitOfWork.Repository<DriverRegion>().Entities
          // .Specify(DriverRegionFilterSpec)
          // .Select(expression).Where(x => x.StationId == request.stationId)
          // .ToPaginatedListAsync(request.PageNumber, request.PageSize);
          //              return data;
          //          }
          //          else
          //          {
          //              var data = await _unitOfWork.Repository<DriverRegion>().Entities
          //.Specify(DriverRegionFilterSpec)
          //.Select(expression).Where(x => x.StationId == request.stationId &&x.DriverId==request.driverId)
          //.ToPaginatedListAsync(request.PageNumber, request.PageSize);
          //              return data;
          //          }



          //      }
              
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                //if (request.stationId == 0)
                //{
                //    var data = await _unitOfWork.Repository<DriverRegion>().Entities
                //   .Specify(DriverRegionFilterSpec)
                //   .OrderBy(ordering) // require system.linq.dynamic.core
                //   .Select(expression)
                //   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                //    return data;
                //}
                //else
                //{

                    //if (request.driverId == 0)
                    //{
                    //    var data = await _unitOfWork.Repository<DriverRegion>().Entities
                    //.Specify(DriverRegionFilterSpec)
                    //.OrderBy(ordering) // require system.linq.dynamic.core
                    //.Select(expression).Where(x => x.StationId == request.stationId)
                    //.ToPaginatedListAsync(request.PageNumber, request.PageSize);
                    //    return data;

                    //}
                    //else
                    //{

                        var data = await _unitOfWork.Repository<DriverRegion>().Entities
                  .Specify(DriverRegionFilterSpec)
                  .OrderBy(ordering) // require system.linq.dynamic.core
               .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId))
                  .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                        return data;
                    //}

                //}
            }
        }
    }
}