using EquityPositionBackend.Models;
using EquityPositionBackend.Repositories;
using EquityPositionBackend.services;
using Microsoft.AspNetCore.Mvc;

namespace EquityPositionBackend.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            // Remove any TransactionID coming from client
            transaction.TransactionID = 0; // Ensure it's set to 0 for auto-generation

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _transactionService.AddTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.TransactionID }, transaction);
        }
    }
}
