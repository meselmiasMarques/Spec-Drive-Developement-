namespace WeMoney.Application.Exceptions;

/// <summary>Recurso inexistente ou pertencente a outro usuário (SPEC-006: não revela existência).</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Operação conflita com o estado atual (ex.: e-mail já cadastrado).</summary>
public class ConflictException(string message) : Exception(message);

/// <summary>Credenciais inválidas.</summary>
public class UnauthorizedException(string message) : Exception(message);
