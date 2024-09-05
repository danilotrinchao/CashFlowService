using CashierService.Core.Entities;
using MediatR;

namespace CashierService.Application.Queries
{
    public class GetDailySummaryQuery : IRequest<DailySummary>
    {
        public DateTime Date { get; set; }
    }
}
