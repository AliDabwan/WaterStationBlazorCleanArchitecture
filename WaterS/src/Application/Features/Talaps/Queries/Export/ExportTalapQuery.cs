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

namespace WaterS.Application.Features.Talaps.Queries.Export
{
    public class ExportTalapsQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportTalapsQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportTalapsQueryHandler : IRequestHandler<ExportTalapsQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportTalapsQueryHandler> _localizer;

        public ExportTalapsQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportTalapsQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportTalapsQuery request, CancellationToken cancellationToken)
        {
            var TalapFilterSpec = new TalapFilterSpecification(request.SearchString);
            var Talaps = await _unitOfWork.Repository<Domain.Entities.Catalog.Talap>().Entities
                .Specify(TalapFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(Talaps, mappers: new Dictionary<string, Func<Domain.Entities.Catalog.Talap, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["المحطة"], item => item.Station.Name },
                { _localizer["السائق"], item => item.Driver.Name },
                { _localizer["المنطقة"], item => item.Region.Name },
                                { _localizer["الزبون"], item => item.Customer.Name },
                                                                { _localizer["الجوال"], item => item.Customer.Phone },
                                                                                                                                { _localizer["عنوانه"], item => item.Customer.Adress },
                { _localizer["حالة الطلب"], item => item.TalapStatueAr },
                { _localizer["الخزان"], item => item.Customer.BottleNo },
                { _localizer["تاريخ الطلب"], item => item.TalapDate }
            }, sheetName: _localizer["Talaps"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}