using EquityPositionBackend.Models;

namespace EquityPositionBackend.services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Position>> GetAllPositionsAsync();
    }
}

