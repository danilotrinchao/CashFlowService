using CashierService.Application.Commands;
using CashierService.Core.Entities;
using CashierService.Core.Interfaces;
using MediatR;

namespace CashierService.Application.CommandHandlers
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
    {
        private readonly ITransactionRepository _repository;

        public CreateTransactionCommandHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                eTransactionType = request.Type,
                Date = DateTime.Now
            };

            await _repository.AddAsync(transaction);
            return transaction.Id;
        }
    }

}
