using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.Reports.BalanceSheet;
using SJP.Accounting.Application.Reports.PartnerSettlement;
using SJP.Accounting.Application.Services;
using SJP.Accounting.Domain.Models;

namespace SJP.Accounting.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITransactionService, TransactionImportService>();
            services.AddScoped<IAccountingQueryService, AccountingQueryService>();
            services.AddScoped<ISJPAccountingReport, PartnerSettlementReport>();
            services.AddScoped<ISJPAccountingReport, BalanceSheetReport>();


            return services;
        }
    }
}
