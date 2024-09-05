using CashierService.Core.Enums;

namespace CashierService.Core.Interfaces
{
    public interface ICashFlowService
    {
        Task AddTransactionAsync(decimal amount, ETransactionType type);
        Task<decimal> GetDailyConsolidatedBalanceAsync(DateTime date);
    }

}
