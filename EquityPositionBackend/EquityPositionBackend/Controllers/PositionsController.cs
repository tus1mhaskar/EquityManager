using EquityPositionBackend.services;
using Microsoft.AspNetCore.Mvc;

namespace EquityPositionBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public PositionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPositions()
        {
            var positions = await _transactionService.GetAllPositionsAsync();
            return Ok(positions);
        }
    }
}
