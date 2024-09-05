using Bogus;
using CashierService.Application;
using CashierService.Application.Commands;
using CashierService.Core.Entities;
using CashierService.Core.Enums;
using CashierService.Core.Interfaces;
using CashierService.Tests.Cache;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace CashierService.Tests.Services
{
    public class CashFlowServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CashFlowService _cashFlowService;
        private readonly Mock<ILogger<CashFlowService>> _logger;
        private readonly Faker _faker;
    
        private readonly FakeMemoryCache _cache;
        public CashFlowServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockMediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<CashFlowService>>();
            _faker = new Faker();
            _cache = new FakeMemoryCache();
            _cashFlowService = new CashFlowService(_mockMediator.Object, _logger.Object, _cache );
        }
        [Fact(DisplayName = "Cria transação com sucesso.")]
        public async Task AddTransactionAsync_AddsTransactionSuccessfully()
        {
            // Arrange
            var amount = _faker.Finance.Amount();
            var type = GenerateRandomTransactionType();
            var command = new CreateTransactionCommand(amount, type);

            // Act
            await _cashFlowService.AddTransactionAsync(amount, type);

            // Assert
            _mockMediator.Verify(mediator => mediator.Send(It.Is<CreateTransactionCommand>(c => c.Amount == amount && c.Type == type), default), Times.Once);
        }

        [Fact(DisplayName = "Lança exceção quando o valor da transação é inválido.")]
        public async Task AddTransactionAsync_ThrowsException_WhenAmountIsInvalid()
        {
            // Arrange
            var invalidAmount = -_faker.Finance.Amount();
            var type = GenerateRandomTransactionType();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _cashFlowService.AddTransactionAsync(invalidAmount, type));

            // Verificar se a mensagem da exceção contém o texto esperado
            Assert.Contains("amount must be greater than zero", exception.Message, StringComparison.OrdinalIgnoreCase);
        }



        [Fact(DisplayName = "Obtém saldo consolidado diário com sucesso.")]
        public async Task GetDailyConsolidatedBalanceAsync_ReturnsCorrectBalance_WhenTransactionsPresent()
        {
            // Arrange
            var date = _faker.Date.Recent();
            var expectedBalance = _faker.Finance.Amount();
            object cacheKey = date;

            _cache.TryGetValue(cacheKey, out _);

            var cacheEntry = _cache.CreateEntry(cacheKey);
            cacheEntry.Value = expectedBalance;

            _mockMediator
                .Setup(mediator => mediator.Send(It.Is<ConsolidateDailyBalanceCommand>(c => c.Date == date), default))
                .ReturnsAsync(expectedBalance);

            // Act
            var result = await _cashFlowService.GetDailyConsolidatedBalanceAsync(date);

            // Assert
            Assert.Equal(expectedBalance, result);
            _mockMediator.Verify(mediator => mediator.Send(It.Is<ConsolidateDailyBalanceCommand>(c => c.Date == date), default), Times.Once);
        }


        [Fact(DisplayName = "Retorna zero quando não há transações")]
        public async Task GetDailyConsolidatedBalanceAsync_ReturnsZero_WhenNoTransactions()
        {
            // Arrange
            var testDate = _faker.Date.Recent();
            var transactions = new List<Transaction>();

            var expectedBalance = 0m;

            _mockTransactionRepository
                .Setup(repo => repo.GetByDateAsync(testDate))
                .ReturnsAsync(transactions);

            // Act
            var result = await _cashFlowService.GetDailyConsolidatedBalanceAsync(testDate);

            // Assert
            Assert.Equal(expectedBalance, result);
        }

        [Fact]
        public async Task GetDailyConsolidatedBalanceAsync_ThrowsException_WhenDateIsInFuture()
        {
            // Arrange
            var futureDate = _faker.Date.Future(); 

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _cashFlowService.GetDailyConsolidatedBalanceAsync(futureDate));

            // Verificar se a mensagem da exceção contém o texto esperado
            Assert.Contains("The date cannot be in the future", exception.Message, StringComparison.OrdinalIgnoreCase);
        }


        [Fact(DisplayName = "Propaga exceção do Mediator.")]
        public async Task AddTransactionAsync_PropagatesMediatorException()
        {
            // Arrange
            var amount = _faker.Finance.Amount();
            var type = GenerateRandomTransactionType();
            var exception = new InvalidOperationException("Mediator exception");
            _mockMediator.Setup(m => m.Send(It.IsAny<CreateTransactionCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(exception);

            // Act & Assert
            var resultException = await Assert.ThrowsAsync<InvalidOperationException>(() => _cashFlowService.AddTransactionAsync(amount, type));
            Assert.Equal(exception.Message, resultException.Message);
        }

        private static ETransactionType GenerateRandomTransactionType()
        {
            var values = Enum.GetValues(typeof(ETransactionType)).Cast<ETransactionType>();
            return values.ElementAt(new Random().Next(values.Count()));
        }

    }

}
