using CashierService.Core.Entities;
using CashierService.Core.Interfaces;
using Dapper;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Transactions;
using Transaction = CashierService.Core.Entities.Transaction;

namespace CashierService.Infra.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _connection;

        public TransactionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            var query = "SELECT * FROM Transactions";

            return await _connection.QueryAsync<Transaction>(query);
        }

        public async Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date)
        {
            var query = "SELECT * FROM transaction WHERE DATE(date) = @Date";
            return await _connection.QueryAsync<Transaction>(query, new { Date = date.Date });
        }

        public async Task AddAsync(Transaction transaction)
        {
            var query = @"
            INSERT INTO ""transaction"" (""id"", amount, etransactiontype, date) 
            VALUES (@Id, @Amount, @eTransactionType, @Date)";

            await _connection.ExecuteAsync(query, transaction);
        }

        public async Task<decimal> GetDailySummaryAsync(DateTime date)
        {
            var query = @"
            SELECT SUM(CASE WHEN Type = 'Credit' THEN amount ELSE -amount END) 
            FROM ""transaction"" 
            WHERE  ""date""::date = @Date";

            return await _connection.ExecuteScalarAsync<decimal>(query, new { Date = date });
        }


    }

}
