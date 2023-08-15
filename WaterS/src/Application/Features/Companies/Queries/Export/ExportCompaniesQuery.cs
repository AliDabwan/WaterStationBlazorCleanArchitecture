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

namespace WaterS.Application.Features.Companies.Queries.Export
{
    public class ExportCompaniesQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportCompaniesQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportCompaniesQueryHandler : IRequestHandler<ExportCompaniesQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportCompaniesQueryHandler> _localizer;

        public ExportCompaniesQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportCompaniesQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportCompaniesQuery request, CancellationToken cancellationToken)
        {
            var brandFilterSpec = new CompanyFilterSpecification(request.SearchString);
            var brands = await _unitOfWork.Repository<Company>().Entities
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<Company, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name }
            }, sheetName: _localizer["Companies"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
