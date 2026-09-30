using WeMoney.Domain.Entities;

namespace WeMoney.Application.Categories;

public record CategoryRequest(string? Name, TransactionType? Type);

public record CategoryResponse(Guid Id, string Name, TransactionType Type);
