using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeMoney.Api.Infrastructure;
using WeMoney.Application.Transactions;

namespace WeMoney.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/transactions")]
public class TransactionsController(TransactionService transactionService) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<TransactionResponse>> List(CancellationToken ct) =>
        await transactionService.ListAsync(User.GetUserId(), ct);

    [HttpGet("{id:guid}")]
    public async Task<TransactionResponse> Get(Guid id, CancellationToken ct) =>
        await transactionService.GetAsync(id, User.GetUserId(), ct);

    [HttpPost]
    [ProducesResponseType<TransactionResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(TransactionRequest request, CancellationToken ct)
    {
        var transaction = await transactionService.CreateAsync(User.GetUserId(), request, ct);
        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:guid}")]
    public async Task<TransactionResponse> Update(Guid id, TransactionRequest request, CancellationToken ct) =>
        await transactionService.UpdateAsync(id, User.GetUserId(), request, ct);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await transactionService.DeleteAsync(id, User.GetUserId(), ct);
        return NoContent();
    }

    /// <summary>REQ-004 (SPEC-001) — saldo do usuário.</summary>
    [HttpGet("balance")]
    public async Task<BalanceResponse> Balance(CancellationToken ct) =>
        await transactionService.GetBalanceAsync(User.GetUserId(), ct);
}
