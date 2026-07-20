using Microsoft.AspNetCore.Mvc;
using SJP.Accounting.Application.Contracts;

namespace SJP.Accounting.API.Controllers
{
    [ApiController]
    [Route("api/v1/dashboard")]
    public sealed class DashboardController : ControllerBase
    {
        private readonly IAccountingQueryService _queryService;

        public DashboardController(IAccountingQueryService queryService)
            => _queryService = queryService;

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            return Ok(await _queryService.GetDashboardAsync(cancellationToken));
        }

        [HttpGet("project-profitability")]
        public async Task<IActionResult> GetProjectProfitability(CancellationToken cancellationToken)
        {
            return Ok(await _queryService.GetProjectProfitabilityAsync(cancellationToken));
        }
    }
}
