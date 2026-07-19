using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.BalanceSheet;

namespace SJP.Accounting.Infrastructure.Report;

public sealed class BalanceSheetPdfExporter
    : IReportExporter<BalanceSheetReportDocument>
{
    public string ExportType => "Pdf";

    public async Task<Stream> ExportAsync(
        BalanceSheetReportDocument document,
        CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        QuestPDF.Settings.License =
            LicenseType.Community;

        QuestPDF.Fluent.Document
            .Create(container =>
            {
                BuildBalanceSheetPage(container, document);
                BuildSupportingSchedulesPage(container, document);
            })
            .GeneratePdf(stream);

        stream.Position = 0;

        return await Task.FromResult(stream);
    }

    private static void BuildSupportingSchedulesPage(
    IDocumentContainer container,
    BalanceSheetReportDocument document)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);

            page.Margin(20);

            page.Header()
                .Text("Supporting Schedules")
                .Bold()
                .FontSize(18);

            page.Content()
                .Column(column =>
                {
                    column.Spacing(20);

                    BuildFundReconciliation(
                        column,
                        document);

                    BuildPartnerEquityDetails(
                        column,
                        document);
                });

            page.Footer()
                .AlignRight()
                .Text("Page 2");
        });
    }

    private static void BuildBalanceSheetPage(
        IDocumentContainer container,
        BalanceSheetReportDocument document)
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
                        .Text(
                            $"As On : {document.GeneratedOn:dd-MMM-yyyy}");
                });

            page.Content()
                .Column(column =>
                {
                    column.Spacing(15);

                    column.Item()
                        .Text("")
                        .FontSize(5);

                    column.Item()
                        .Text("Assets")
                        .Bold()
                        .FontSize(16);

                    BuildAssetsTable(
                        column,
                        document);

                    column.Item()
                        .Text("Liabilities")
                        .Bold()
                        .FontSize(16);

                    BuildLiabilitiesTable(
                        column);

                    column.Item()
                        .Text("Partner Equity")
                        .Bold()
                        .FontSize(16);

                    BuildEquityTable(
                        column,
                        document);

                    column.Item()
                        .Text("Balance Verification")
                        .Bold()
                        .FontSize(16);

                    BuildBalanceVerification(
                        column,
                        document);

                    BuildNotes(
                        column);
                });

            page.Footer()
                .AlignRight()
                .Text("Page 1");
        });
    }

    private static void BuildFundReconciliation(
    ColumnDescriptor column,
    BalanceSheetReportDocument document)
    {
        column.Item()
            .Text("Fund Reconciliation")
            .Bold()
            .FontSize(16);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                });

                AddHeader(table, "Particulars");
                AddHeader(table, "Amount");

                AddCell(table, "Total Contributions");
                AddCell(table,
                    Currency(document.Dashboard.TotalContribution));

                AddCell(table, "Add : Total Income");
                AddCell(table,
                    Currency(document.Dashboard.TotalIncome));

                AddCell(table, "Less : Total Expense");
                AddCell(table,
                    Currency(document.Dashboard.TotalExpense));

                AddCell(table, "Less : Asset Purchase");
                AddCell(table,
                    Currency(document.Dashboard.TotalAssetPurchase));

                AddCell(table, "Fund Balance");
                AddCell(table,
                    Currency(document.Dashboard.FundBalance));
            });
    }

    private static void BuildPartnerEquityDetails(
    ColumnDescriptor column,
    BalanceSheetReportDocument document)
    {
        column.Item()
            .Text("Partner Equity Details")
            .Bold()
            .FontSize(16);

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

                foreach (var item in document.PartnerEquity)
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

    private static void BuildAssetsTable(
        ColumnDescriptor column,
        BalanceSheetReportDocument document)
    {
        var totalAssets =
            document.Dashboard.FundBalance +
            document.Dashboard.TotalAssetPurchase;

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                });

                AddHeader(table, "Particulars");
                AddHeader(table, "Amount");

                AddCell(table, "Fund Balance");
                AddCell(
                    table,
                    Currency(
                        document.Dashboard.FundBalance));

                AddCell(table, "Fixed Assets");
                AddCell(
                    table,
                    Currency(
                        document.Dashboard.TotalAssetPurchase));

                AddCell(table, "Total Assets");
                AddCell(
                    table,
                    Currency(totalAssets));
            });
    }

    private static void BuildLiabilitiesTable(
        ColumnDescriptor column)
    {
        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                });

                AddHeader(table, "Particulars");
                AddHeader(table, "Amount");

                AddCell(table, "External Liabilities");
                AddCell(table, Currency(0));

                AddCell(table, "Total Liabilities");
                AddCell(table, Currency(0));
            });
    }

    private static void BuildEquityTable(
        ColumnDescriptor column,
        BalanceSheetReportDocument document)
    {
        var totalEquity =
            document.PartnerEquity
                .Sum(x => x.CapitalPosition);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                });

                AddHeader(table, "Partner");
                AddHeader(table, "Equity");

                foreach (var item in document.PartnerEquity)
                {
                    AddCell(table, item.Partner);

                    AddCell(
                        table,
                        Currency(
                            item.CapitalPosition));
                }

                AddCell(table, "Total Equity");

                AddCell(
                    table,
                    Currency(totalEquity));
            });
    }

    private static void BuildBalanceVerification(
        ColumnDescriptor column,
        BalanceSheetReportDocument document)
    {
        var assets =
            document.Dashboard.FundBalance +
            document.Dashboard.TotalAssetPurchase;

        var liabilities = 0m;

        var equity =
            document.PartnerEquity
                .Sum(x => x.CapitalPosition);

        var difference =
            assets - (liabilities + equity);

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                });

                AddHeader(table, "Particulars");
                AddHeader(table, "Amount");

                AddCell(table, "Total Assets");
                AddCell(table, Currency(assets));

                AddCell(table, "Total Liabilities");
                AddCell(table, Currency(liabilities));

                AddCell(table, "Total Equity");
                AddCell(table, Currency(equity));

                AddCell(
                    table,
                    "Liabilities + Equity");

                AddCell(
                    table,
                    Currency(liabilities + equity));

                AddCell(table, "Difference");
                AddCell(table, Currency(difference));
            });

        column.Item()
            .PaddingTop(10)
            .Text(
                difference == 0
                    ? "✓ Balance Sheet Balanced"
                    : "⚠ Balance Sheet Out Of Balance")
            .Bold()
            .FontColor(
                difference == 0
                    ? Colors.Green.Darken2
                    : Colors.Red.Darken2);
    }

    private static void BuildNotes(
        ColumnDescriptor column)
    {
        column.Item()
            .Text("Notes")
            .Bold()
            .FontSize(10);

        column.Item()
            .Border(1)
            .Padding(8)
            .Background(Colors.Grey.Lighten4)
            .Text(
                "Fund Balance represents cash or funds expected to remain " +
                "after considering partner contributions, income, expenses " +
                "and asset purchases.\n\n" +

                "Partner Equity represents the business value attributable " +
                "to each partner after allocating profits and losses.\n\n" +

                "This report is intended for management purposes and provides " +
                "a snapshot of the financial position of the business as on " +
                "the report date.")
            .FontSize(8);
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
            .Bold();
    }

    private static void AddCell(
        TableDescriptor table,
        string text)
    {
        table.Cell()
            .Border(1)
            .Padding(4)
            .Text(text)
            .FontSize(9);

    }

    private static string Currency(
        decimal value)
    {
        return value.ToString("#,##0.00");
    }
}