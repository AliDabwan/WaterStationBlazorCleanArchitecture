using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Customers.Queries.Export
{
    public class ExportCustomersQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportCustomersQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportCustomersQueryHandler : IRequestHandler<ExportCustomersQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportCustomersQueryHandler> _localizer;

        public ExportCustomersQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportCustomersQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
        {
            var CustomerFilterSpec = new CustomerFilterSpecification(request.SearchString,"");
            var Customers = await _unitOfWork.Repository<Domain.Entities.Catalog.Customer>().Entities
                .Specify(CustomerFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(Customers, mappers: new Dictionary<string, Func<Domain.Entities.Catalog.Customer, object>>
            {
                { _localizer["Id"], item => item.Id },
                  { _localizer["Name"], item => item.Name },
                { _localizer["Email"], item => item.Email },
                { _localizer["Phone"], item => item.Phone },
                                { _localizer["BottleNo"], item => item.BottleNo },
                { _localizer["Adress"], item => item.Adress },
                { _localizer["Station Name"], item => item.Station.Name },
                { _localizer["Driver Name"], item => item.Driver.Name },
                              { _localizer["Region Name"], item => item.RegionId },


            }, sheetName: _localizer["Customers"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}