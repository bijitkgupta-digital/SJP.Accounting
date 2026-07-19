using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Services;
using SJP.Accounting.Domain.Models;

namespace SJP.Accounting.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ReportConfiguration>(configuration.GetSection("ReportSettings"));

            services.AddScoped<ITransactionService, TransactionImportService>();
            //services.AddScoped<SJPAccountingReport<PartnerSettlementReportDocument>, PartnerSettlementReport>();
            
            return services;
        }
    }
}
