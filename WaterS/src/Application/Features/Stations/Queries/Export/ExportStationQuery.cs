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

namespace WaterS.Application.Features.Stations.Queries.Export
{
    public class ExportStationsQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportStationsQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportStationsQueryHandler : IRequestHandler<ExportStationsQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportStationsQueryHandler> _localizer;

        public ExportStationsQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportStationsQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportStationsQuery request, CancellationToken cancellationToken)
        {
            var StationFilterSpec = new StationFilterSpecification(request.SearchString);
            var Stations = await _unitOfWork.Repository<Domain.Entities.Catalog.Station>().Entities
                .Specify(StationFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(Stations, mappers: new Dictionary<string, Func<Domain.Entities.Catalog.Station, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["MyCompanyID"], item => item.myCompany.MyCompanyID },
                { _localizer["MystationID"], item => item.myCompany.MystationID },
                { _localizer["Name"], item => item.Name },
                { _localizer["Adress"], item => item.Adress }
            }, sheetName: _localizer["Stations"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}