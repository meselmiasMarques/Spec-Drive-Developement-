using System.Net;
using WeMoney.Web.Services;

namespace WeMoney.UnitTests.Web;

public class ProblemDetailsReaderTests
{
    [Fact]
    public void GetMessage_WithDetail_ReturnsDetail()
    {
        var body = """{"title":"Conflito","status":409,"detail":"E-mail já cadastrado."}""";

        Assert.Equal("E-mail já cadastrado.", ProblemDetailsReader.GetMessage(body, HttpStatusCode.Conflict));
    }

    [Fact]
    public void GetMessage_WithValidationErrors_JoinsMessages()
    {
        var body = """{"title":"Erro","errors":{"Name":["Nome é obrigatório."],"Email":["E-mail inválido."]}}""";

        Assert.Equal("Nome é obrigatório. E-mail inválido.", ProblemDetailsReader.GetMessage(body, HttpStatusCode.BadRequest));
    }

    [Fact]
    public void GetMessage_WithOnlyTitle_ReturnsTitle()
    {
        Assert.Equal("Erro interno", ProblemDetailsReader.GetMessage("""{"title":"Erro interno"}""", HttpStatusCode.InternalServerError));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("<html>gateway</html>")]
    public void GetMessage_WithoutProblemDetails_ReturnsStatusMessage(string? body)
    {
        Assert.Equal("Erro inesperado (502).", ProblemDetailsReader.GetMessage(body, HttpStatusCode.BadGateway));
    }
}
