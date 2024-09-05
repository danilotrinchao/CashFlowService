using CashierService.Core.Enums;
using MediatR;

namespace CashierService.Application.Commands
{
    public record CreateTransactionCommand(decimal Amount, ETransactionType Type) : IRequest<Guid>;
}
