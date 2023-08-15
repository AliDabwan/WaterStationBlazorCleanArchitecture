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

namespace WaterS.Application.Features.BottleTypes.Commands.AddEdit
{
    public partial class AddEditBottleTypeCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int FillDays { get; set; }
        public decimal Price { get; set; }
     
    }

    internal class AddEditBottleTypeCommandHandler : IRequestHandler<AddEditBottleTypeCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditBottleTypeCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditBottleTypeCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditBottleTypeCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditBottleTypeCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                
                   var brand = _mapper.Map<BottleType>(command);
                var check = await _unitOfWork.Repository<BottleType>().GetAllAsync();
                foreach (var item in check)
                {
                    if (item.Name== brand.Name)
                    {
                        return await Result<int>.FailAsync(_localizer["BottleType Repeated"]);

                    }
                }
                
                await _unitOfWork.Repository<BottleType>().AddAsync(brand);
                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBottletypesCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, _localizer["BottleType Saved"]);

            }
            else
            {
                var brand = await _unitOfWork.Repository<BottleType>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    brand.Name = command.Name ?? brand.Name;
                    brand.Description = command.Description ?? brand.Description;
                    brand.FillDays = command.FillDays ;
                    brand.Price = command.Price;
                    await _unitOfWork.Repository<BottleType>().UpdateAsync(brand);
                    //await _unitOfWork.Commit(cancellationToken);

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBottletypesCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["BottleType Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["BottleType Not Found!"]);
                }
            }
        }
    }
}