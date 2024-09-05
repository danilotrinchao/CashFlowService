using CashierService.Core.Entities;

namespace CashierService.Core.Interfaces
{
    public interface IDailySummaryRepository
    {
        Task<DailySummary> GetByDateAsync(DateTime date);
    }
}
