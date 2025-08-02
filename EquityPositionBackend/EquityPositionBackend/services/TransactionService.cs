using EquityPositionBackend.Models;
using EquityPositionBackend.Repositories;

namespace EquityPositionBackend.services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _repository.GetAllTransactionsAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _repository.GetTransactionByIdAsync(id);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _repository.AddTransactionAsync(transaction);
            await _repository.UpdatePositionAsync(transaction);
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _repository.GetAllPositionsAsync();
        }

        public async Task UpdatePositionAsync(Transaction transaction)
        {
            await _repository.UpdatePositionAsync(transaction);
        }
    }
}
