using Xunit;

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EquityPositionBackend.Data;
using EquityPositionBackend.Repositories;
using EquityPositionBackend.Models;

namespace EquityPositionManager.Tests.Repositories
{
    public class TransactionRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TransactionRepository _repository;

        public TransactionRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new TransactionRepository(_context);
        }

        [Fact]
        public async Task AddTransactionAsync_ShouldAddTransaction()
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
            await _repository.AddTransactionAsync(transaction);
            var result = await _context.Transactions.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("REL", result.SecurityCode);
        }

        [Fact]
        public async Task RecalculateAllPositions_ShouldCorrectlyCalculateNetPositions()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { TradeID = 1, Version = 1, SecurityCode = "REL", Quantity = 50, ActionType = "INSERT", TradeType = "Buy" },
                new Transaction { TradeID = 1, Version = 2, SecurityCode = "REL", Quantity = 60, ActionType = "UPDATE", TradeType = "Buy" },
                new Transaction { TradeID = 2, Version = 1, SecurityCode = "ITC", Quantity = 40, ActionType = "INSERT", TradeType = "Sell" },
                new Transaction { TradeID = 2, Version = 2, SecurityCode = "ITC", Quantity = 30, ActionType = "CANCEL", TradeType = "Buy" },
                new Transaction { TradeID = 3, Version = 1, SecurityCode = "INF", Quantity = 70, ActionType = "INSERT", TradeType = "Buy" },
                new Transaction { TradeID = 4, Version = 1, SecurityCode = "INF", Quantity = 20, ActionType = "INSERT", TradeType = "Sell" }
            };

            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();

            // Act
            await _repository.RecalculateAllPositions();
            var positions = await _repository.GetAllPositionsAsync();

            // Assert
            Assert.Equal(3, positions.Count());
            Assert.Contains(positions, p => p.SecurityCode == "REL" && p.Quantity == 60);
            Assert.Contains(positions, p => p.SecurityCode == "ITC" && p.Quantity == 0);
            Assert.Contains(positions, p => p.SecurityCode == "INF" && p.Quantity == 50);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}