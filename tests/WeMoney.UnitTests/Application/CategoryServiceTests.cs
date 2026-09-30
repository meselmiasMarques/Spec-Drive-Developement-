using WeMoney.Application.Categories;
using WeMoney.Application.Exceptions;
using WeMoney.Domain.Entities;
using WeMoney.UnitTests.Fakes;

namespace WeMoney.UnitTests.Application;

public class CategoryServiceTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    private readonly FakeTransactionRepository _transactions = new();
    private readonly FakeCategoryRepository _categories;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _categories = new FakeCategoryRepository(_transactions);
        _service = new CategoryService(_categories, new FakeUnitOfWork());
    }

    [Fact]
    public async Task Create_AssignsCategoryToUser()
    {
        var result = await _service.CreateAsync(UserA, new CategoryRequest("Contas fixas", TransactionType.EXPENSE));

        Assert.Equal(UserA, _categories.Items.Single(c => c.Id == result.Id).UserId);
    }

    // RN-003 / RN-004
    [Fact]
    public async Task Get_Update_Delete_FromAnotherUser_ThrowsNotFound()
    {
        var created = await _service.CreateAsync(UserA, new CategoryRequest("Contas fixas", TransactionType.EXPENSE));
        var request = new CategoryRequest("Alterada", TransactionType.EXPENSE);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAsync(created.Id, UserB));
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(created.Id, UserB, request));
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(created.Id, UserB));
        Assert.Equal("Contas fixas", _categories.Items.Single().Name);
    }

    [Fact]
    public async Task List_ReturnsOnlyUserCategories()
    {
        await _service.CreateAsync(UserA, new CategoryRequest("Da A", TransactionType.EXPENSE));
        await _service.CreateAsync(UserB, new CategoryRequest("Da B", TransactionType.EXPENSE));

        var result = await _service.ListAsync(UserA);

        Assert.Equal("Da A", Assert.Single(result).Name);
    }

    // RN-002
    [Fact]
    public async Task Delete_WithTransactions_ThrowsConflict()
    {
        var created = await _service.CreateAsync(UserA, new CategoryRequest("Mercado", TransactionType.EXPENSE));
        var category = _categories.Items.Single();
        _transactions.Add(new Transaction(UserA, category, "Compra", 10m, TransactionType.EXPENSE, new DateOnly(2026, 1, 1)));

        await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteAsync(created.Id, UserA));
        Assert.Single(_categories.Items);
    }

    [Fact]
    public async Task Delete_WithoutTransactions_Removes()
    {
        var created = await _service.CreateAsync(UserA, new CategoryRequest("Mercado", TransactionType.EXPENSE));

        await _service.DeleteAsync(created.Id, UserA);

        Assert.Empty(_categories.Items);
    }
}
