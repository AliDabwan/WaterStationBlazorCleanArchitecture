using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.CustomerPhones.Commands.Delete
{
    public class DeleteCustomerPhoneCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteCustomerPhoneCommandHandler : IRequestHandler<DeleteCustomerPhoneCommand, Result<int>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<DeleteCustomerPhoneCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteCustomerPhoneCommandHandler(IUnitOfWork<int> unitOfWork, IProductRepository productRepository, IStringLocalizer<DeleteCustomerPhoneCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteCustomerPhoneCommand command, CancellationToken cancellationToken)
        {
            //var isBrandUsed = await _productRepository.IsBrandUsed(command.Id);
            //if (!isBrandUsed)
            //{
                var brand = await _unitOfWork.Repository<CustomerPhone>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    await _unitOfWork.Repository<CustomerPhone>().DeleteAsync(brand);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBottletypesCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["تم حذف السجل"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["السجل غير موجود"]);
                }
            //}
            //else
            //{
            //    return await Result<int>.FailAsync(_localizer["Deletion Not Allowed"]);
            //}
        }
    }
}