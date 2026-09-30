using WeMoney.Application.Abstractions;
using WeMoney.Application.Exceptions;
using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;

namespace WeMoney.Application.Categories;

/// <summary>SPEC-FUNC — Categorias.</summary>
public class CategoryService(ICategoryRepository categories, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<CategoryResponse>> ListAsync(Guid userId, CancellationToken ct = default)
    {
        var list = await categories.ListAsync(userId, ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<CategoryResponse> GetAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        return ToResponse(await FindAsync(id, userId, ct));
    }

    public async Task<CategoryResponse> CreateAsync(Guid userId, CategoryRequest request, 
        CancellationToken ct = default)
    {
        var category = new Category(userId, request.Name!, RequireType(request.Type));
        categories.Add(category);
        await unitOfWork.SaveChangesAsync(ct);
        return ToResponse(category);
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, Guid userId, CategoryRequest request, CancellationToken ct = default)
    {
        var category = await FindAsync(id, userId, ct);
        category.Update(request.Name!, RequireType(request.Type));
        await unitOfWork.SaveChangesAsync(ct);
        return ToResponse(category);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var category = await FindAsync(id, userId, ct);
        if (await categories.HasTransactionsAsync(id, userId, ct))
            throw new ConflictException("Categoria possui transações associadas e não pode ser excluída.");

        categories.Remove(category);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Category> FindAsync(Guid id, Guid userId, CancellationToken ct)
    {
        return await categories.GetAsync(id, userId, ct)
               ?? throw new NotFoundException("Categoria não encontrada.");
    }

    private static TransactionType RequireType(TransactionType? type)
    {
        return type ?? throw new DomainException("Tipo é obrigatório (INCOME ou EXPENSE).");
    }

    private static CategoryResponse ToResponse(Category c) => new(c.Id, c.Name, c.Type);
}
