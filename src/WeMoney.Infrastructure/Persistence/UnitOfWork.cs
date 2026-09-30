using Microsoft.EntityFrameworkCore;
using Npgsql;
using WeMoney.Application.Abstractions;
using WeMoney.Application.Exceptions;

namespace WeMoney.Infrastructure.Persistence;

public class UnitOfWork(WeMoneyDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("Registro já existente.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            throw new ConflictException("Registro possui vínculos e não pode ser alterado.");
        }
    }
}
