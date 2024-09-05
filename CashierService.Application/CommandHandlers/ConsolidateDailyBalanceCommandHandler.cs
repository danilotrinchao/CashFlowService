using CashierService.Application.Commands;
using CashierService.Core.Enums;
using CashierService.Core.Interfaces;
using MediatR;

namespace CashierService.Application.CommandHandlers
{
    public class ConsolidateDailyBalanceCommandHandler : IRequestHandler<ConsolidateDailyBalanceCommand, decimal>
    {
        private readonly ITransactionRepository _repository;

        public ConsolidateDailyBalanceCommandHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> Handle(ConsolidateDailyBalanceCommand request, CancellationToken cancellationToken)
        {
            var transactions = await _repository.GetByDateAsync(request.Date);
            var balance = transactions
                .Where(t => t.eTransactionType == ETransactionType.Credit).Sum(t => t.Amount) -
                transactions.Where(t => t.eTransactionType == ETransactionType.Debit).Sum(t => t.Amount);

            return balance;
        }
    }
}
