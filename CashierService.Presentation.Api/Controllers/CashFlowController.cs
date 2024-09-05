using CashierService.Application.Commands;
using CashierService.Core.Enums;
using CashierService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace CashierService.Presentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashFlowController : ControllerBase
    {
        private readonly ICashFlowService _cashFlowService;

        public CashFlowController(ICashFlowService cashFlowService)
        {
            _cashFlowService = cashFlowService;
        }

        /// <summary>
        /// Endpoint para criar um lançamento (débito ou crédito).
        /// </summary>
        /// <param name="amount">Valor do lançamento.</param>
        /// <param name="type">Tipo do lançamento: Débito ou Crédito.</param>
        /// <returns>Retorna um status 200 em caso de sucesso.</returns>
        [HttpPost("transactions")]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand createTransaction)
        {
            await _cashFlowService.AddTransactionAsync(createTransaction.Amount, createTransaction.Type);
            return Ok("Lançamento criado com sucesso.");
        }

        /// <summary>
        /// Endpoint para obter o saldo consolidado diário.
        /// </summary>
        /// <param name="date">Data para a consolidação.</param>
        /// <returns>Retorna o saldo consolidado do dia especificado.</returns>
        [HttpGet("balance")]
        public async Task<IActionResult> GetDailyConsolidatedBalance([FromQuery] string date)
        {
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", new CultureInfo("pt-BR"), DateTimeStyles.None, out var parsedDate))
            {
                var transactions = await _cashFlowService.GetDailyConsolidatedBalanceAsync(parsedDate);
                return Ok(transactions);
            }

            return BadRequest("Formato de data inválido. Utilize o formato dd/MM/yyyy.");
        }
    }
}
