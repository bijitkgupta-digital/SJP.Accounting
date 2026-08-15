using Microsoft.AspNetCore.Mvc;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.DTOs;

[ApiController]
[Route("api/v1/transactions")]
public sealed class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ITransactionSourceReader _reader;

    public TransactionController(ITransactionService transactionService, ITransactionSourceReader reader)
    {
        _transactionService = transactionService;
        _reader = reader;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionImportDto transaction, CancellationToken cancellationToken)
    {
        var result = await _transactionService.ProcessAsync([transaction], cancellationToken);

        return Ok(result);
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Excel file is required.");

        var extension = Path.GetExtension(file.FileName);

        if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx files are supported.");

        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{extension}");

        try
        {
            await using (var stream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var transactions = await _reader.ReadAsync(tempFilePath, cancellationToken);

            var result = await _transactionService.ProcessAsync(transactions, cancellationToken);
            if (result.ImportErrors.Any())
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Transaction import validation error."
                });
            }

            return Ok(result);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            //_logger.LogWarning("Transaction Excel import was cancelled.");
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Unexpected error while importing transaction Excel file {FileName}.", file.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred while importing the transaction file."
                });
        }
        finally
        {
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }
    }
}