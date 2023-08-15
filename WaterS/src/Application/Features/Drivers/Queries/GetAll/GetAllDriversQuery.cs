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

namespace WaterS.Application.Features.Drivers.Queries.GetAllDrivers
{
    public class GetAllDriversQuery : IRequest<PaginatedResult<GetAllPagedDriversResponse>>
    {
        public int companyId { get; set; }
        public int stationId { get; set; }
        public int driverId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int without { get; set; }

        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllDriversQuery(int pageNumber, int pageSize, string searchString, int companyId, int stationId, int driverId, string orderBy,int without)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            this.companyId = companyId;
            this.without = without;

            this.stationId = stationId;
            this.driverId = driverId;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, PaginatedResult<GetAllPagedDriversResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllDriversQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllPagedDriversResponse>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Driver, GetAllPagedDriversResponse>> expression = e => new GetAllPagedDriversResponse
            {
                Id = e.Id,
                Name=e.Name,
                Adress=e.Adress,
                Email=e.Email,
                Phone=e.Phone,
                LoginName=e.LoginName,
                LoginPassword=e.LoginPassword,
                Userid=e.Userid,
                myCompany = e.myCompany,
                myStation = e.myStation,
           KindType=e.KindType,
           KindTypeAr=e.KindTypeAr,
                CompanyId = e.CompanyId,
                            StationId = e.StationId,
                            CustomerList=e.CustomerList,
                            //DriverRegionList=e.DriverRegionList,
                            CustomersCount=e.CustomerList.Where(x=>x.statue!="deleted").Count(),
                RegionsCount = e.DriverRegionList.Count,

                AccountId = e.AccountId,
                            ShowDetails=false

            };
            Expression<Func<Driver, GetAllPagedDriversResponse>> expressionWithOut = e => new GetAllPagedDriversResponse
            {
                Id = e.Id,
                Name = e.Name,
                Adress = e.Adress,
                Email = e.Email,
                Phone = e.Phone,
                LoginName = e.LoginName,
                LoginPassword = e.LoginPassword,
                Userid = e.Userid,
           
                KindType = e.KindType,
                KindTypeAr = e.KindTypeAr,
                CompanyId = e.CompanyId,
                StationId = e.StationId,
             

                AccountId = e.AccountId,
                ShowDetails = false

            };

            //int serial = 0;
            int serial = 0;

            var DriversFilterSpec = new DriverFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                if (request.without == 0)
                {


                    var data = await _unitOfWork.Repository<Driver>().Entities
                       .Specify(DriversFilterSpec)
                       .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.Id == request.driverId))
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);





                    //foreach (var item in data.Data)
                    //{
                    //    serial++;
                    //    item.serial = serial;
                    //}

                    foreach (var item in data.Data)
                    {
                        serial++;
                        item.serial = serial;
                    }
                    return data;
                }
                else
                {

                    var data = await _unitOfWork.Repository<Driver>().Entities
                       .Select(expressionWithOut).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.Id == request.driverId))
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);




                    return data;
                }


            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                if (request.without == 0)
                {

                    var data = await _unitOfWork.Repository<Driver>().Entities
                   .Specify(DriversFilterSpec)
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.Id == request.driverId))
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                    foreach (var item in data.Data)
                    {
                        serial++;
                        item.serial = serial;
                    }
                    //foreach (var item in data.Data)
                    //{
                    //    serial++;
                    //    item.serial = serial;
                    //}
                    return data;
                }
                else
                {

                    var data = await _unitOfWork.Repository<Driver>().Entities
                       .Select(expressionWithOut).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.Id == request.driverId))
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);




                    return data;
                }


            }
        }
    }
}