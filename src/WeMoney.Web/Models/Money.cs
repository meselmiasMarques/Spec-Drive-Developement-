using System.Globalization;

namespace WeMoney.Web.Models;

public static class Money
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string Format(decimal value) => value.ToString("C", PtBr);
}
