using CashierService.Application.Commands;
using CashierService.Core.Enums;
using CashierService.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;

namespace CashierService.Application
{
    public class CashFlowService: ICashFlowService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CashFlowService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IAsyncPolicy<decimal> _fallbackPolicy;
        private const decimal NotAvailableValue = decimal.MinValue;
        public CashFlowService(IMediator mediator, ILogger<CashFlowService> logger, IMemoryCache cache)
        {
            _mediator = mediator;
            _logger = logger;
            _cache = cache;
            _fallbackPolicy = Policy<decimal>
            .Handle<Exception>()
            .FallbackAsync(
                fallbackValue: NotAvailableValue,
                onFallbackAsync: async (exception, context) =>
                {
                    _logger.LogError(exception.Result.ToString(), "Failed to retrieve consolidated balance. Returning NotAvailableValue.");
                    await Task.CompletedTask; // Assegura que o método assíncrono complete sua execução
                });
        }

        public async Task AddTransactionAsync(decimal amount, ETransactionType type)
        {
            if (amount <= 0)
            {
                _logger.LogWarning("Attempted to add a transaction with non-positive amount: {Amount}", amount);
                throw new ArgumentException("The transaction amount must be greater than zero.", nameof(amount));
            }

            var command = new CreateTransactionCommand(amount, type);

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Transaction added successfully. Amount: {Amount}, Type: {Type}", amount, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding transaction. Amount: {Amount}, Type: {Type}", amount, type);
                throw;
            }
        }

        public async Task<decimal> GetDailyConsolidatedBalanceAsync(DateTime date)
        {
          
            if (date.Date > DateTime.Now.Date)
            {
                _logger.LogWarning("Attempted to get daily consolidated balance for a future date: {Date}", date);
                throw new ArgumentException("The date cannot be in the future.", nameof(date));
            }

     
            if (_cache.TryGetValue(date, out decimal cachedBalance))
            {
                _logger.LogInformation("Retrieved balance from cache for date {Date}. Balance: {Balance}", date, cachedBalance);
                return cachedBalance;
            }

            var command = new ConsolidateDailyBalanceCommand(date);

            decimal balance;
            try
            {
                balance = await _fallbackPolicy.ExecuteAsync(() => _mediator.Send(command));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily consolidated balance for date {Date} using fallback policy", date);
                throw;
            }

            if (balance != NotAvailableValue)
            {
                _cache.Set(date, balance, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                _logger.LogInformation("Stored balance in cache for date {Date}. Balance: {Balance}", date, balance);
            }
            else
            {
                _logger.LogWarning("Fallback value used for date {Date}. No valid balance was returned.", date);
            }

            return balance;
        }

    }
}
