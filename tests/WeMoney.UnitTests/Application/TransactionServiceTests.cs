using WeMoney.Application.Exceptions;
using WeMoney.Application.Transactions;
using WeMoney.Domain.Entities;
using WeMoney.Domain.Exceptions;
using WeMoney.UnitTests.Fakes;

namespace WeMoney.UnitTests.Application;

public class TransactionServiceTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 9, 25, 12, 0, 0, TimeSpan.Zero);

    private readonly FakeTransactionRepository _transactions = new();
    private readonly FakeCategoryRepository _categories = new();
    private readonly TransactionService _service;
    private readonly Category _categoryA = new(UserA, "Mercado", TransactionType.EXPENSE);
    private readonly Category _categoryB = new(UserB, "Salário", TransactionType.INCOME);

    public TransactionServiceTests()
    {
        _categories.Add(_categoryA);
        _categories.Add(_categoryB);
        _service = new TransactionService(_transactions, _categories, new FakeUnitOfWork(), new FixedTimeProvider(Now));
    }

    private TransactionRequest Request(decimal amount = 250.50m, Guid? categoryId = null, DateOnly? date = null) =>
        new("Supermercado", amount, TransactionType.EXPENSE, categoryId ?? _categoryA.Id, date);

    [Fact]
    public async Task Create_WithoutDate_UsesToday()
    {
        var result = await _service.CreateAsync(UserA, Request());

        Assert.Equal(new DateOnly(2026, 9, 25), result.TransactionDate);
    }

    [Fact]
    public async Task Create_WithDate_KeepsDate()
    {
        var result = await _service.CreateAsync(UserA, Request(date: new DateOnly(2026, 1, 10)));

        Assert.Equal(new DateOnly(2026, 1, 10), result.TransactionDate);
    }

    // RN-001
    [Fact]
    public async Task Create_WithZeroAmount_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(UserA, Request(amount: 0)));
    }

    // RN-002
    [Fact]
    public async Task Create_WithUnknownCategory_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(UserA, Request(categoryId: Guid.NewGuid())));
    }

    // RN-003
    [Fact]
    public async Task Create_WithCategoryFromAnotherUser_Throws()
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(UserA, Request(categoryId: _categoryB.Id)));
        Assert.Empty(_transactions.Items);
    }

    [Fact]
    public async Task Create_WithoutType_Throws()
    {
        var request = Request() with { Type = null };
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(UserA, request));
    }

    // RN-004
    [Fact]
    public async Task Update_Delete_FromAnotherUser_ThrowsNotFound()
    {
        var created = await _service.CreateAsync(UserA, Request());

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAsync(created.Id, UserB));
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(created.Id, UserB, Request(10m, _categoryB.Id)));
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(created.Id, UserB));
        Assert.Equal(250.50m, _transactions.Items.Single().Amount);
    }

    [Fact]
    public async Task Update_ChangesValues()
    {
        var created = await _service.CreateAsync(UserA, Request());

        var updated = await _service.UpdateAsync(created.Id, UserA, Request(amount: 99m));

        Assert.Equal(99m, updated.Amount);
    }

    [Fact]
    public async Task Balance_IsIncomeMinusExpense_ForUserOnly()
    {
        await _service.CreateAsync(UserA, Request(amount: 100m) with { Type = TransactionType.INCOME });
        await _service.CreateAsync(UserA, Request(amount: 30m));
        await _service.CreateAsync(UserB, new TransactionRequest("Salário", 5000m, TransactionType.INCOME, _categoryB.Id, null));

        var balance = await _service.GetBalanceAsync(UserA);

        Assert.Equal(new BalanceResponse(100m, 30m, 70m), balance);
    }
}
