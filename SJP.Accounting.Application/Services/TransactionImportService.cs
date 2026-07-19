using Microsoft.Extensions.Logging;
using SJP.Accounting.Application.Contracts;
using SJP.Accounting.Application.DTOs;
using SJP.Accounting.Domain.Contracts;
using SJP.Accounting.Domain.Entities;

namespace SJP.Accounting.Application.Services;

public sealed class TransactionImportService : ITransactionService
{

    private readonly IRepository<TransactionMaster> _transactionRepository;
    private readonly IRepository<Entity> _entityRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<TransactionType> _transactionTypeRepository;
    private readonly ILogger<TransactionImportService> _logger;

    public TransactionImportService(
        IRepository<TransactionMaster> transactionRepository,
        IRepository<Entity> entityRepository,
        IRepository<Project> projectRepository,
        IRepository<Category> categoryRepository,
        IRepository<TransactionType> transactionTypeRepository,
        ILogger<TransactionImportService> logger)
    {
        _transactionRepository = transactionRepository;
        _entityRepository = entityRepository;
        _projectRepository = projectRepository;
        _categoryRepository = categoryRepository;
        _transactionTypeRepository = transactionTypeRepository;
        _logger = logger;
    }

    public async Task<ImportResultDto> ProcessAsync(
    IEnumerable<TransactionImportDto> transactions,
    CancellationToken cancellationToken)
    {
        var result = new ImportResultDto();

        try
        {
            var transactionList = transactions.ToList();

            result.TotalRows = transactionList.Count;

            var importedOn = DateTime.UtcNow;

            _logger.LogInformation(
                "Starting transaction processing. Total Rows : {TotalRows}",
                result.TotalRows);

            var projects = (await _projectRepository.ListAsync())
                .ToDictionary(
                    x => x.ProjectName,
                    StringComparer.OrdinalIgnoreCase);

            var categories = (await _categoryRepository.ListAsync())
                .ToDictionary(
                    x => x.CategoryName,
                    StringComparer.OrdinalIgnoreCase);

            var transactionTypes =
                (await _transactionTypeRepository.ListAsync())
                .ToDictionary(
                    x => x.TransactionTypeName,
                    StringComparer.OrdinalIgnoreCase);

            var entityCache =
                (await _entityRepository.ListAsync())
                .ToDictionary(
                    x => x.EntityName,
                    StringComparer.OrdinalIgnoreCase);

            var existingHashes =
                (await _transactionRepository.ListAsync())
                .Select(x => x.TransactionHash)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newEntities = new List<Entity>();

            var transactionsToInsert =
                new List<TransactionMaster>();

            int importedRows = 0;
            int duplicateRows = 0;

            for (int rowNumber = 1;
                 rowNumber <= transactionList.Count;
                 rowNumber++)
            {
                var transaction =
                    transactionList[rowNumber - 1];

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    #region Validation

                    if (string.IsNullOrWhiteSpace(transaction.ProjectName))
                    {
                        AddImportError(
                            result,
                            rowNumber,
                            nameof(transaction.ProjectName),
                            transaction.ProjectName,
                            "Project Name is mandatory.");

                        continue;
                    }

                    if (transaction.Amount <= 0)
                    {
                        AddImportError(
                            result,
                            rowNumber,
                            nameof(transaction.Amount),
                            transaction.Amount.ToString(),
                            "Amount must be greater than zero.");

                        continue;
                    }

                    //if (string.IsNullOrWhiteSpace(transaction.PaidBy))
                    //{
                    //    AddImportError(
                    //        result,
                    //        rowNumber,
                    //        nameof(transaction.PaidBy),
                    //        transaction.PaidBy,
                    //        "Paid By is mandatory.");

                    //    continue;
                    //}

                    //if (string.IsNullOrWhiteSpace(transaction.ReceivedBy))
                    //{
                    //    AddImportError(
                    //        result,
                    //        rowNumber,
                    //        nameof(transaction.ReceivedBy),
                    //        transaction.ReceivedBy,
                    //        "Received By is mandatory.");

                    //    continue;
                    //}

                    #endregion

                    if (!projects.TryGetValue(
                            transaction.ProjectName,
                            out var project))
                    {
                        AddImportError(
                            result,
                            rowNumber,
                            nameof(transaction.ProjectName),
                            transaction.ProjectName,
                            "Project not found.");

                        continue;
                    }

                    if (!categories.TryGetValue(
                            transaction.CategoryName,
                            out var category))
                    {
                        AddImportError(
                            result,
                            rowNumber,
                            nameof(transaction.CategoryName),
                            transaction.CategoryName,
                            "Category not found.");

                        continue;
                    }

                    if (!transactionTypes.TryGetValue(
                            transaction.TransactionType,
                            out var transactionType))
                    {
                        AddImportError(
                            result,
                            rowNumber,
                            nameof(transaction.TransactionType),
                            transaction.TransactionType,
                            "Transaction Type not found.");

                        continue;
                    }

                    var paidByEntity =
                        GetOrCreateEntity(
                            transaction.PaidByType,
                            transaction.PaidBy,
                            entityCache,
                            newEntities);

                    var receivedByEntity =
                        GetOrCreateEntity(
                            transaction.ReceivedByType,
                            transaction.ReceivedBy,
                            entityCache,
                            newEntities);

                    var hash = transaction.HashValue;

                    if (existingHashes.Contains(hash))
                    {
                        duplicateRows++;
                        continue;
                    }

                    existingHashes.Add(hash);

                    transactionsToInsert.Add(
                        new TransactionMaster
                        {
                            TransactionDate = transaction.TransactionDate,

                            ProjectId = project.ProjectId,

                            CategoryId = category.CategoryId,

                            TransactionTypeId =
                                transactionType.TransactionTypeId,

                            Amount = transaction.Amount,

                            Narration = transaction.Narration,

                            GoogleDriveLink =
                                transaction.GoogleDriveLink,

                            TransactionHash = hash,

                            ImportedOn = importedOn,

                            PaidByEntity = paidByEntity,

                            ReceivedByEntity =
                                receivedByEntity
                        });

                    importedRows++;
                }
                catch (Exception ex)
                {
                    AddImportError(
                        result,
                        rowNumber,
                        "System",
                        string.Empty,
                        ex.Message);

                    _logger.LogError(
                        ex,
                        "Error processing row {RowNumber}",
                        rowNumber);
                }
            }

            if (newEntities.Count > 0)
            {
                await _entityRepository.AddRangeAsync(
                    newEntities);

                await _entityRepository.SaveChangesAsync();
            }

            if (transactionsToInsert.Count > 0)
            {
                await _transactionRepository.AddRangeAsync(
                    transactionsToInsert);

                await _transactionRepository.SaveChangesAsync();
            }

            result.ImportedRows = importedRows;
            result.DuplicateRows = duplicateRows;
            result.ErrorRows = result.ImportErrors.Count;
            result.Success = true;

            result.Message =
                $"Processed={result.TotalRows}, Imported={importedRows}, Duplicates={duplicateRows}, Errors={result.ErrorRows}";

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Transaction processing failed.");

            result.Success = false;
            result.Message = ex.Message;

            return result;
        }
    }

    private Entity GetOrCreateEntity(
    string entityType,
    string entityName,
    Dictionary<string, Entity> entityCache,
    List<Entity> newEntities)
    {
        if (entityCache.TryGetValue(
                entityName,
                out var entity))
        {
            return entity;
        }

        entity = new Entity
        {
            EntityType = entityType,
            EntityName = string.IsNullOrWhiteSpace(entityName) ? $"Unknown {entityType}" : entityName.Trim(),
            IsActive = true
        };

        entityCache.Add(
            entityName,
            entity);

        newEntities.Add(entity);

        return entity;
    }

    private static void AddImportError(
    ImportResultDto result,
    int rowNumber,
    string columnName,
    string? fieldValue,
    string errorMessage)
    {
        result.ImportErrors.Add(
            new ValidationErrorDto
            {
                RowNumber = rowNumber,
                ColumnName = columnName,
                FieldValue = fieldValue ?? string.Empty,
                ErrorMessage = errorMessage
            });
    }
}