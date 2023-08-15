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

namespace WaterS.Application.Features.AccountMovments.Commands.AddEdit
{
    public partial class AddEditAccountMovmentCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int EntryType { get; set; }//,
        public int refId { get; set; }//,
        public string Note{ get; set; }

        public string NoteDebit { get; set; }
        public string NoteCredit { get; set; }

        public decimal DebitAmmount { get; set; }//debit=0
        public decimal CreditAmmount { get; set; }//credit=0
        public decimal Ammount { get; set; }//credit=0

        public int AccTransId { get; set; }//
        public int debitAccountId { get; set; }//

        public int creditAccountId { get; set; }//
        public int CompanyId { get; set; }
        public int DriverId { get; set; }

        public int StationId { get; set; }

        public string userId { get; set; }

    }

    internal class AddEditAccountMovmentCommandHandler : IRequestHandler<AddEditAccountMovmentCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditAccountMovmentCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditAccountMovmentCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditAccountMovmentCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditAccountMovmentCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {

                //var accTrans = _mapper.Map<AccTransMovment>(command);


                var myacc = await _unitOfWork.Repository<Domain.Entities.Catalog.Accounts>().GetByIdAsync(command.creditAccountId);

                var myVoucherTrans = new AccTrans()
                    {
                        TransType = 3,
                        RefId = command.refId,
                        Note = command.Note,
                        Ammount = command.DebitAmmount,// Talap.Price,
                        Transby = command.userId,
                        UserId = command.userId,
                        CompanyId = command.CompanyId,
                        StationId = command.StationId==0? myacc.StationId: command.StationId

                    };


                    var resultVoucherTrans = await _unitOfWork.Repository<AccTrans>().AddAsync(myVoucherTrans);
                    await _unitOfWork.Commit(cancellationToken);


                if (myacc==null)
                {
                    return await Result<int>.FailAsync(_localizer["لم يتم جلب الحساب بنجاح يرجى المحاولة مرة اخرى"]);
                }

                if (resultVoucherTrans.Id > 0)
                    {
                        //debit

                        var myVoucherTransDebit = new AccTransMovment()
                        {

                            EntryType = 1,
                            Note = command.Note,
                            DebitAmmount = command.DebitAmmount,
                            CreditAmmount = 0,
                            AccountsId = command.debitAccountId,
                            AccTransId = myVoucherTrans.Id,
                        NoteDebit = command.NoteDebit + myacc.Name,
                            CompanyId = command.CompanyId,
                            StationId = command.StationId,
                            DriverId = command.DriverId,


                        };


                        var resultmyTransDebit = await _unitOfWork.Repository<AccTransMovment>().AddAsync(myVoucherTransDebit);
                        await _unitOfWork.Commit(cancellationToken);



                        //credit
                        if (resultmyTransDebit.Id > 0)
                        {
                            var myVoucherTransCredit = new AccTransMovment()
                            {

                                EntryType = 2,
                                Note = command.Note,
                                CreditAmmount = command.CreditAmmount,
                                DebitAmmount = 0,
                                AccountsId = command.creditAccountId,
                                AccTransId = myVoucherTrans.Id,
                                NoteCredit = command.NoteCredit,
                                
                                CompanyId = command.CompanyId,
                                StationId = command.StationId,
                                DriverId = command.DriverId,


                            };

                            var resultmyTransCredit = await _unitOfWork.Repository<AccTransMovment>().AddAsync(myVoucherTransCredit);
                            //await _unitOfWork.Commit(cancellationToken);
                        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountMovmentsCacheKey);

                    }



                }


                return await Result<int>.SuccessAsync( _localizer["AccountMovment Done"]);










                //await _unitOfWork.Commit(cancellationToken);



            }
            else
            {
                var brand = await _unitOfWork.Repository<AccTransMovment>().GetByIdAsync(command.Id);
                if (brand != null)
                {
                    brand.Note = command.Note ?? brand.Note;
                    brand.DebitAmmount = command.DebitAmmount;
                    brand.CreditAmmount = command.CreditAmmount;
                    await _unitOfWork.Repository<AccTransMovment>().UpdateAsync(brand);
                    //await _unitOfWork.Commit(cancellationToken);

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountMovmentsCacheKey);
                    return await Result<int>.SuccessAsync(brand.Id, _localizer["AccountMovment Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["AccountMovment Not Found!"]);
                }
            }
        }
    }
}