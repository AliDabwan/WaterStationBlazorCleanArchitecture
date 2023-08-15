using AutoMapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Extensions;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Application.Specifications.Catalog;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Application;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountMovments.Queries.GetAll
{
    public class GetAllAccountMovmentsQuery : IRequest<PaginatedResult<GetAllAccountMovmentsResponse>>
    {
        public int accountId { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...
        public GetAllAccountMovmentsQuery()
        {
          
        }
        public GetAllAccountMovmentsQuery(int pageNumber, int pageSize, int accountId, string searchString, string orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            this.accountId = accountId;

            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllAccountMovmentsQueryHandler : IRequestHandler<GetAllAccountMovmentsQuery, PaginatedResult<GetAllAccountMovmentsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllAccountMovmentsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        //public async Task<Result<List<GetAllAccountMovmentsResponse>>> Handle(GetAllAccountMovmentsQuery request, CancellationToken cancellationToken)
        public async Task<PaginatedResult<GetAllAccountMovmentsResponse>> Handle(GetAllAccountMovmentsQuery request, CancellationToken cancellationToken)

        {
            //Func<Task<List<AccTransMovment>>> getAllBottleTypes = () =>  _unitOfWork.Repository<AccTransMovment>().GetAllAsync();

            Expression<Func<AccTransMovment, GetAllAccountMovmentsResponse>> expression = e => new GetAllAccountMovmentsResponse
            {
                Id = e.Id,
                AccTransId=e.AccTransId,
                EntryType = e.EntryType,
                AccountsId = e.AccountsId,
                CreditAmmount= e.CreditAmmount,
                DebitAmmount = e.DebitAmmount,
                DriverId = e.DriverId,
                CompanyId = e.CompanyId,
                StationId = e.StationId,
               NoteDebit = e.NoteDebit,
                NoteCredit = e.NoteCredit,
                Note = e.Note,
                
                CreatedOn = e.CreatedOn,
               Accounts=e.Accounts
              

                //CustomerList = e.CustomerList
            };

            var AccountMovmentsFilterSpec = new AccountMovmentsFilterSpecification();

            var data = await _unitOfWork.Repository<AccTransMovment>().Entities.Specify(AccountMovmentsFilterSpec)
               .Select(expression).Where(x=>x.AccountsId==request.accountId)
               .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return data;

            //var brandList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllAccountMovmentsCacheKey, getAllBottleTypes);
            //var mappedBottleTypes = _mapper.Map<List<GetAllAccountMovmentsResponse>>(brandList);
            //return await Result<List<GetAllAccountMovmentsResponse>>.SuccessAsync(mappedBottleTypes);
        }
    }
}