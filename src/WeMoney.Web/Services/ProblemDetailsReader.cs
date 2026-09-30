using System.Net;
using System.Text.Json;

namespace WeMoney.Web.Services;

/// <summary>Extrai uma mensagem legível de uma resposta de erro (ProblemDetails) da API.</summary>
public static class ProblemDetailsReader
{
    public static string GetMessage(string? body, HttpStatusCode status)
    {
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (TryGetString(root, "detail", out var detail))
                        return detail;

                    // Erros de validação do model binding: { "errors": { "campo": ["mensagem"] } }
                    if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                    {
                        var messages = errors.EnumerateObject()
                            .SelectMany(e => e.Value.ValueKind == JsonValueKind.Array
                                ? e.Value.EnumerateArray().Select(m => m.GetString())
                                : [])
                            .Where(m => !string.IsNullOrWhiteSpace(m))
                            .ToList();
                        if (messages.Count > 0)
                            return string.Join(" ", messages);
                    }

                    if (TryGetString(root, "title", out var title))
                        return title;
                }
            }
            catch (JsonException)
            {
                // Corpo não é JSON: usa a mensagem padrão.
            }
        }

        return $"Erro inesperado ({(int)status}).";
    }

    private static bool TryGetString(JsonElement element, string name, out string value)
    {
        value = element.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? ""
            : "";
        return value.Length > 0;
    }
}
