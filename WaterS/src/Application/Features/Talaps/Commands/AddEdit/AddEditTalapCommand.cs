using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Interfaces.Services;
using WaterS.Application.Requests;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.Talaps.Commands.AddEdit
{
    public partial class AddEditTalapCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int No { get; set; }
        public string ServiceRate { get; set; }//Good//poor
        public string TalapStatue { get; set; }//Running-Complete-Declined
        public string TalapStatueAr { get; set; }//جاري-مكتمل-ملغي



        public string Comment { get; set; }//التعليق
        public string TalapDate { get; set; }
        public string TalapArrivalDate { get; set; }
        public string TalapArrivalTime { get; set; }


        public decimal Price { get; set; }
        public decimal Paid { get; set; }

        public string DoneByName { get; set; }

        public int DoneByAccountId { get; set; }



        public int BottleNo { get; set; }

        public int RegionId { get; set; }
        //public virtual Region Region { get; set; }
        public int CompanyId { get; set; }
        //public virtual Company Company { get; set; }

        public int StationId { get; set; }
        //public virtual Stationk Station { get; set; }

        public int DriverId { get; set; }
        //public virtual Driver Driver { get; set; }
        //[Required]

        public int CustomerId { get; set; }
        //public virtual Customer Customer { get; set; }
    }

    internal class AddEditTalapCommandHandler : IRequestHandler<AddEditTalapCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditTalapCommandHandler> _localizer; 
        private readonly Microsoft.AspNetCore.Identity.UserManager<BlazorHeroUser> userManager;
        private readonly IDateTimeService _dateTimeService;

        public AddEditTalapCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, IDateTimeService dateTimeService, UserManager<BlazorHeroUser> userManager, IStringLocalizer<AddEditTalapCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadService = uploadService;
            this.userManager = userManager;
            _localizer = localizer;
            _dateTimeService = dateTimeService;

        }

        public async Task<Result<int>> Handle(AddEditTalapCommand command, CancellationToken cancellationToken)
        {

            //if (await _unitOfWork.Repository<Talap>().Entities.Where(p => p.Id != command.Id)
            //    .AnyAsync(p => p.CompanyId == command.CompanyId && p.RegionId==command.RegionId, cancellationToken))
            //{
            //    return await Result<int>.FailAsync(_localizer["already exists."]);
            //}


            CultureInfo french = CultureInfo.GetCultureInfo("fr-FR");

            String nowStr = DateTime.Now.AddHours(3).ToString(french);
            //DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
            //DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
            //string result = Convert.ToDateTime("12/01/2011", usDtfi).ToString(ukDtfi.ShortDatePattern);



            if (command.Id == 0)
            {

                var talap = _mapper.Map<Talap>(command);

                var check = await _unitOfWork.Repository<Talap>().GetAllAsync();


                var checkBottleType = await _unitOfWork.Repository<Customer>().Entities.Where(x => x.BottleNo == talap.BottleNo &&x.StationId== talap.StationId&&x.statue!= "deleted").FirstOrDefaultAsync();

                if (checkBottleType == null)
                {
                    return await Result<int>.FailAsync(_localizer["الرجاء التأكد من رقم الخزان"]);
                }



             
                talap.RegionId = checkBottleType.RegionId;
                talap.TalapDate = nowStr;// _dateTimeService.NowUtc.Date.ToString("dd/MM/yyyy");

                var myTalapType = check.FindAll(x => x.CompanyId == talap.CompanyId);

                int maxvalue = int.MinValue;
                if (myTalapType == null || myTalapType.Count == 0)
                {
                    maxvalue = 0;
                }
                else
                {
                    foreach (var item in myTalapType)
                    {
                        if (item.No == talap.No && item.CustomerId == talap.CustomerId)
                        {
                            return await Result<int>.FailAsync(_localizer["Talap Repeated"]);

                        }
                        //if (item.TalapDate == talap.TalapDate && item.CustomerId == talap.CustomerId)
                        //{
                        //    return await Result<int>.FailAsync(_localizer["لقد تم إضافة الطلب مسبقا"]);

                        //}
                        if (item.TalapStatue == "Running" && item.CustomerId == talap.CustomerId)
                        {
                            return await Result<int>.FailAsync(_localizer["لايمكن إضافة طلب جديد ! يوجد طلب سابق في الإنتظار"]);

                        }
                        if (item.No > maxvalue)
                        {
                            maxvalue = item.No;

                        }
                    }
                }

                talap.No = maxvalue + 1;


                var customerr = await _unitOfWork.Repository<Customer>().GetByIdAsync(talap.CustomerId);
                if (customerr != null)
                {

                    var customRegion = await _unitOfWork.Repository<DriverRegion>().Entities.FirstOrDefaultAsync(x => x.StationId == customerr.StationId && x.RegionId == customerr.RegionId);
                    if (customRegion != null)
                    {
                      
                            customerr.DriverId = customRegion.DriverId;
                            command.DriverId = customRegion.DriverId;
                            await _unitOfWork.Repository<Customer>().UpdateAsync(customerr);
                            var result = await _unitOfWork.Commit();

                        

                    }

                }




                var custom = await _unitOfWork.Repository<Customer>().GetByIdAsync(talap.CustomerId);
                if (custom == null)
                {
                    return await Result<int>.FailAsync(_localizer["لاتوجد بيانات للعميل"]);

                }


                if (command.DriverId != custom.DriverId)
                {
                    return await Result<int>.FailAsync(_localizer["هذا الزبون تابع لسائق اخر ! لايمكن إضافة الطلب"]);

                }





                var bottle = await _unitOfWork.Repository<BottleType>().GetByIdAsync(custom.BottleTypeId);

                decimal myPrice = bottle.Price;
                //int filldays = bottle.FillDays;

                //var lastDate = custom.LastFillDate.Date;

                //var curentDays = (_dateTimeService.NowUtc.Date - lastDate).TotalDays;

                //if (filldays >= curentDays  )
                //{
                //    return await Result<int>.FailAsync(_localizer["لايمكنك الطلب قبل " + (filldays-curentDays) + " ايام ."]);

                //}
                if (myPrice <= 0)
                {
                    return await Result<int>.FailAsync(_localizer["مبلغ نوع الخزان هذا  صفر"]);

                }
                talap.Price = myPrice;
                var resulttalap = await _unitOfWork.Repository<Talap>().AddAsync(talap);



                //await _unitOfWork.Commit(cancellationToken);

                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllTalapsCacheKey);







                //await _unitOfWork.Rollback();

                return await Result<int>.SuccessAsync(talap.Id, _localizer["تم حفظ طلبك بنجاح"]);
                //return await Result<int>.SuccessAsync(talap.Id, _localizer["Talap Saved"]+ "lastDate : "+ lastDate
                //    + "  curentDays :"+ curentDays+ "  filldays :"+ filldays);
            }
            else  ///Here edit talap statue and compete or undo
            {
                var Talap = await _unitOfWork.Repository<Talap>().GetByIdAsync(command.Id);


                //var Talap = await _unitOfWork.Repository<Talap>().Entities.Where(p => p.Id == command.Id).FirstOrDefaultAsync(cancellationToken);

               

                if (Talap != null)
                {
                    Talap.CompanyId = command.CompanyId;
                    Talap.StationId = command.StationId;
                    Talap.DriverId = command.DriverId;
                    Talap.CustomerId = command.CustomerId;
                    Talap.RegionId = command.RegionId;
                    Talap.BottleNo = command.BottleNo;
                    Talap.No = command.No;
                    //Talap.TalapDate = command.TalapDate;
                    Talap.Price = command.Price;
                    Talap.DoneByAccountId = command.DoneByAccountId;
                    Talap.TalapArrivalDate = nowStr;// _dateTimeService.NowUtc.ToString("dd/MM/yyyy");
                    Talap.TalapArrivalTime = _dateTimeService.NowLocal.ToString("HH:MM:ss tt");

                    Talap.TalapStatue = command.TalapStatue;
                    Talap.ServiceRate = command.ServiceRate;
                    Talap.TalapStatueAr = command.TalapStatueAr;
                    Talap.Comment = command.Comment;


                    await _unitOfWork.Repository<Talap>().UpdateAsync(Talap);
                    //await _unitOfWork.Commit(cancellationToken);
                    if (Talap.TalapStatue == "Complete")
                    {


                        var custom = await _unitOfWork.Repository<Customer>().GetByIdAsync(command.CustomerId);
                        if (custom == null)
                        {
                            return await Result<int>.FailAsync(_localizer["لاتوجد بيانات للعميل"]);

                        }


                        var bottle = await _unitOfWork.Repository<BottleType>().GetByIdAsync(custom.BottleTypeId);

                        decimal myPrice = bottle.Price;


                        if (myPrice <= 0)
                        {
                            return await Result<int>.FailAsync(_localizer["مبلغ الوجبة صفر"]);

                        }


                        var myTrans = new AccTrans()
                        {
                            TransType = 2,
                            RefId = Talap.Id,
                            Note = " قيمة وجبة رقم : " + Talap.No + ".",
                            Ammount = myPrice,// Talap.Price,
                            Transby = Talap.DoneByName,
                            UserId = Talap.LastModifiedBy,
                            CompanyId = Talap.CompanyId,
                            StationId = Talap.StationId,


                        };


                        var resultAsTrans = await _unitOfWork.Repository<AccTrans>().AddAsync(myTrans);


                        await _unitOfWork.Commit(cancellationToken);



                        if (resultAsTrans.Id > 0)
                        {
                            //debit

                            var myTransDebit = new AccTransMovment()
                            {

                                EntryType = 1,
                                Note = " قيمة وجبة رقم : " + Talap.No + ".",
                                DebitAmmount = myPrice,
                                CreditAmmount = 0,
                                AccountsId = custom.AccountId,
                                AccTransId = myTrans.Id,
                            CompanyId = Talap.CompanyId,
                                StationId = Talap.StationId,
                                DriverId = command.DriverId


                            };
                       

                            var resultmyTransDebit = await _unitOfWork.Repository<AccTransMovment>().AddAsync(myTransDebit);
                            await _unitOfWork.Commit(cancellationToken);


                            //credit
                            if (resultmyTransDebit.Id > 0)
                            {
                                var myTransCredit = new AccTransMovment()
                                {

                                    EntryType = 2,
                                    Note = " قيمة وجبة رقم : " + Talap.No + ".",
                                    CreditAmmount = myPrice,
                                    DebitAmmount = 0,
                                    AccountsId = 72,
                                    AccTransId = myTrans.Id,
                                    
                                    CompanyId = Talap.CompanyId,
                                    StationId = Talap.StationId,
                                    DriverId = command.DriverId

                                };

                                var resultmyTransCredit = await _unitOfWork.Repository<AccTransMovment>().AddAsync(myTransCredit);
                                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAccountMovmentsCacheKey);

                                if (myTransCredit.Id > 0)
                                {
                                    custom.LastFillDate = _dateTimeService.NowLocal;
                                    await _unitOfWork.Repository<Customer>().UpdateAsync(custom);
                                    //await _unitOfWork.Commit(cancellationToken);
                                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllCustomersCacheKey);




                                }
                                else
                                {
                                    await _unitOfWork.Rollback();
                                    return await Result<int>.FailAsync(_localizer["@سيتم إلغاء العملية لم يتم ترحيل المبلغ"]);

                                }
                            }
                            else
                            {
                                await _unitOfWork.Rollback();
                                return await Result<int>.FailAsync(_localizer["سيتم إلغاء العملية لم يتم ترحيل المبلغ"]);

                            }

                         

                        }




                        if (command.Paid > 0)
                        {
                            var myVoucherTrans = new AccTrans()
                            {
                                TransType = 3,
                                RefId = Talap.Id,
                                Note = " دفع مبلغ عن الطلب رقم : " + Talap.No + ".",
                                Ammount = command.Paid,// Talap.Price,
                                Transby = Talap.DoneByName,
                                UserId = Talap.LastModifiedBy,
                                CompanyId = Talap.CompanyId,
                                StationId = Talap.StationId


                            };


                            var resultVoucherTrans = await _unitOfWork.Repository<AccTrans>().AddAsync(myVoucherTrans);
                            await _unitOfWork.Commit(cancellationToken);



                            if (resultVoucherTrans.Id > 0)
                            {
                                //debit

                                var myVoucherTransDebit = new AccTransMovment()
                                {

                                    EntryType = 1,
                                    Note = " استلام  من الزبون : " + custom.Name + " - خزان رقم : " + Talap.BottleNo + " .",
                                    DebitAmmount = command.Paid,
                                    CreditAmmount = 0,
                                    AccountsId = Talap.DoneByAccountId,
                                    AccTransId = myVoucherTrans.Id,

                                    CompanyId = Talap.CompanyId,
                                    StationId = Talap.StationId,
                                    DriverId = command.DriverId


                                };


                                var resultmyTransDebit = await _unitOfWork.Repository<AccTransMovment>().AddAsync(myVoucherTransDebit);
                                await _unitOfWork.Commit(cancellationToken);



                                //credit
                                if (resultmyTransDebit.Id > 0)
                                {
                                    var myVoucherTransCredit = new AccTransMovment()
                                    {

                                        EntryType = 2,
                                        Note = " دفع مبلغ عن الطلب رقم : " + Talap.No + ".",
                                        CreditAmmount = command.Paid,
                                        DebitAmmount = 0,
                                        AccountsId = custom.AccountId,
                                        AccTransId = myVoucherTrans.Id,

                                        CompanyId = Talap.CompanyId,
                                        StationId = Talap.StationId,
                                        DriverId = command.DriverId

                                    };

                                    var resultmyTransCredit = await _unitOfWork.Repository<AccTransMovment>().AddAsync(myVoucherTransCredit);
                                    await _unitOfWork.Commit(cancellationToken);
                                }



                            }



                        }



                        //  var myVoucher = new AccTrans()
                    }
                    if (Talap.TalapStatue == "Undo")
                    {
                        await _unitOfWork.Commit(cancellationToken);

                    }

                    return await Result<int>.SuccessAsync(Talap.Id, _localizer["Talap Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Talap Not Found!"]);
                }
            }
        }
    }
}