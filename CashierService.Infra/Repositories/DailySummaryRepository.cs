using CashierService.Core.Entities;
using CashierService.Core.Interfaces;
using Dapper;
using Npgsql;
using System.Data;

namespace CashierService.Infra.Repositories
{
    public class DailySummaryRepository : IDailySummaryRepository
    {
        private readonly IDbConnection _connection;

        public DailySummaryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<DailySummary> GetByDateAsync(DateTime date)
        {
            var query = @"
            SELECT Date, TotalCredits, TotalDebits, Balance 
            FROM DailySummaries 
            WHERE Date = @Date";

            return await _connection.QueryFirstOrDefaultAsync<DailySummary>(query, new { Date = date });
        }
        public async Task<int> UpsertDailySummaryAsync(DailySummary dailySummary)
        {
            var query = @"
            INSERT INTO DailySummaries (Date, TotalCredits, TotalDebits, Balance) 
            VALUES (@Date, @TotalCredits, @TotalDebits, @Balance)
            ON CONFLICT (Date) 
            DO UPDATE SET 
                TotalCredits = EXCLUDED.TotalCredits,
                TotalDebits = EXCLUDED.TotalDebits,
                Balance = EXCLUDED.Balance";

            return await _connection.ExecuteAsync(query, dailySummary);
        }
    }
}

