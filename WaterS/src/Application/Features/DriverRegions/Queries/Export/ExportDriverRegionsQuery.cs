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

namespace WaterS.Application.Features.DriverRegions.Queries.Export
{
    public class ExportDriverRegionsQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportDriverRegionsQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportDriverRegionsQueryHandler : IRequestHandler<ExportDriverRegionsQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportDriverRegionsQueryHandler> _localizer;

        public ExportDriverRegionsQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportDriverRegionsQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportDriverRegionsQuery request, CancellationToken cancellationToken)
        {
            var DriverRegionFilterSpec = new DriverRegionFilterSpecification(request.SearchString);
            var DriverRegions = await _unitOfWork.Repository<DriverRegion>().Entities
                .Specify(DriverRegionFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(DriverRegions, mappers: new Dictionary<string, Func<DriverRegion, object>>
            {
                { _localizer["Id"], item => item.Id },
                               { _localizer["المحطة"], item => item.Station.Name },

                { _localizer["المنطقة"], item => item.Region.Name },
                { _localizer["السائق"], item => item.Driver.Name }
            }, sheetName: _localizer["مناطق السائقين"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}