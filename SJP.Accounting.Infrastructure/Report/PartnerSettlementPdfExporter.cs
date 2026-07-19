using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.PartnerSettlement;

public sealed class PartnerSettlementPdfExporter : IReportExporter<PartnerSettlementReportDocument>
{
    public string ExportType => "Pdf";

    public async Task<Stream> ExportAsync(
        PartnerSettlementReportDocument document,
        CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        QuestPDF.Settings.License =
            LicenseType.Community;

        Document
            .Create(container =>
            {
                BuildPage1(container, document);

                BuildPage2(container, document);

                BuildPage3(container, document);

                BuildLedgerPages(container, document);
            })
            .GeneratePdf(stream);

        stream.Position = 0;

        return await Task.FromResult(stream);
    }

    private static void BuildPage1(
    IDocumentContainer container,
    PartnerSettlementReportDocument document)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);

            page.Margin(20);

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Text(document.ReportName)
                        .Bold()
                        .FontSize(20);

                    column.Item()
                        .Text($"Generated On : {document.GeneratedOn:dd-MMM-yyyy HH:mm}");

                    column.Item()
                        .Text(string.Empty)
                        .Bold()
                        .FontSize(14);
                });

            page.Content()
                .Column(column =>
                {
                    column.Spacing(15);
                    BuildDashboard(column, document);
                    BuildCapitalPosition(column, document);
                    BuildPartnerEquity(column, document);
                    BuildEquityExplanation(column);

                });

            page.Footer()
                .AlignRight()
                .Text("Page 1");
        });
    }

    private static void BuildPartnerEquity(
    ColumnDescriptor column,
    PartnerSettlementReportDocument document)
    {
        column.Item()
            .Text("Partner Equity Details")
            .Bold()
            .FontSize(14);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddHeader(table, "Partner");
                AddHeader(table, "Contribution");
                AddHeader(table, "Profit Share");
                AddHeader(table, "Remaining Equity");

                foreach (var item in document.SettlementStatus)
                {
                    AddCell(table, item.Partner);

                    AddCell(
                        table,
                        Currency(item.ActualContribution));

                    AddCell(
                        table,
                        Currency(item.ExpectedProfitShare));

                    AddCell(
                        table,
                        Currency(item.CapitalPosition));
                }
            });
    }

    private static void BuildEquityExplanation(
    ColumnDescriptor column)
    {
        column.Item()
            .Text("Equity Explanation")
            .Bold()
            .FontSize(14);

        column.Item()
            .Border(1)
            .Padding(10)
            .Background(Colors.Grey.Lighten4)
            .Text(
                "Remaining Equity represents each partner's share " +
                "of business value after allocating profits and losses. " +
                "It is not cash available for withdrawal and does not " +
                "indicate any pending settlement amount. Settlement " +
                "obligations are determined solely from contribution " +
                "variance shown in the Settlement Status section.");
    }

    private static void BuildPage2(
    IDocumentContainer container,
    PartnerSettlementReportDocument document)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);

            page.Margin(20);

            page.Header()
                .Text("Settlement Analysis")
                .Bold()
                .FontSize(18);

            page.Content()
                .Column(column =>
                {
                    column.Spacing(15);

                    BuildSettlementStatus(
                        column,
                        document);

                    BuildSettlementRecommendation(
                        column,
                        document);

                    BuildExecutiveSummary(
                        column,
                        document);
                });

            page.Footer()
                .AlignRight()
                .Text("Page 3");
        });
    }

    private static void BuildPage3(
    IDocumentContainer container,
    PartnerSettlementReportDocument document)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);

            page.Margin(20);

            page.Header()
                .Text("Project Profitability")
                .Bold()
                .FontSize(18);

            page.Content()
                .Column(column =>
                {
                    BuildProjectProfitability(
                        column,
                        document);
                });

            page.Footer()
                .AlignRight()
                .Text("Page 2");
        });
    }

    private static void BuildLedgerPages(IDocumentContainer container, PartnerSettlementReportDocument document)
    {
        if (!document.Transactions.Any())
        {
            return;
        }

        container.Page(page =>
        {
            page.Size(PageSizes.A4);

            page.Margin(20);

            page.Header()
                .Column(column =>
                {
                    column.Item()
                        .Text("Transaction Ledger")
                        .FontSize(18)
                        .Bold();

                    column.Item()
                        .Text(string.Empty)
                        .FontSize(10);

                    column.Item()
                        .Text(
                            $"Total Transactions : {document.Transactions.Count}")
                        .FontSize(10);
                });

            page.Content()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.4f); // Date
                        columns.RelativeColumn(1.8f); // Project
                        columns.RelativeColumn(1.8f); // Category
                        columns.RelativeColumn(1.6f); // Type
                        columns.RelativeColumn(1.2f); // Amount
                        columns.RelativeColumn(1.7f); // Paid By
                        columns.RelativeColumn(1.7f); // Received By
                        columns.RelativeColumn(3.0f); // Narration
                    });

                    table.Header(header =>
                    {
                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Date")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Project")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Category")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Type")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Amount")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Paid By")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Received By")
                            .Bold();

                        header.Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Border(1)
                            .Padding(5)
                            .Text("Narration")
                            .Bold();
                    });

                    foreach (var transaction in document.Transactions
                                 .OrderByDescending(x => x.TransactionDate)
                                 .ThenByDescending(x => x.ImportedOn))
                    {
                        table.Cell()
                            .Border(1)
                            .Padding(2)
                            .Text(
                                transaction.TransactionDate
                                    .ToString("dd-MMM-yyyy"))
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(transaction.ProjectName ?? String.Empty)
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(
                                transaction.CategoryName
                                ?? String.Empty)
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(
                                transaction.TransactionTypeName)
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(
                                Currency(transaction.Amount))
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(
                                transaction.PaidBy
                                ?? String.Empty)
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(
                                transaction.ReceivedBy
                                ?? String.Empty)
                            .FontSize(7);

                        table.Cell()
                            .Border(1)
                            .Padding(4)
                            .Text(
                                transaction.Narration
                                ?? String.Empty)
                            .FontSize(7);
                    }
                });

            page.Footer()
                .AlignRight()
                .Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                });
        });
    }

    private static void BuildDashboard(ColumnDescriptor column, PartnerSettlementReportDocument document)
    {
        var dashboard =
            document.Dashboard;

        column.Item()
            .Text("Dashboard")
            .Bold()
            .FontSize(14);

        column.Item()
            .Row(row =>
            {
                row.RelativeItem()
                    .Border(1)
                    .Padding(8)
                    .Text($"Contribution\n\n{Currency(dashboard.TotalContribution)}");

                row.RelativeItem()
                    .Border(1)
                    .Padding(8)
                    .Text($"Income\n\n{Currency(dashboard.TotalIncome)}");

                row.RelativeItem()
                    .Border(1)
                    .Padding(8)
                    .Text($"Expense\n\n{Currency(dashboard.TotalExpense)}");

                row.RelativeItem()
                    .Border(1)
                    .Padding(8)
                    .Text($"Assets\n\n{Currency(dashboard.TotalAssetPurchase)}");

                row.RelativeItem()
                    .Border(1)
                    .Padding(8)
                    .Text($"Profit/Loss\n\n{Currency(dashboard.NetProfitLoss)}");

                row.RelativeItem()
                    .Border(1)
                    .Padding(8)
                    .Text($"Fund Balance\n\n{Currency(dashboard.FundBalance)}");
            });
    }

    private static void BuildCapitalPosition(
    ColumnDescriptor column,
    PartnerSettlementReportDocument document)
    {
        column.Item()
            .Text("Partner Capital Position")
            .Bold()
            .FontSize(14);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddHeader(table, "Partner");
                AddHeader(table, "Investment");
                AddHeader(table, "Expense Funding");
                AddHeader(table, "Asset Funding");
                AddHeader(table, "Withdrawal");
                AddHeader(table, "Settlement Paid");
                AddHeader(table, "Settlement Received");
                AddHeader(table, "Contribution");

                foreach (var item in document.CapitalPositions)
                {
                    AddCell(table, item.Partner);
                    AddCell(table, Currency(item.Investment));
                    AddCell(table, Currency(item.ExpenseFunding));
                    AddCell(table, Currency(item.AssetFunding));
                    AddCell(table, Currency(item.Withdrawal));
                    AddCell(table, Currency(item.SettlementPaid));
                    AddCell(table, Currency(item.SettlementReceived));
                    AddCell(table, Currency(item.Contribution));
                }
            });
    }

    private static void BuildProjectProfitability(
    ColumnDescriptor column,
    PartnerSettlementReportDocument document)
    {
        column.Item()
            .Text(string.Empty)
            .Bold()
            .FontSize(14);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddHeader(table, "Project");
                AddHeader(table, "Income");
                AddHeader(table, "Expense");
                AddHeader(table, "Profit / Loss");

                foreach (var item in document.ProjectProfitability)
                {
                    AddCell(table, item.ProjectName);
                    AddCell(table, Currency(item.Income));
                    AddCell(table, Currency(item.Expense));
                    AddCell(table, Currency(item.ProfitLoss));
                }
            });
    }

    private static void BuildSettlementStatus(
    ColumnDescriptor column,
    PartnerSettlementReportDocument document)
    {
        column.Item()
            .Text(string.Empty)
            .Bold()
            .FontSize(14);

        column.Item()
            .Text("Settlement Status")
            .Bold()
            .FontSize(14);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Partner
                    columns.RelativeColumn();   // Own %
                    columns.RelativeColumn();   // Actual %
                    columns.RelativeColumn();   // Contribution
                    columns.RelativeColumn();   // Expected
                    columns.RelativeColumn();   // Variance
                    columns.RelativeColumn();   // Status
                });

                AddHeader(table, "Partner");
                AddHeader(table, "Own %");
                AddHeader(table, "Actual %");
                AddHeader(table, "Contribution");
                AddHeader(table, "Expected");
                AddHeader(table, "Variance");
                AddHeader(table, "Status");

                foreach (var item in document.SettlementStatus)
                {
                    AddCell(table, item.Partner);

                    AddCell(
                        table,
                        item.OwnershipPercentage.ToString("N2"));

                    AddCell(
                        table,
                        item.ContributionPercentage.ToString("N2"));

                    AddCell(
                        table,
                        Currency(item.ActualContribution));

                    AddCell(
                        table,
                        Currency(item.ExpectedContribution));

                    AddCell(
                        table,
                        Currency(item.ContributionVariance));

                    AddCell(
                        table,
                        item.FundingStatus);
                }
            });
    }

    private static void BuildSettlementRecommendation(
    ColumnDescriptor column,
    PartnerSettlementReportDocument document)
    {
        if (document.SettlementRecommendation == null)
            return;

        var recommendation =
            document.SettlementRecommendation;

        column.Item()
            .Text("Settlement Recommendation")
            .Bold()
            .FontSize(14);

        column.Item()
            .Border(1)
            .Padding(10)
            .Background(Colors.Blue.Lighten5)
            .Text(
                $"{recommendation.PayingPartner} should pay " +
                $"{recommendation.ReceivingPartner} an amount of " +
                $"{Currency(recommendation.SettlementAmount)}")
            .Bold()
            .FontSize(14);
    }

    private static void BuildExecutiveSummary(
    ColumnDescriptor column,
    PartnerSettlementReportDocument document)
    {
        var recommendation =
            document.SettlementRecommendation;

        if (recommendation == null)
            return;

        var totalProfitLoss =
            document.SettlementStatus
                .FirstOrDefault()?
                .TotalProfitLoss ?? 0;

        column.Item()
            .Text("Executive Summary")
            .Bold()
            .FontSize(14);

        column.Item()
            .PaddingTop(5)
            .Text(
                $"Total business profit/loss for the reporting period is " +
                $"{Currency(totalProfitLoss)}.\n\n" +

                $"This report summarizes partner contributions, project profitability " +
                $"and settlement obligations based on current business activity.\n\n" +

                $"{recommendation.PayingPartner} should pay " +
                $"{recommendation.ReceivingPartner} an amount of " +
                $"{Currency(recommendation.SettlementAmount)}.");
    }

    private static void AddHeader(
    TableDescriptor table,
    string text)
    {
        table.Cell()
            .Background(Colors.Grey.Lighten3)
            .Border(1)
            .Padding(5)
            .Text(text)
            .Bold()
            .FontSize(9);
    }

    private static void AddCell(
        TableDescriptor table,
        string text)
    {
        table.Cell()
            .Border(1)
            .Padding(4)
            .Text(text)
            .FontSize(8);
    }

    private static string Currency(decimal value)
    {
        return value.ToString("#,##0.00");
    }
}