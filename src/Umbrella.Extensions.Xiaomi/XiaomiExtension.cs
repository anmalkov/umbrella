using Umbrella.Core.Extensions;

namespace Umbrella.Extensions.Xiaomi;

public class XiaomiExtension : IExtension
{
    private const string UsernameParameterName = "username";
    private const string PasswordParameterName = "password";
    private const string ServerCountryCodeParameterName = "server";
    //private const string GatewayIpParameterName = "gatewayIp";

    public string Id => "mi";
    public string? DisplayName => "Xiaomi";
    public string? Image => "";

    public string? HtmlForRegistration => @$"
  <div class=""mb-3"">
    <label for=""{UsernameParameterName}"" class=""form-label"">Username</label>
    <input type=""text"" class=""form-control"" name=""{UsernameParameterName}"" aria-describedby=""apiKeyHelp"">
    <div id=""apiKeyHelp"" class=""form-text"">Your e-mail or Xiaomi Cloud account ID that you are using to login to Xiaomi Home mobile app</div>
  </div>
  <div class=""mb-3"">
    <label for=""{PasswordParameterName}"" class=""form-label"">Password</label>
    <input type=""text"" class=""form-control"" name=""{PasswordParameterName}"" aria-describedby=""citiesHelp"">
    <div id=""citiesHelp"" class=""form-text"">Your password to login to Xiaomi Home mobile app</div>
  </div>
  <div class=""mb-3"">
    <label for=""{ServerCountryCodeParameterName}"" class=""form-label"">Server country</label>
    <select class=""form-select"" name=""{ServerCountryCodeParameterName}"" aria-describedby=""unitsHelp"">
        <option value=""cn"">China</option>
        <option value=""de"">Germany</option>
        <option value=""us"">USA</option>
        <option value=""ru"">Russia</option>
        <option value=""tw"">Taiwan</option>
        <option value=""sg"">Singapore</option>
        <option value=""in"">India</option>
        <option value=""i2"">India 2</option>
    </select>
    <div id=""unitsHelp"" class=""form-text"">Select Xiaomi server location</div>
  </div>";
  //<div class=""mb-3"">
  //  <label for=""{GatewayIpParameterName}"" class=""form-label"">Xiaomi gateway IP address</label>
  //  <input type=""text"" class=""form-control"" name=""{GatewayIpParameterName}"" aria-describedby=""hubIpAddressHelp"">
  //  <div id=""hubIpAddressHelp"" class=""form-text"">In the Mi Home mobile app go to: Gateaway -> Settings -> Additional settings -> Network info</div>
  //</div>";

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