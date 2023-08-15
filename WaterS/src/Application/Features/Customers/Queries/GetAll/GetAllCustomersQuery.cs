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

namespace WaterS.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<PaginatedResult<GetAllPagedCustomersResponse>>
    {
        public int PageNumber { get; set; }
        public int customerId { get; set; }
        public int companyId { get; set; }
        public int stationId { get; set; }
        public int driverId { get; set; }
        public int regionId { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string SearchBy { get; set; }
        public string Statue { get; set; }
        public int without { get; set; }

        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllCustomersQuery(int pageNumber, int pageSize, int customerId, int companyId, int stationId, int driverId, int regionId, int without, string searchString, string searchBy, string statue, string orderBy)
        {
            PageNumber = pageNumber;
            this.customerId = customerId;
            this.companyId = companyId;
            this.stationId = stationId;
            this.driverId = driverId;
            this.regionId = regionId;
            this.without = without;

            PageSize = pageSize;
            SearchString = searchString;
            SearchBy = searchBy;
            Statue = statue;


            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, PaginatedResult<GetAllPagedCustomersResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllCustomersQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllPagedCustomersResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Customer, GetAllPagedCustomersResponse>> expression = e => new GetAllPagedCustomersResponse
            {
                Id = e.Id,
                Name = e.Name,
                No = e.No,
                AccountId = e.AccountId,
                Adress = e.Adress,
                Email = e.Email,
                Phone = e.Phone,
                LoginName = e.LoginName,
                LoginPassword = e.LoginPassword,
                Userid = e.Userid,
                BottleNo = e.BottleNo,
                BottleNoStatue = e.BottleNoStatue,

                BottleTypeId = e.BottleTypeId,
                CompanyId = e.CompanyId,
                StationId = e.StationId,
                DriverId = e.DriverId,
                RegionId = e.RegionId,
                Company = e.Company,
                Station = e.Station,
                Driver = e.Driver,
                Region = e.Region,
                BottleType = e.BottleType,
                BottleTypeName = e.BottleType.Name,
                updatedOn = e.LastModifiedOn.HasValue ? e.LastModifiedOn.Value : null,
                CustomerPhones = e.CustomerPhones,
                ShowDetails = false,
                LastFillDate = e.LastFillDate,
                LastFillDateDays = ((DateTime.Now.Date - e.LastFillDate.Date).Days)-e.BottleType.FillDays,
                statue=e.statue

            };
            Expression<Func<Customer, GetAllPagedCustomersResponse>> expressionWithOut = e => new GetAllPagedCustomersResponse
            {
                Id = e.Id,
                Name = e.Name,
                No = e.No,
                AccountId = e.AccountId,
                Adress = e.Adress,
                Email = e.Email,
                Phone = e.Phone,
                LoginName = e.LoginName,
                LoginPassword = e.LoginPassword,
                Userid = e.Userid,
                BottleNo = e.BottleNo,
                BottleNoStatue = e.BottleNoStatue,

                BottleTypeId = e.BottleTypeId,
                CompanyId = e.CompanyId,
                StationId = e.StationId,
                DriverId = e.DriverId,
                RegionId = e.RegionId,
             
                ShowDetails = false,
            
                statue = e.statue

            };
            int serial = 0;
            var CustomersFilterSpec = new CustomerFilterSpecification(request.SearchString,request.SearchBy);
            if (request.OrderBy?.Any() != true)
            {

                if (request.without == 0)
                {




                    if (request.Statue == "deleted")
                    {
                        var data = await _unitOfWork.Repository<Customer>().Entities
                   .Specify(CustomersFilterSpec)
                    .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue == "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                        foreach (var item in data.Data)
                        {
                            serial++;
                            item.serial = serial;
                        }

                        return data;
                    }
                    else
                    {



                        if (request.SearchBy == "LastFillDays")
                        {



                            var data = await _unitOfWork.Repository<Customer>().Entities
                 .Specify(CustomersFilterSpec)
                  .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue != "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId)
                  && (request.regionId == 0 || x.RegionId == request.regionId) )
                 .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                            foreach (var item in data.Data)
                            {
                                serial++;
                                item.serial = serial;
                            }

                            return data;

                        }
                        else
                        {
                            var data = await _unitOfWork.Repository<Customer>().Entities
                 .Specify(CustomersFilterSpec)
                  .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue != "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                 .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                            foreach (var item in data.Data)
                            {
                                serial++;
                                item.serial = serial;
                            }

                            return data;

                        }





                    }
                }
                else
                {

                    var data = await _unitOfWork.Repository<Customer>().Entities
                   .Select(expressionWithOut).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue != "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                  .ToPaginatedListAsync(request.PageNumber, request.PageSize);



                    return data;

                }








            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...

                if (request.without == 0)
                {



                    if (request.Statue == "deleted")
                    {
                        var data = await _unitOfWork.Repository<Customer>().Entities
                       .Specify(CustomersFilterSpec)
                       .OrderBy(ordering) // require system.linq.dynamic.core
                        .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue == "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                       .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                        foreach (var item in data.Data)
                        {
                            serial++;
                            item.serial = serial;
                        }

                        return data;
                    }
                    else
                    {

                        if (request.SearchBy == "LastFillDays")
                        {


                            var data = await _unitOfWork.Repository<Customer>().Entities
                        .Specify(CustomersFilterSpec)
                        .OrderBy(ordering) // require system.linq.dynamic.core
                         .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue != "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) 
                         && (request.regionId == 0 || x.RegionId == request.regionId))
                        .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                            foreach (var item in data.Data)
                            {
                                serial++;
                                item.serial = serial;
                            }

                            return data;

                        }else
                        {
                            var data = await _unitOfWork.Repository<Customer>().Entities
                      .Specify(CustomersFilterSpec)
                      .OrderBy(ordering) // require system.linq.dynamic.core
                       .Select(expression).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue != "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                      .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                            foreach (var item in data.Data)
                            {
                                serial++;
                                item.serial = serial;
                            }

                            return data;

                        }



                    }






                }
                else
                {

                    var data = await _unitOfWork.Repository<Customer>().Entities
                      .OrderBy(ordering) // require system.linq.dynamic.core
                       .Select(expressionWithOut).Where(x => (request.companyId == 0 || x.CompanyId == request.companyId) && x.statue != "deleted" && (request.stationId == 0 || x.StationId == request.stationId) && (request.driverId == 0 || x.DriverId == request.driverId) && (request.regionId == 0 || x.RegionId == request.regionId))
                      .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                    return data;

                }

            }
        }
    }
}