using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.CustomerPhones.Commands.AddEdit
{
    public partial class AddEditCustomerPhoneCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public int CompanyId { get; set; }
        //public virtual Company Company { get; set; }
        public int StationId { get; set; }
        //public virtual Station Station { get; set; }
        public int DriverId { get; set; }
        //public virtual Driver Driver { get; set; }
        public int CustomerId { get; set; }
        //public virtual Customer Customer { get; set; }
    }

    internal class AddEditCustomerPhoneCommandHandler : IRequestHandler<AddEditCustomerPhoneCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditCustomerPhoneCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditCustomerPhoneCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditCustomerPhoneCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditCustomerPhoneCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {

                var brand = _mapper.Map<CustomerPhone>(command);

                var check = await _unitOfWork.Repository<CustomerPhone>().GetAllAsync();
                foreach (var item in check)
                {
                    if (item.PhoneNumber== brand.PhoneNumber)
                    {
                        return await Result<int>.FailAsync(_localizer["رقم الجوال مسجل مسبقا"]);

                    }
                }

                await _unitOfWork.Repository<CustomerPhone>().AddAsync(brand);
                //return await Result<int>.FailAsync(_localizer["CompanyId" + command.CompanyId + " : " + "StationId" + command.StationId + " : " + "DriverId" + command.DriverId + " : " + "CustomerId" + command.CustomerId]);

                //await _unitOfWork.Commit();

                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCustomerPhonesCacheKey);

                return await Result<int>.SuccessAsync(brand.Id, _localizer["تم حفظ السجل"]);

            }
            else
            {
                var brand = await _unitOfWork.Repository<CustomerPhone>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    brand.PhoneNumber = command.PhoneNumber ;
                    brand.Description = command.Description ;
                    brand.CustomerId = command.CustomerId;
                    brand.CompanyId = command.CompanyId;
                    brand.StationId = command.StationId;
                    brand.DriverId = command.DriverId;

                    var check2 = await _unitOfWork.Repository<CustomerPhone>().GetAllAsync();
                    foreach (var item in check2)
                    {
                        if (item.PhoneNumber == brand.PhoneNumber && item.CustomerId != brand.CustomerId)
                        {
                            return await Result<int>.FailAsync(_localizer["رقم الجوال مسجل مسبقا"]);

                        }
                    }


                    await _unitOfWork.Repository<CustomerPhone>().UpdateAsync(brand);
                    //await _unitOfWork.Commit(cancellationToken);

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCustomerPhonesCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["تم تعديل السجل"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["رقم الجوال غير موجود!"]);
                }
            }
        }
    }
}