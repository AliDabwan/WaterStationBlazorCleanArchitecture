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

namespace WaterS.Application.Features.Regions.Queries.Export
{
    public class ExportRegionsQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportRegionsQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportRegionsQueryHandler : IRequestHandler<ExportRegionsQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportRegionsQueryHandler> _localizer;

        public ExportRegionsQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportRegionsQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportRegionsQuery request, CancellationToken cancellationToken)
        {
            var brandFilterSpec = new RegionFilterSpecification("123");
            var brands = await _unitOfWork.Repository<Region>().Entities.Specify(brandFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<Region, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Description"], item => item.Description }
            }, sheetName: _localizer["Regions"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
