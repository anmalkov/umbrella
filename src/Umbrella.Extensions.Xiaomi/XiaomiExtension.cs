using Umbrella.Core.Extensions;

namespace Umbrella.Extensions.Xiaomi;

public class XiaomiExtension : IExtension
{
    public string Id => "mi";
    public string DisplayName => "Xiaomi";
    public string Image => "";

    public string HtmlForRegistration => @"None";

    public Task RegisterAsync(Dictionary<string, string?>? parameters)
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(Dictionary<string, string?>? parameters)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        throw new NotImplementedException();
    }

    public Task UnregisterAsync(Dictionary<string, string?>? parameters)
    {
        throw new NotImplementedException();
    }
}