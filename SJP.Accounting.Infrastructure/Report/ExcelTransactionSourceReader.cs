using ClosedXML.Excel;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.DTOs;

namespace SJP.Accounting.Infrastructure.Report;

public sealed class ExcelTransactionSourceReader : ITransactionSourceReader
{
    public async Task<IEnumerable<TransactionImportDto>> ReadAsync(string source, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var transactions =
                new List<TransactionImportDto>();

            using var stream = new FileStream(
                source,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            using var workbook = new XLWorkbook(stream);

            var worksheet =
                workbook.Worksheet(1);

            var rows =
                worksheet.RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                transactions.Add(
                    new TransactionImportDto
                    {
                        TransactionDate =
                            row.Cell(1).GetDateTime(),

                        ProjectName =
                            row.Cell(2).GetString(),

                        CategoryName =
                            row.Cell(3).GetString(),

                        TransactionType =
                            row.Cell(4).GetString(),

                        Amount =
                            row.Cell(5).GetValue<decimal>(),

                        PaidByType =
                            row.Cell(6).GetString(),

                        PaidBy =
                            row.Cell(7).GetString(),

                        ReceivedByType =
                            row.Cell(8).GetString(),

                        ReceivedBy =
                            row.Cell(9).GetString(),

                        Narration =
                            row.Cell(10).GetString(),

                        GoogleDriveLink =
                            row.Cell(11).GetString()
                    });
            }

            return transactions.AsEnumerable();

        }, cancellationToken);
    }
}