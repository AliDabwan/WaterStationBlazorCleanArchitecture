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

namespace WaterS.Application.Features.Regions.Commands.AddEdit
{
    public partial class AddEditRegionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? StationId { get; set; }
    }

    internal class AddEditRegionCommandHandler : IRequestHandler<AddEditRegionCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditRegionCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditRegionCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditRegionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditRegionCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                
                   var brand = _mapper.Map<Region>(command);
                var check = await _unitOfWork.Repository<Region>().GetAllAsync();
                foreach (var item in check)
                {
                    if (item.Name== brand.Name)
                    {
                        return await Result<int>.FailAsync(_localizer["Region Repeated"]);

                    }
                }
                brand.StationId = command.StationId.HasValue ? command.StationId.Value : 0;

                await _unitOfWork.Repository<Region>().AddAsync(brand);
                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllRegionsCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, _localizer["Region Saved"]);

            }
            else
            {
                var brand = await _unitOfWork.Repository<Region>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    brand.Name = command.Name ?? brand.Name;
                    brand.Description = command.Description ?? brand.Description;
                    brand.StationId = command.StationId.HasValue? command.StationId.Value:0;
                    await _unitOfWork.Repository<Region>().UpdateAsync(brand);
                    //await _unitOfWork.Commit(cancellationToken);

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllRegionsCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["Region Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Region Not Found!"]);
                }
            }
        }
    }
}