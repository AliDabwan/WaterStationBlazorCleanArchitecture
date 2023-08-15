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

namespace WaterS.Application.Features.Stations.Queries.GetAllStations
{
    public class GetAllStationsQuery : IRequest<PaginatedResult<GetAllPagedStationsResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllStationsQuery(int pageNumber, int pageSize, string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllStationsQueryHandler : IRequestHandler<GetAllStationsQuery, PaginatedResult<GetAllPagedStationsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetAllStationsQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<GetAllPagedStationsResponse>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Domain.Entities.Catalog.Station, GetAllPagedStationsResponse>> expression = e => new GetAllPagedStationsResponse
            {
                Id = e.Id,
                Name=e.Name,
                ResName=e.ResName,
                Adress=e.Adress,
                Email=e.Email,
                Phone=e.Phone,
                LoginName=e.LoginName,
                LoginPassword=e.LoginPassword,
                Userid=e.Userid,
                MyCompanyId = e.myCompany.MyCompanyID,
                MyStationId = e.myCompany.MystationID,
                myCompany = e.myCompany,
           
                CompanyId = e.CompanyId,
                AccountId = e.AccountId

            };
            var StationsFilterSpec = new StationFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().Entities
                   .Specify(StationsFilterSpec)
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().Entities
                   .Specify(StationsFilterSpec)
                   .OrderBy(ordering) // require system.linq.dynamic.core
                   .Select(expression)
                   .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;

            }
        }
    }
}