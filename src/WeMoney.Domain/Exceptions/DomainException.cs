namespace WeMoney.Domain.Exceptions;

/// <summary>Violação de uma regra de negócio ou de validação do domínio.</summary>
public class DomainException(string message) : Exception(message);
