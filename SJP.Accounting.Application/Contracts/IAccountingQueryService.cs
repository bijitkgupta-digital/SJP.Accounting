using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Application.Contracts;

public interface IAccountingQueryService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectProfitabilityViewModel>> GetProjectProfitabilityAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<PartnerCapitalPositionViewModel>> GetPartnerCapitalPositionAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<SettlementStatusViewModel>> GetSettlementStatusAsync(CancellationToken cancellationToken);

    Task<SettlementRecommendationViewModel> GetSettlementRecommendationAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<AllTransactionViewModel>> GetTransactionsAsync(CancellationToken cancellationToken);
}