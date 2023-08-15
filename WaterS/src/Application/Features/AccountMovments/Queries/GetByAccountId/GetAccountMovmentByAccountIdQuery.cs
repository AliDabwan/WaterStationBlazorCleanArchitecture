using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Application.Interfaces.Repositories;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Features.AccountMovments.Queries.GetByAccountId
{
    public class GetAccountMovmentByAccountIdQuery : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public GetAccountMovmentByAccountIdQuery(int AccountId)
        {
            Id = AccountId;
        }
    }
  
    internal class GetAccountMovmentByAccountIdQueryHandler : IRequestHandler<GetAccountMovmentByAccountIdQuery, Result<string>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetAccountMovmentByAccountIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }   

        public async Task<Result<string>> Handle(GetAccountMovmentByAccountIdQuery query, CancellationToken cancellationToken)
        {
            var myMovment = await _unitOfWork.Repository<AccTransMovment>().Entities.Where(x => x.AccountsId == query.Id).ToListAsync();

            decimal debit = myMovment.Sum(x => x.DebitAmmount);
            decimal credit = myMovment.Sum(x => x.CreditAmmount);

            decimal balance = debit - credit;


            return await Result<string>.SuccessAsync(balance.ToString("N0"));
        }
    }
}