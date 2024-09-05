using CashierService.Application.Queries;
using CashierService.Core.Entities;
using CashierService.Core.Interfaces;
using MediatR;

namespace CashierService.Application.QueriesHandler
{
    public class GetDailySummaryQueryHandler : IRequestHandler<GetDailySummaryQuery, DailySummary>
    {
        private readonly IDailySummaryRepository _repository;

        public GetDailySummaryQueryHandler(IDailySummaryRepository repository)
        {
            _repository = repository;
        }

        public async Task<DailySummary> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByDateAsync(request.Date);
        }
    }
}
