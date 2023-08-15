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

namespace WaterS.Application.Features.AccountNames.Queries.Export
{
    public class ExportAccountNameQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportAccountNameQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportAccountNameQueryHandler : IRequestHandler<ExportAccountNameQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportAccountNameQueryHandler> _localizer;

        public ExportAccountNameQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportAccountNameQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportAccountNameQuery request, CancellationToken cancellationToken)
        {
            var TalapFilterSpec = new AccountNameFilterSpecification("");
            var Talaps = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().Entities
                .Specify(TalapFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(Talaps, mappers: new Dictionary<string, Func<Domain.Entities.Catalog.Accounts, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["AccountType"], item => item.AccountType },
                { _localizer["CategoryType"], item => item.CategoryType }
            }, sheetName: _localizer["Accounts"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}