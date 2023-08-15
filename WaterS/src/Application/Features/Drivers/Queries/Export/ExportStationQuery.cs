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

namespace WaterS.Application.Features.Drivers.Queries.Export
{
    public class ExportDriversQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportDriversQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportDriversQueryHandler : IRequestHandler<ExportDriversQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportDriversQueryHandler> _localizer;

        public ExportDriversQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportDriversQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportDriversQuery request, CancellationToken cancellationToken)
        {
            var DriverFilterSpec = new DriverFilterSpecification(request.SearchString);
            var Drivers = await _unitOfWork.Repository<Domain.Entities.Catalog.Driver>().Entities
                .Specify(DriverFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(Drivers, mappers: new Dictionary<string, Func<Domain.Entities.Catalog.Driver, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Station Name"], item => item.myStation.Name },
                { _localizer["Name"], item => item.Name },
                                { _localizer["Phone"], item => item.Phone },

                { _localizer["Adress"], item => item.Adress }
            }, sheetName: _localizer["Drivers"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}