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

namespace WaterS.Application.Features.AccountMovments.Queries.Export
{
    public class ExportAccountMovmentsQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportAccountMovmentsQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportAccountMovmentsQueryHandler : IRequestHandler<ExportAccountMovmentsQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportAccountMovmentsQueryHandler> _localizer;

        public ExportAccountMovmentsQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportAccountMovmentsQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportAccountMovmentsQuery request, CancellationToken cancellationToken)
        {
            var brandFilterSpec = new AccountMovmentsFilterSpecification();
            var brands = await _unitOfWork.Repository<AccTransMovment>().Entities.Specify(brandFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<AccTransMovment, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Accounts.Name },
                { _localizer["EntryType"], item => item.EntryType },
                { _localizer["Note"], item => item.Note },
                { _localizer["DebitAmmount"], item => item.DebitAmmount },
                                { _localizer["CreditAmmount"], item => item.CreditAmmount }

            }, sheetName: _localizer["AccountMovments"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
