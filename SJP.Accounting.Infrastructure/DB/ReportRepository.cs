using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJP.Accounting.Domain.Contracts;
using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Infrastructure.DB;

public sealed class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _dbContext;

    private readonly ILogger<ReportRepository> _logger;

    public ReportRepository(ApplicationDbContext dbContext, ILogger<ReportRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading dashboard report data.");

        return await _dbContext.Dashboard.AsNoTracking().FirstOrDefaultAsync(cancellationToken) ?? new DashboardViewModel();
    }

    public async Task<List<ProjectProfitabilityViewModel>> GetProjectProfitabilityAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading project profitability report data.");

        return await _dbContext.ProjectProfitability
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PartnerCapitalPositionViewModel>> GetPartnerCapitalPositionAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading partner capital position report data.");

        return await _dbContext.PartnerCapitalPositions
            .AsNoTracking()
            .OrderBy(x => x.Partner)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SettlementStatusViewModel>> GetSettlementStatusAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading settlement status report data.");

        return await _dbContext.SettlementStatus
            .AsNoTracking()
            .OrderBy(x => x.Partner)
            .ToListAsync(cancellationToken);
    }

    public async Task<SettlementRecommendationViewModel?> GetSettlementRecommendationAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading settlement recommendation report data.");

        return await _dbContext.SettlementRecommendations
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<AllTransactionViewModel>> GetAllTransactionsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading transaction ledger report data.");

        var result = await _dbContext.AllTransactions
            .AsNoTracking()
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Loaded {Count} transactions.", result.Count);

        return result;
    }
}