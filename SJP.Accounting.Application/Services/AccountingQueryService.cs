using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Domain.Contracts;
using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Application.Services;

public sealed class AccountingQueryService : IAccountingQueryService
{
    private readonly IReportRepository _reportRepository;

    public AccountingQueryService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken)
    {
        return _reportRepository.GetDashboardAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectProfitabilityViewModel>> GetProjectProfitabilityAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository.GetProjectProfitabilityAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PartnerCapitalPositionViewModel>> GetPartnerCapitalPositionAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository .GetPartnerCapitalPositionAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementStatusViewModel>> GetSettlementStatusAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository .GetSettlementStatusAsync(cancellationToken);
    }

    public Task<SettlementRecommendationViewModel> GetSettlementRecommendationAsync(CancellationToken cancellationToken)
    {
        return _reportRepository .GetSettlementRecommendationAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AllTransactionViewModel>> GetTransactionsAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository.GetAllTransactionsAsync(cancellationToken);
    }
}