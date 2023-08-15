using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Requests;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.DriverRegions.Commands.AddEdit
{
    public partial class AddEditDriverRegionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        //[Required]
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int RegionId { get; set; }
        //[Required]
        public int DriverId { get; set; }
        public virtual Region Region { get; set; }
        public virtual Driver Driver { get; set; }
    }

    internal class AddEditDriverRegionCommandHandler : IRequestHandler<AddEditDriverRegionCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditDriverRegionCommandHandler> _localizer;

        public AddEditDriverRegionCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper,  IStringLocalizer<AddEditDriverRegionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDriverRegionCommand command, CancellationToken cancellationToken)
        {
            

            if (command.Id == 0)
            {
                if (await _unitOfWork.Repository<DriverRegion>().Entities.Where(p => p.Id != command.Id)
                .AnyAsync(p => p.DriverId == command.DriverId && p.RegionId == command.RegionId, cancellationToken))
                {
                    return await Result<int>.FailAsync(_localizer["لايمكن إضافة السائق لنفس المنطقة اكثر من مرة."]);
                }
                if (await _unitOfWork.Repository<DriverRegion>().Entities.Where(p => p.Id != command.Id)
               .AnyAsync(p => p.RegionId == command.RegionId && p.StationId == command.StationId, cancellationToken))
                {
                    return await Result<int>.FailAsync(_localizer["لايمكن إضافة اكثر من سائق للمنطقة"]);
                }
                var mydriver = await _unitOfWork.Repository<Driver>().GetByIdAsync(command.DriverId);



                var DriverRegion = _mapper.Map<DriverRegion>(command);

                DriverRegion.StationId = mydriver.StationId;
                DriverRegion.CompanyId = mydriver.CompanyId;

                await _unitOfWork.Repository<DriverRegion>().AddAsync(DriverRegion);
                await _unitOfWork.Commit(cancellationToken);
                return await Result<int>.SuccessAsync(DriverRegion.Id, _localizer[" تم إضافة السائق  للمنطقة بنجاح"]);
            }
            else
            {
                var DriverRegion = await _unitOfWork.Repository<DriverRegion>().GetByIdAsync(command.Id);
                if (DriverRegion != null)
                {

                    if (await _unitOfWork.Repository<DriverRegion>().Entities.Where(p => p.Id != command.Id)
             .AnyAsync(p => p.DriverId == command.DriverId && p.RegionId == command.RegionId, cancellationToken))
                    {
                        return await Result<int>.FailAsync(_localizer["لايمكن إضافة السائق لنفس المنطقة اكثر من مرة."]);
                    }
                    if (await _unitOfWork.Repository<DriverRegion>().Entities.Where(p => p.Id != command.Id)
                   .AnyAsync(p => p.RegionId == command.RegionId && p.StationId == command.StationId, cancellationToken))
                    {
                        return await Result<int>.FailAsync(_localizer["لايمكن إضافة اكثر من سائق للمنطقة"]);
                    }

                    var drivId = DriverRegion.DriverId;
                    var regionId = DriverRegion.RegionId;

                    var getCustomers = await _unitOfWork.Repository<Customer>().Entities.Where(x=>x.StationId== command.StationId).ToListAsync();
                    int custCount = 0;
                    foreach (var item in getCustomers)
                    {


                        if (item.DriverId== drivId && item.RegionId== regionId)
                        {
                            custCount++;

                            item.DriverId = command.DriverId;
                            item.RegionId = command.RegionId;

                            await _unitOfWork.Repository<Customer>().UpdateAsync(item);

                        }

                    }
                    //return await Result<int>.FailAsync(_localizer["u[dat1 "]);

                    DriverRegion.DriverId = command.DriverId;
                    DriverRegion.RegionId = command.RegionId ;
                    DriverRegion.CompanyId = command.CompanyId;
                    DriverRegion.StationId = command.StationId;


                    await _unitOfWork.Repository<DriverRegion>().UpdateAsync(DriverRegion);
                    await _unitOfWork.Commit(cancellationToken);
                    if (custCount>0)
                    {
                        return await Result<int>.SuccessAsync(DriverRegion.Id, _localizer["تم التعديل بنجاح , وتعديل عدد  : "+ custCount + " زبون "]);

                    }
                    else
                    {
                        return await Result<int>.SuccessAsync(DriverRegion.Id, _localizer["تم التعديل بنجاح"]);

                    }
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["السجل غير موجود !"]);
                }
            }
        }
    }
}