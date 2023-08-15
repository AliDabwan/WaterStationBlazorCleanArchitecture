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

namespace WaterS.Application.Features.CustomerPhones.Queries.Export
{
    public class ExportCustomerPhonesQuery : IRequest<Result<string>>
    {
        public string SearchString { get; set; }

        public ExportCustomerPhonesQuery(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal class ExportCustomerPhonesQueryHandler : IRequestHandler<ExportCustomerPhonesQuery, Result<string>>
    {
        private readonly IExcelService _excelService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ExportCustomerPhonesQueryHandler> _localizer;

        public ExportCustomerPhonesQueryHandler(IExcelService excelService
            , IUnitOfWork<int> unitOfWork
            , IStringLocalizer<ExportCustomerPhonesQueryHandler> localizer)
        {
            _excelService = excelService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportCustomerPhonesQuery request, CancellationToken cancellationToken)
        {
            var brandFilterSpec = new CustomerPhoneFilterSpecification(request.SearchString);
            var brands = await _unitOfWork.Repository<CustomerPhone>().Entities.Specify(brandFilterSpec)
                .ToListAsync(cancellationToken);
            var data = await _excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<CustomerPhone, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["الهاتف"], item => item.PhoneNumber },
                { _localizer["الملاحظات"], item => item.Description },
                { _localizer["الزبون"], item => item.CustomerId },
            }, sheetName: _localizer["CustomerPhones"]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
