using SJP.Accounting.Application.DTOs;

namespace SJP.Accounting.Application.Contracts
{
    public interface ITransactionSourceReader
    {
        Task<IEnumerable<TransactionImportDto>> ReadAsync(string source, CancellationToken cancellationToken);
    }
}
