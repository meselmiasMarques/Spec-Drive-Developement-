using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeMoney.Application.Exceptions;
using WeMoney.Domain.Exceptions;

namespace WeMoney.Api.Infrastructure;

/// <summary>Converte exceções de negócio em respostas ProblemDetails com o status HTTP adequado.</summary>
public class ExceptionHandler(IProblemDetailsService problemDetails, ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            NotFoundException => (StatusCodes.Status404NotFound, "Não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            UnauthorizedException or UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Não autorizado"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado");

        context.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                // Mensagens internas não são expostas em erros 500.
                Detail = status == StatusCodes.Status500InternalServerError ? null : exception.Message
            }
        });
    }
}
