using EquityPositionBackend.Data;
using EquityPositionBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace EquityPositionBackend.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePositionAsync(Transaction transaction)
        {
            // Get all versions of this trade
            var tradeVersions = await _context.Transactions
                .Where(t => t.TradeID == transaction.TradeID)
                .OrderBy(t => t.Version)
                .ToListAsync();

            // Determine if this is the latest version
            bool isLatestVersion = tradeVersions.All(t => t.Version <= transaction.Version);

            // Only process if this is the latest version or if there's no newer version
            if (isLatestVersion)
            {
                // Calculate net effect for this trade
                int netQuantity = 0;
                string securityCode = transaction.SecurityCode;

                foreach (var version in tradeVersions)
                {
                    if (version.ActionType == "CANCEL")
                    {
                        netQuantity = 0;
                        break;
                    }

                    int quantityEffect = version.TradeType == "Buy" ? version.Quantity : -version.Quantity;
                    netQuantity += quantityEffect;
                    securityCode = version.SecurityCode; // In case security code changed in updates
                }

                // Update or create position
                var position = await _context.Positions
                    .FirstOrDefaultAsync(p => p.SecurityCode == securityCode);

                if (position == null)
                {
                    position = new Position
                    {
                        SecurityCode = securityCode,
                        Quantity = netQuantity,
                        LastUpdated = DateTime.UtcNow
                    };
                    await _context.Positions.AddAsync(position);
                }
                else
                {
                    // Recalculate all positions in case transactions arrived out of order
                    await RecalculateAllPositions();
                    return;
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task RecalculateAllPositions()
        {
            // Clear all positions
            _context.Positions.RemoveRange(_context.Positions);
            await _context.SaveChangesAsync();

            // Group transactions by TradeID and get the latest version of each trade
            var latestTransactions = await _context.Transactions
                .GroupBy(t => t.TradeID)
                .Select(g => g.OrderByDescending(t => t.Version).First())
                .ToListAsync();

            // Group by security code and calculate net quantity
            var positions = latestTransactions
                .Where(t => t.ActionType != "CANCEL")
                .GroupBy(t => t.SecurityCode)
                .Select(g => new Position
                {
                    SecurityCode = g.Key,
                    Quantity = g.Sum(t => t.TradeType == "Buy" ? t.Quantity : -t.Quantity),
                    LastUpdated = DateTime.UtcNow
                });

            await _context.Positions.AddRangeAsync(positions);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _context.Positions.ToListAsync();
        }

        Task ITransactionRepository.RecalculateAllPositions()
        {
            return RecalculateAllPositions();
        }
    }

}