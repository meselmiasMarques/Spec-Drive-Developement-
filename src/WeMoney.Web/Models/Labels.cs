namespace WeMoney.Web.Models;

public static class Labels
{
    public static string For(TransactionType type) => type switch
    {
        TransactionType.INCOME => "Receita",
        TransactionType.EXPENSE => "Despesa",
        _ => type.ToString()
    };
}
