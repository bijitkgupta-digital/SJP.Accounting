using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Domain.Contracts;

public interface IReportRepository
{
    Task<List<AllTransactionViewModel>> GetAllTransactionsAsync(CancellationToken cancellationToken);
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken);
    Task<List<PartnerCapitalPositionViewModel>> GetPartnerCapitalPositionAsync(CancellationToken cancellationToken);
    Task<List<ProjectProfitabilityViewModel>> GetProjectProfitabilityAsync(CancellationToken cancellationToken);
    Task<SettlementRecommendationViewModel?> GetSettlementRecommendationAsync(CancellationToken cancellationToken);
    Task<List<SettlementStatusViewModel>> GetSettlementStatusAsync(CancellationToken cancellationToken);
}