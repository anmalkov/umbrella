using Moq;
using Moq.Protected;
using PhilipsHue.Sdk.Exceptions;
using System.Net;
using System.Net.Http;

namespace PhilipsHue.Sdk.Tests;

public class HueClientTest
{
    [Fact]
    public async Task Register_throws_PhilipsHueLinkButtonNotPressedException_if_button_is_not_pressed()
    {
        var handler = new Mock<HttpMessageHandler>();
        ConfigureHandlerToReturnStringContent(handler, "[{\"error\":{\"type\":101,\"address\":\"\",\"description\":\"link button not pressed\"}}]");
        
        var httpClient = new HttpClient(handler.Object);
        
        var hueClient = new HueClient(httpClient, "0.0.0.0");

        await Assert.ThrowsAsync<PhilipsHueLinkButtonNotPressedException>(async () => await hueClient.RegisterAsync("test", "test"));
    }

    [Fact]
    public async Task Register_throws_PhilipsHueException_if_empty_response()
    {
        var handler = new Mock<HttpMessageHandler>();
        ConfigureHandlerToReturnStringContent(handler, "");
        
        var httpClient = new HttpClient(handler.Object);
              
        
        var hueClient = new HueClient(httpClient, "0.0.0.0");

        await Assert.ThrowsAsync<PhilipsHueException>(async () => await hueClient.RegisterAsync("test", "test"));
    }

    [Fact]
    public async Task Register_throws_PhilipsHueException_if_response_has_error()
    {
        var handler = new Mock<HttpMessageHandler>();
        ConfigureHandlerToReturnStringContent(handler, "[{\"error\":{\"type\":7,\"address\":\"/devicetype\",\"description\":\"invalid value,  umbdgdgdgdfgdgdgdgdfgdfgdfeeerlerta#sdjhfgsdfhgsdfjhg, for parameter, devicetype\"}}]");

        var httpClient = new HttpClient(handler.Object);

        var hueClient = new HueClient(httpClient, "0.0.0.0");

        await Assert.ThrowsAsync<PhilipsHueException>(async () => await hueClient.RegisterAsync("test", "test"));
    }

    [Fact]
    public async Task Register_returns_correct_RegistrationInfo()
    {
        const string appKey = "e7agfgujwtNIZgwdGluyCKWqztvKYWBn99EzRB2f";
        const string clientKey = "7CC024A72845421CB31CC4152746B242";

        var handler = new Mock<HttpMessageHandler>();
        ConfigureHandlerToReturnStringContent(handler, $"[{{\"success\":{{\"username\":\"{appKey}\",\"clientkey\":\"{clientKey}\"}}}}]");

        var httpClient = new HttpClient(handler.Object);

        var hueClient = new HueClient(httpClient, "0.0.0.0");

        var regInfo = await hueClient.RegisterAsync("test", "test");

        Assert.NotNull(regInfo);
        Assert.Equal(appKey, regInfo.ApplicationKey);
        Assert.Equal(clientKey, regInfo.StreamingClientKey);
    }

    private static void ConfigureHandlerToReturnStringContent(Mock<HttpMessageHandler> handler, string content)
    {
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            })
            .Verifiable();
    }
}