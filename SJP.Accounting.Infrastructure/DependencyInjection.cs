using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.BalanceSheet;
using SJP.Accounting.Application.Reports.PartnerSettlement;
using SJP.Accounting.Domain.Contracts;
using SJP.Accounting.Infrastructure.DB;
using SJP.Accounting.Infrastructure.Report;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SJP_Accounting")));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ITransactionSourceReader, ExcelTransactionSourceReader>();
        services.AddScoped<IReportExporter<PartnerSettlementReportDocument>, PartnerSettlementPdfExporter>();
        services.AddScoped<IReportExporter<BalanceSheetReportDocument>, BalanceSheetPdfExporter>();

        return services;
    }
}