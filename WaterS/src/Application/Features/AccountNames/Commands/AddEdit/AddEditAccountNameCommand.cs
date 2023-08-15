using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountNames.Commands.AddEdit
{
    public partial class AddEditAccountNameCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        public int No { get; set; }

        public string Name { get; set; }
        public int AccountType { get; set; }//debit=0
        public string CategoryType { get; set; }//العملاء - المبيعات =0

        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int CustomerId { get; set; }




        public string UserId { get; set; }

    }

    internal class AddEditAccountNameCommandHandler : IRequestHandler<AddEditAccountNameCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditAccountNameCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditAccountNameCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditAccountNameCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditAccountNameCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var brand = _mapper.Map<Accounts>(command);
                await _unitOfWork.Repository<Accounts>().AddAsync(brand);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountNamesCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, _localizer["AccountName Saved"]);
            }
            else
            {
                var brand = await _unitOfWork.Repository<Accounts>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    brand.Name = command.Name ?? brand.Name;
                 
                    await _unitOfWork.Repository<Accounts>().UpdateAsync(brand);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountNamesCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["AccountName Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["AccountName Not Found!"]);
                }
            }
        }
    }
}