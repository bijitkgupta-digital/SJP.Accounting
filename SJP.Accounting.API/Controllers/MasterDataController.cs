using Microsoft.AspNetCore.Mvc;
using SJP.Accounting.Application.Contracts;

namespace SJP.Accounting.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class MasterDataController : ControllerBase
{
    private readonly IAccountingQueryService _queryService;

    public MasterDataController(IAccountingQueryService queryService)
        => _queryService = queryService;

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
    {
        return Ok(await _queryService.GetProjectsAsync(cancellationToken));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        return Ok(await _queryService.GetCategoriesAsync(cancellationToken));
    }

    [HttpGet("entities")]
    public async Task<IActionResult> GetEntities(CancellationToken cancellationToken)
    {
        return Ok(await _queryService.GetEntitiesAsync(cancellationToken));
    }

    [HttpGet("transaction-types")]
    public async Task<IActionResult> GetTransactionTypes(CancellationToken cancellationToken)
    {
        return Ok(await _queryService.GetTransactionTypesAsync(cancellationToken));
    }
}