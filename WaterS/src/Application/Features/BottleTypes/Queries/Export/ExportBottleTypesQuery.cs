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

namespace WaterS.Application.Features.BottleTypes.Queries.Export
{
    public class ExportBottleTypesQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportBottleTypesQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportBottleTypesQueryHandler : IRequestHandler<ExportBottleTypesQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportBottleTypesQueryHandler> _localizer;

        public ExportBottleTypesQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportBottleTypesQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportBottleTypesQuery request, CancellationToken cancellationToken)
        {
            var brandFilterSpec = new BottleTypeFilterSpecification(request.SearchString);
            var brands = await _unitOfWork.Repository<BottleType>().Entities.Specify(brandFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<BottleType, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Description"], item => item.Description },
                { _localizer["FillDays"], item => item.FillDays },
                { _localizer["Price"], item => item.Price }
            }, sheetName: _localizer["BottleTypes"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
