using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services.Identity;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Domain.Entities.ExtendedAttributes;
using WaterS.Domain.Entities.Misc;
using WaterS.Shared.Constants.Role;
using WaterS.Shared.Constants.Statue;
using WaterS.Shared.Constants.Storage;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Dashboards.Queries.GetData
{
    public class GetDashboardDataQuery : IRequest<Result<DashboardDataResponse>>
    {
        public int companyId { get; set; }
        public int stationId { get; set; }
        public int customerId { get; set; }

        public string KindType { get; set; }

    }


    internal class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, Result<DashboardDataResponse>>
    {
        private readonly int customerId;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper mapper;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<GetDashboardDataQueryHandler> _localizer;
      
        public GetDashboardDataQueryHandler( IUnitOfWork<int> unitOfWork, IMapper mapper, IUserService userService, IRoleService roleService, IStringLocalizer<GetDashboardDataQueryHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            _userService = userService;
            _roleService = roleService;
            _localizer = localizer;
        }


        public async Task<Result<DashboardDataResponse>> Handle(GetDashboardDataQuery query, CancellationToken cancellationToken)
        {
            var TalapsFilterSpec = new DashBoardByCustomerFilterSpecification();

            var response = new DashboardDataResponse
            {
                BottleTypeCount = await _unitOfWork.Repository<BottleType>().Entities.CountAsync(cancellationToken),
                //ProductCount = await _unitOfWork.Repository<Product>().Entities.CountAsync(cancellationToken),
                CompanyAdminCount = await _unitOfWork.Repository<Company>().Entities.CountAsync(cancellationToken),
                StationAdminCount = await _unitOfWork.Repository<Station>().Entities.CountAsync(cancellationToken),
                DriverAdminCount = await _unitOfWork.Repository<Driver>().Entities.CountAsync(cancellationToken),
                CustomerAdminCount = await _unitOfWork.Repository<Customer>().Entities.CountAsync(cancellationToken),
                TalapsCompleted = await _unitOfWork.Repository<Talap>().Entities.Where(x => x.TalapStatue == StatueConstants.Completed).CountAsync(cancellationToken),
                TalapsInProcess = await _unitOfWork.Repository<Talap>().Entities.Where(x => x.TalapStatue == StatueConstants.InProcess).CountAsync(cancellationToken),
                AllTalaps = mapper.Map<List<GetAllPagedTalapsResponse>>(await _unitOfWork.Repository<Talap>().Entities.Specify(TalapsFilterSpec).ToListAsync()),






                //BrandCount = await _unitOfWork.Repository<Brand>().Entities.CountAsync(cancellationToken),
                //DocumentCount = await _unitOfWork.Repository<Document>().Entities.CountAsync(cancellationToken),
                //DocumentTypeCount = await _unitOfWork.Repository<DocumentType>().Entities.CountAsync(cancellationToken),
                //DocumentExtendedAttributeCount = await _unitOfWork.Repository<DocumentExtendedAttribute>().Entities.CountAsync(cancellationToken),
                UserCount = await _userService.GetCountAsync(),
                RoleCount = await _roleService.GetCountAsync()
            };

            var selectedYear = DateTime.Now.Year;
            //double[] bottleTypesFigure = new double[13];
            //double[] productsFigure = new double[13];
            //double[] brandsFigure = new double[13];
            double[] talapsFigure = new double[13];
            //double[] documentsFigure = new double[13];
            //double[] documentTypesFigure = new double[13];
            //double[] documentExtendedAttributesFigure = new double[13];
            for (int i = 1; i <= 12; i++)
            {
                var month = i;
                var filterStartDate = new DateTime(selectedYear, month, 01);
                var filterEndDate = new DateTime(selectedYear, month, DateTime.DaysInMonth(selectedYear, month), 23, 59, 59); // Monthly Based

                //bottleTypesFigure[i - 1] = await _unitOfWork.Repository<BottleType>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                //productsFigure[i - 1] = await _unitOfWork.Repository<Product>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                //brandsFigure[i - 1] = await _unitOfWork.Repository<Brand>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                talapsFigure[i - 1] = await _unitOfWork.Repository<Talap>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                //documentsFigure[i - 1] = await _unitOfWork.Repository<Document>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                //documentTypesFigure[i - 1] = await _unitOfWork.Repository<DocumentType>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
                //documentExtendedAttributesFigure[i - 1] = await _unitOfWork.Repository<DocumentExtendedAttribute>().Entities.Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate).CountAsync(cancellationToken);
            }

            //response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["BottleTypes"], Data = bottleTypesFigure });
            //response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Products"], Data = productsFigure });
            //response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Brands"], Data = brandsFigure });
            response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Documents"], Data = talapsFigure });
            //response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Documents"], Data = documentsFigure });
            //response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Types"], Data = documentTypesFigure });
            //response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Document Extended Attributes"], Data = documentExtendedAttributesFigure });

            return await Result<DashboardDataResponse>.SuccessAsync(response);
        }
    }
}