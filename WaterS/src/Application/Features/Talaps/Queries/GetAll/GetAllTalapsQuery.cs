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

namespace WaterS.Application.Features.Talaps.Queries.GetAllTalaps
{
    public class GetAllTalapsQuery : IRequest<PaginatedResult<GetAllPagedTalapsResponse>>
    {
        public  DateTime? dateFrom;
        public  DateTime? dateTo;

        public int PageNumber { get; set; }
        public int customerId { get; set; }
        public int companyId { get; set; }
        public int stationId { get; set; }
        public int driverId { get; set; }
        public int regionId { get; set; }

        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string statue { get; set; }

        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllTalapsQuery(int pageNumber, int pageSize,int customerId, int companyId, int stationId, int driverId,int regionId,string statue
            ,DateTime? dateFrom, DateTime? dateTo ,string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            this.customerId = customerId;
            this.companyId = companyId;
            this.stationId = stationId;
            this.driverId = driverId;
            this.regionId = regionId;

            PageSize = pageSize;
            SearchString = searchString;
            this.statue = statue;
            this.dateFrom = dateFrom;
            this.dateTo = dateTo;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllTalapsQueryHandler : IRequestHandler<GetAllTalapsQuery, PaginatedResult<GetAllPagedTalapsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllTalapsQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllPagedTalapsResponse>> Handle(GetAllTalapsQuery request, CancellationToken cancellationToken)
        {
            //if (request.PageSize==10)
            //{
            //    request.PageSize = 50;
            //}
            if (string.IsNullOrEmpty(request.statue) || request.statue == "")
            {
                request.statue = "all";

            }
            Expression<Func<Talap, GetAllPagedTalapsResponse>> expression = e => new GetAllPagedTalapsResponse
            {
                Id = e.Id,
                ServiceRate = e.ServiceRate,
                TalapStatue = e.TalapStatue,
                TalapStatueAr = e.TalapStatueAr,
                Comment = e.Comment,
                BottleNo = e.BottleNo,
                No = e.No,
                TalapDate = e.TalapDate,
                Price = e.Price,
                DoneByAccountId = e.DoneByAccountId,
                DoneByName = e.DoneByName,
                TalapArrivalDate = e.TalapArrivalDate,
                TalapArrivalTime = e.TalapArrivalTime,
                //Company = e.Company,
                //Station = e.Station,
                DriverName = e.Driver.Name,
                CustomerName = e.Customer.Name,
                CustomerPhone = e.Customer.Phone,
                CustomerAddress = e.Customer.Adress,
                CustomerBotlType = e.Customer.BottleType.Name,

                CompanyName = e.Company.Name,
                StationName = e.Station.Name,
                
                RegionName = e.Region.Name,
                CompanyId = e.CompanyId,
                StationId = e.StationId,
                DriverId = e.DriverId,
                CustomerId = e.CustomerId,
                RegionId = e.RegionId,
                CreatedOn=e.CreatedOn,
                CustomerPhones=e.Customer.CustomerPhones



            };


            var TalapsFilterSpec = new TalapFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
               
                if (request.customerId>0)
                {
                    var data = await _unitOfWork.Repository<Talap>().Entities
                   .Specify(TalapsFilterSpec)
                   .Select(expression).Where(x=> (request.statue == "all" || x.TalapStatue == request.statue) && x.CustomerId==request.customerId)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                    return data;
                }
                else
                {
                    if (request.dateFrom.HasValue)
                    {
                        var data = await _unitOfWork.Repository<Talap>().Entities
                    .Specify(TalapsFilterSpec)
                    .Select(expression).Where(x => (x.CreatedOn.Date >= request.dateFrom && x.CreatedOn.Date <= request.dateTo) &&(request.statue == "all" || x.TalapStatue == request.statue) && (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                        return data;

                    }
                    else
                    {
                        var data = await _unitOfWork.Repository<Talap>().Entities
                    .Specify(TalapsFilterSpec)
                    .Select(expression).Where(x => (request.statue == "all" || x.TalapStatue == request.statue)  && (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                        return data;

                    }
                  



                }
                

            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                if (request.customerId > 0)
                {
                    var data = await _unitOfWork.Repository<Talap>().Entities
                   .Specify(TalapsFilterSpec)
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression).Where(x => (request.statue == "all" || x.TalapStatue == request.statue) && x.CustomerId == request.customerId)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                    return data;
                }
                else
                {
                    if (request.dateFrom.HasValue)
                    {
                        var data = await _unitOfWork.Repository<Talap>().Entities
                   .Specify(TalapsFilterSpec)
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression).Where(x => (x.CreatedOn.Date >= request.dateFrom && x.CreatedOn.Date <= request.dateTo)&&(request.statue == "all" || x.TalapStatue == request.statue) && (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                        return data;
                    }
                    else
                    {
                        var data = await _unitOfWork.Repository<Talap>().Entities
                  .Specify(TalapsFilterSpec)
                  .OrderBy(ordering) // require system.linq.dynamic.core
                  .Select(expression).Where(x => (request.statue == "all" || x.TalapStatue == request.statue) && (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                  .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                        return data;
                    }

                }
            }
        }
    }
}