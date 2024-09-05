using CashierService.Core.Entities;

namespace CashierService.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<decimal> GetDailySummaryAsync(DateTime date);
        Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date);
    }
}
