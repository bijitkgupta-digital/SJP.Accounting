using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Domain.Contracts;
using SJP.Accounting.Domain.Entities;
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
        return await _reportRepository.GetPartnerCapitalPositionAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SettlementStatusViewModel>> GetSettlementStatusAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository.GetSettlementStatusAsync(cancellationToken);
    }

    public async Task<SettlementRecommendationViewModel> GetSettlementRecommendationAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository.GetSettlementRecommendationAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AllTransactionViewModel>> GetTransactionsAsync(CancellationToken cancellationToken)
    {
        return await _reportRepository.GetAllTransactionsAsync(cancellationToken);
    }

    public Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken cancellationToken)
    {

        return _reportRepository.GetProjectsAsync(cancellationToken);

    }

    public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {

        return _reportRepository.GetCategoriesAsync(cancellationToken);

    }

    public Task<IReadOnlyList<Entity>> GetEntitiesAsync(CancellationToken cancellationToken)
    {

        return _reportRepository.GetEntitiesAsync(cancellationToken);

    }

    public Task<IReadOnlyList<TransactionType>> GetTransactionTypesAsync(CancellationToken cancellationToken)
    {

        return _reportRepository.GetTransactionTypesAsync(cancellationToken);

    }
}