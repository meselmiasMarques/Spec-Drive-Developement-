namespace WeMoney.Web.Services;

/// <summary>Erro retornado pela API, com mensagem pronta para exibição.</summary>
public class ApiException(string message) : Exception(message);
