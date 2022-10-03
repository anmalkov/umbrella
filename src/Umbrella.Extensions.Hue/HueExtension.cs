using PhilipsHue.Sdk;
using PhilipsHue.Sdk.Models;
using System.Net.Http;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Extensions.Hue;

public class HueExtension : IExtension
{
    private const string BridgeIpParameterName = "bridgeIp";
    private const string AppKeyParameterName = "appKey";
    private const string ClientKeyParameterName = "clientKey";
    private const string ApplicationName = "Umbrella";
    private const string DeviceName = "ExtensionHue";

    private readonly HttpClient _httpClient;
    private readonly IRegistrationService _coreService;
    private IHueClient? _hueClient;

    public string Id => "hue";
    public string DisplayName => "Philips Hue";
    public string Image => "";

    public string HtmlForRegistration => @"
  <div class=""mb-3"">
    <label for=""" + BridgeIpParameterName + @""" class=""form-label"">Hue bridge IP address</label>
    <input type=""text"" class=""form-control"" name=""" + BridgeIpParameterName + @""" aria-describedby=""hubIpAddressHelp"">
    <div id=""hubIpAddressHelp"" class=""form-text"">In the Hue mobile app go to: Settings -> My Hue System -> Philips Hue</div>
  </div>
  <p><b>NOTE:</b> Please make sure that you press the button on your Philips Hue Hub before you press 'Register' button below</p>";

    
    public HueExtension(IRegistrationService coreService) : this (coreService, null) { }

    public HueExtension(IRegistrationService coreService, HttpClient? httpClient)
    {
        if (httpClient is null)
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
        } else
        {
            _httpClient = httpClient;
        }
        _coreService = coreService;
    }
    

    public HueExtension(IRegistrationService coreService, HttpClient? httpClient, IHueClient hueClient)
        : this(coreService, httpClient)
    {
        _hueClient = hueClient;
    }

    
    public async Task RegisterAsync(Dictionary<string, string?>? parameters)
    {
        if (parameters is null || !parameters.ContainsKey(BridgeIpParameterName))
        {
            throw new ArgumentException($"Missing required parameter '{BridgeIpParameterName}'");
        }

        var bridgeIp = parameters[BridgeIpParameterName];
        if (string.IsNullOrWhiteSpace(bridgeIp))
        {
            throw new ArgumentException($"Parameter '{BridgeIpParameterName}' must have a value");
        }

        _hueClient = new HueClient(_httpClient, bridgeIp);
        var regInfo = await _hueClient.RegisterAsync(ApplicationName, DeviceName);

        parameters.Add(AppKeyParameterName, regInfo?.ApplicationKey);
        parameters.Add(ClientKeyParameterName, regInfo?.ApplicationKey);

        var lights = await _hueClient.GetLightsAsync();
        int lightIndex = 1;
        foreach (var light in lights)
        {
            var entity = MapLightToEntity(light, lightIndex++);
            await _coreService.RegisterEntityAsync(entity, Id);
        }
    }

    public Task UnregisterAsync(Dictionary<string, string?>? parameters)
    {
        return Task.CompletedTask;
    }
    
    public Task StartAsync(Dictionary<string, string?>? parameters)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }


    private LightEntity MapLightToEntity(PhilipsHueLight light, int index)
    {
        return new LightEntity(GenerateEntityId(light, index))
        {
            Name = light.Metadata?.Name ?? $"Light {index}",
            Available = true,
            Enabled = true,
            MinColorTemperature = 153,
            MaxColorTemperature = 542
        };
    }

    private string GenerateEntityId(PhilipsHueLight light, int index)
    {
        var name = light.Metadata?.Name ?? $"Light {index}";
        name = name.ToLower().Replace(' ', '_');
        
        return $"light.{Id}.{name}";
    }

}