using Xunit;
using Moq;
using EquityPositionBackend.services;
using EquityPositionBackend.Repositories;
using EquityPositionBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


namespace EquityPositionManager.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockRepo;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _mockRepo = new Mock<ITransactionRepository>();
            _service = new TransactionService(_mockRepo.Object);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldCallRepositoryMethods()
        {
            // Arrange
            var transaction = new Transaction
            {
                TradeID = 1,
                Version = 1,
                SecurityCode = "REL",
                Quantity = 50,
                ActionType = "INSERT",
                TradeType = "Buy"
            };

            // Act
            await _service.AddTransactionAsync(transaction);

            // Assert
            _mockRepo.Verify(x => x.AddTransactionAsync(transaction), Times.Once);
            _mockRepo.Verify(x => x.UpdatePositionAsync(transaction), Times.Once);
        }

        [Fact]
        public async Task GetAllPositionsAsync_ShouldReturnPositions()
        {
            // Arrange
            var positions = new List<Position>
            {
                new Position { SecurityCode = "REL", Quantity = 50 },
                new Position { SecurityCode = "ITC", Quantity = -40 }
            };
            _mockRepo.Setup(x => x.GetAllPositionsAsync()).ReturnsAsync(positions);

            // Act
            var result = await _service.GetAllPositionsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.SecurityCode == "REL" && p.Quantity == 50);
            Assert.Contains(result, p => p.SecurityCode == "ITC" && p.Quantity == -40);
        }

        [Fact]
        public async Task UpdatePositionAsync_WithUpdateAction_ShouldRecalculatePositions()
        {
            // Arrange
            var updateTransaction = new Transaction
            {
                TransactionID = 4,
                TradeID = 1,
                Version = 2,
                SecurityCode = "REL",
                Quantity = 60,
                ActionType = "UPDATE",
                TradeType = "Buy"
            };

            var allTransactions = new List<Transaction>
    {
        new Transaction
        {
            TransactionID = 1,
            TradeID = 1,
            Version = 1,
            SecurityCode = "REL",
            Quantity = 50,
            ActionType = "INSERT",
            TradeType = "Buy"
        },
        updateTransaction
    };

            _mockRepo.Setup(x => x.GetAllTransactionsAsync())
                    .ReturnsAsync(allTransactions);

            // Act
            await _service.UpdatePositionAsync(updateTransaction);

            // Assert
            _mockRepo.Verify(x => x.RecalculateAllPositions(), Times.Once);
        }
    }
}