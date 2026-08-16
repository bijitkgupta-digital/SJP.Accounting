using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SJP.Accounting.Application;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.BalanceSheet;
using SJP.Accounting.Application.Reports.PartnerSettlement;
using SJP.Accounting.Infrastructure.Report;

namespace SJP.Accounting.CLI;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            if (args.Length == 0)
            {
                await ExecuteSyncCommandAsync(host.Services, "C:\\bijitkgupta-digital@github.com\\SJP.Accounting\\Swijit_Transaction_Template.xlsx");
                await ExecuteReportCommandAsync(host.Services);
            }
            else
            {
                var command = args[0].ToLowerInvariant();
                //sync "C:\Bijit\SJP\Swijit_Transaction_Template.xlsx"
                //report

                switch (command)
                {
                    case "sync":
                        if (args.Length < 2)
                        {
                            logger.LogError("Excel file path is required.");
                            return -1;
                        }
                        await ExecuteSyncCommandAsync(host.Services, args[1]);
                        break;
                    case "report":
                        await ExecuteReportCommandAsync(host.Services);
                        break;
                    default:
                        logger.LogError("Unknown command : {Command}", command);
                        PrintUsage();
                        return -1;
                }
            }
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application execution failed.");
            return -1;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
                {
                    services.AddApplicationServices(context.Configuration);
                    services.AddInfrastructureServices(context.Configuration);
                    services.AddScoped<ITransactionSourceReader, ExcelTransactionSourceReader>();
                    services.AddScoped<IReportExporter<PartnerSettlementReportDocument>, PartnerSettlementPdfExporter>();
                    services.AddScoped<IReportExporter<BalanceSheetReportDocument>, BalanceSheetPdfExporter>();
                    services.AddScoped<ISJPAccountingReport, PartnerSettlementReport>();
                    services.AddScoped<ISJPAccountingReport, BalanceSheetReport>();
                });
    }

    private static async Task ExecuteSyncCommandAsync(IServiceProvider serviceProvider, string filePath)
    {
        using var scope = serviceProvider.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var reader = scope.ServiceProvider.GetRequiredService<ITransactionSourceReader>();
        var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

        logger.LogInformation("Reading Excel file {FilePath}", filePath);
        var transactions = await reader.ReadAsync(filePath, CancellationToken.None);
        var result = await transactionService.ProcessAsync(transactions, CancellationToken.None);

        logger.LogInformation("Import Result : {Message}", result.Message);
        logger.LogInformation("Total Rows      : {TotalRows}", result.TotalRows);
        logger.LogInformation("Imported Rows   : {ImportedRows}", result.ImportedRows);
        logger.LogInformation("Duplicate Rows  : {DuplicateRows}", result.DuplicateRows);
        logger.LogInformation("Error Rows      : {ErrorRows}", result.ErrorRows);

        if (result.ImportErrors.Any())
        {
            logger.LogWarning("Import completed with validation errors.");

            foreach (var error in result.ImportErrors)
            {
                logger.LogWarning("Row:{Row}, Column:{Column}, Value:{Value}, Error:{Error}",
                    error.RowNumber,
                    error.ColumnName,
                    error.FieldValue,
                    error.ErrorMessage);
            }
        }
    }

    private static async Task ExecuteReportCommandAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting report generation.");
            var outputFolder = "C:\\Bijit\\SJP\\Reports";
            Directory.CreateDirectory(outputFolder);
            var reportList = scope.ServiceProvider.GetServices<ISJPAccountingReport>();

            foreach (var report in reportList)
            {
                var streamList = await report.ExportAsync(CancellationToken.None);
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var reportCode = report.ReportCode;

                foreach (var item in streamList)
                {
                    var extension = item.Key.ToLowerInvariant();
                    var fileName = $"{reportCode}_{timestamp}.{extension}";
                    var path = Path.Combine(outputFolder, fileName);
                    await using var file = File.Create(path);
                    await using var stream = item.Value;
                    stream.Position = 0;
                    await stream.CopyToAsync(file);
                    logger.LogInformation("Report exported : {Path}", path);
                }

                logger.LogInformation("Report generated successfully.");
                logger.LogInformation("Report Name : {ReportName}", report.ReportName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Report generation failed.");
            throw ex;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine();
        Console.WriteLine("SJP.Accounting.CLI sync <excel-file>");
        Console.WriteLine();
        Console.WriteLine("SJP.Accounting.CLI report");
        Console.WriteLine();
    }
}