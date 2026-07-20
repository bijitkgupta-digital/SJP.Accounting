using Microsoft.AspNetCore.Mvc;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.DTOs;

[ApiController]
[Route("api/v1/transactions")]
public sealed class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionImportDto transaction, CancellationToken cancellationToken)
    {
        var result = await _transactionService.ProcessAsync([transaction], cancellationToken);

        return Ok(result);
    }
}