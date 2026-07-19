using Microsoft.EntityFrameworkCore;
using SJP.Accounting.Domain.Entities;
using SJP.Accounting.Domain.ViewModels;

namespace SJP.Accounting.Infrastructure.DB;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region Tables

    public DbSet<Partner> Partners { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Entity> Entities { get; set; }

    public DbSet<TransactionType> TransactionTypes { get; set; }

    public DbSet<TransactionMaster> TransactionMasters { get; set; }

    public DbSet<Asset> Assets { get; set; }

    #endregion

    #region Views

    public DbSet<AllTransactionViewModel> AllTransactions { get; set; }
    public DbSet<DashboardViewModel> Dashboard { get; set; }
    public DbSet<ProjectProfitabilityViewModel> ProjectProfitability { get; set; }
    public DbSet<PartnerCapitalPositionViewModel> PartnerCapitalPositions { get; set; }
    public DbSet<SettlementStatusViewModel> SettlementStatus { get; set; }
    public DbSet<SettlementRecommendationViewModel> SettlementRecommendations { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTables(modelBuilder);

        ConfigureViews(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Partner>()
            .HasKey(x => x.PartnerId);

        modelBuilder.Entity<Project>()
            .HasKey(x => x.ProjectId);

        modelBuilder.Entity<Category>()
            .HasKey(x => x.CategoryId);

        modelBuilder.Entity<Entity>()
            .HasKey(x => x.EntityId);

        modelBuilder.Entity<TransactionType>()
            .HasKey(x => x.TransactionTypeId);

        modelBuilder.Entity<TransactionMaster>()
            .HasKey(x => x.TransactionId);

        modelBuilder.Entity<Asset>()
            .HasKey(x => x.AssetId);

        // existing code below...
        modelBuilder.Entity<Project>()
            .HasMany(x => x.Transactions)
            .WithOne(x => x.Project)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Category>()
            .HasMany(x => x.Transactions)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TransactionType>()
            .HasMany(x => x.Transactions)
            .WithOne(x => x.TransactionType)
            .HasForeignKey(x => x.TransactionTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TransactionMaster>()
            .HasIndex(x => x.TransactionHash)
            .IsUnique();

        modelBuilder.Entity<Asset>()
            .HasOne(x => x.Transaction)
            .WithMany(x => x.Assets)
            .HasForeignKey(x => x.TransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Entity>()
            .HasIndex(x => x.EntityName)
            .IsUnique();

        modelBuilder.Entity<TransactionMaster>()
            .HasOne(x => x.PaidByEntity)
            .WithMany(x => x.PaidTransactions)
            .HasForeignKey(x => x.PaidByEntityId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TransactionMaster>()
            .HasOne(x => x.ReceivedByEntity)
            .WithMany(x => x.ReceivedTransactions)
            .HasForeignKey(x => x.ReceivedByEntityId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureViews(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllTransactionViewModel>()
            .HasNoKey()
            .ToView("vw_AllTransactions");

        modelBuilder.Entity<DashboardViewModel>()
            .HasNoKey()
            .ToView("vw_Dashboard");

        modelBuilder.Entity<ProjectProfitabilityViewModel>()
            .HasNoKey()
            .ToView("vw_ProjectProfitability");

        modelBuilder.Entity<PartnerCapitalPositionViewModel>()
            .HasNoKey()
            .ToView("vw_PartnerCapitalPosition");

        modelBuilder.Entity<SettlementStatusViewModel>()
            .HasNoKey()
            .ToView("vw_SettlementStatus");

        modelBuilder.Entity<SettlementRecommendationViewModel>()
            .HasNoKey()
            .ToView("vw_SettlementRecommendation");
    }
}