using WeMoney.Application.Abstractions;
using WeMoney.Application.Exceptions;
using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;

namespace WeMoney.Application.Transactions;

/// <summary>SPEC-FUNC — Transações.</summary>
public class TransactionService(
    ITransactionRepository transactions,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<IReadOnlyList<TransactionResponse>> ListAsync(Guid userId, CancellationToken ct = default)
    {
        var list = await transactions.ListAsync(userId, ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<TransactionResponse> GetAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        return ToResponse(await FindAsync(id, userId, ct));
    }

    public async Task<TransactionResponse> CreateAsync(Guid userId, TransactionRequest request, CancellationToken ct = default)
    {
        var category = await FindCategoryAsync(request.CategoryId, userId, ct);
        var transaction = new Transaction(userId, category, request.Description!, request.Amount,
            RequireType(request.Type), ResolveDate(request.TransactionDate));

        transactions.Add(transaction);
        await unitOfWork.SaveChangesAsync(ct);
        return ToResponse(transaction);
    }

    /// <summary>RN-004: o usuário somente pode alterar suas próprias transações.</summary>
    public async Task<TransactionResponse> UpdateAsync(Guid id, Guid userId, TransactionRequest request, CancellationToken ct = default)
    {
        var transaction = await FindAsync(id, userId, ct);
        var category = await FindCategoryAsync(request.CategoryId, userId, ct);

        transaction.Update(category, request.Description!, request.Amount,
            RequireType(request.Type), ResolveDate(request.TransactionDate));

        await unitOfWork.SaveChangesAsync(ct);
        return ToResponse(transaction);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var transaction = await FindAsync(id, userId, ct);
        transactions.Remove(transaction);
        await unitOfWork.SaveChangesAsync(ct);
    }

    /// <summary>REQ-004 (SPEC-001) — saldo = receitas - despesas.</summary>
    public async Task<BalanceResponse> GetBalanceAsync(Guid userId, CancellationToken ct = default)
    {
        var income = await transactions.SumAsync(userId, TransactionType.INCOME, ct);
        var expense = await transactions.SumAsync(userId, TransactionType.EXPENSE, ct);
        return new BalanceResponse(income, expense, income - expense);
    }

    private async Task<Transaction> FindAsync(Guid id, Guid userId, CancellationToken ct)
    {
        return await transactions.GetAsync(id, userId, ct)
               ?? throw new NotFoundException("Transação não encontrada.");
    }

    /// <summary>RN-002 / RN-003: categoria válida e do próprio usuário.</summary>
    private async Task<Category> FindCategoryAsync(Guid categoryId, Guid userId, CancellationToken ct)
    {
        return await categories.GetAsync(categoryId, userId, ct)
               ?? throw new DomainException("Categoria inválida.");
    }

    /// <summary>Decisão registrada na SPEC: data opcional, padrão = data atual (UTC).</summary>
    private DateOnly ResolveDate(DateOnly? date)
    {
        return date ?? DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
    }

    private static TransactionType RequireType(TransactionType? type)
    {
        return type ?? throw new DomainException("Tipo é obrigatório (INCOME ou EXPENSE).");
    }

    private static TransactionResponse ToResponse(Transaction t) =>
        new(t.Id, t.Description, t.Amount, t.Type, t.CategoryId, t.TransactionDate, t.CreatedAt);
}
