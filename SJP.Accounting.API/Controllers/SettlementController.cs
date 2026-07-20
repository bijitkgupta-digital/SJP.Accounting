using Microsoft.AspNetCore.Mvc;
using SJP.Accounting.Application.Contracts;

namespace SJP.Accounting.API.Controllers
{
    [ApiController]
    [Route("api/v1/settlement")]
    public sealed class SettlementController : ControllerBase
    {
        private readonly IAccountingQueryService _queryService;

        public SettlementController(IAccountingQueryService queryService)
            => _queryService = queryService;

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
        {
            return Ok(await _queryService.GetSettlementStatusAsync(cancellationToken));
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> GetRecommendation(CancellationToken cancellationToken)
        {
            return Ok(await _queryService.GetSettlementRecommendationAsync(cancellationToken));
        }

        [HttpGet("capital-position")]
        public async Task<IActionResult> GetCapitalPosition(CancellationToken cancellationToken)
        {
            return Ok(await _queryService.GetPartnerCapitalPositionAsync(cancellationToken));
        }
    }
}
