using Xunit;
using Moq;

using EquityPositionBackend.Repositories;
using EquityPositionBackend.services;

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
            
        }
    }
}