using SJP.Accounting.Application.DTOs;
namespace SJP.Accounting.Application.Contracts;

public interface ITransactionService
{
    Task<ImportResultDto> ProcessAsync(IEnumerable<TransactionImportDto> transactions, CancellationToken cancellationToken);
}