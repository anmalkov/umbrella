using PhilipsHue.Sdk;
using PhilipsHue.Sdk.Models;
using System.Net.Http;
using System.Text.Json;
using Umbrella.Core.Events;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Extensions.Hue;

internal record LightId(Guid HueId, string EntityId);

public class HueExtension : IExtension
{
    private const string BridgeIpParameterName = "bridgeIp";
    private const string AppKeyParameterName = "appKey";
    private const string ClientKeyParameterName = "clientKey";
    private const string LightsIdsParameterName = "ids";
    private const string ApplicationName = "Umbrella";
    private const string DeviceName = "ExtensionHue";

    private readonly HttpClient _httpClient;
    private readonly IRegistrationService _coreService;
    private readonly IEventsService _eventsService;

    private IHueClient? _hueClient;
    private List<LightId> _lightsIds = new();

    public string Id => "hue";
    public string? DisplayName => "Philips Hue";
    public string? Image => "";

    public string? HtmlForRegistration => @"
  <div class=""mb-3"">
    <label for=""" + BridgeIpParameterName + @""" class=""form-label"">Hue bridge IP address</label>
    <input type=""text"" class=""form-control"" name=""" + BridgeIpParameterName + @""" aria-describedby=""hubIpAddressHelp"">
    <div id=""hubIpAddressHelp"" class=""form-text"">In the Hue mobile app go to: Settings -> My Hue System -> Philips Hue</div>
  </div>
  <p><b>NOTE:</b> Please make sure that you press the button on your Philips Hue Hub before you press 'Register' button below</p>";

    
    public HueExtension(IRegistrationService coreService, IEventsService eventsService) : this (coreService, eventsService, null)
    {
        
    }

    public HueExtension(IRegistrationService coreService, IEventsService eventsService, HttpClient? httpClient)
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
        _eventsService = eventsService;
    }
    
    public HueExtension(IRegistrationService coreService, IEventsService eventsService, HttpClient? httpClient, IHueClient hueClient)
        : this(coreService, eventsService, httpClient)
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
        var lightsIds = new List<LightId>();
        foreach (var light in lights)
        {
            var entity = MapLightToEntity(light, lightIndex++);
            await _coreService.RegisterEntityAsync(entity, Id);
            lightsIds.Add(new LightId(light.Id, entity.Id));
        }

        parameters.Add(LightsIdsParameterName, JsonSerializer.Serialize(lightsIds));
    }

    public Task UnregisterAsync(Dictionary<string, string?>? parameters)
    {
        return Task.CompletedTask;
    }
    
    public async Task StartAsync(Dictionary<string, string?>? parameters)
    {
        if (parameters is not null && parameters.ContainsKey(BridgeIpParameterName)) {
            var bridgeIp = parameters[BridgeIpParameterName];
            var appKey = parameters.ContainsKey(BridgeIpParameterName) ? parameters[AppKeyParameterName] : null;
            if (!string.IsNullOrWhiteSpace(bridgeIp))
            {
                _hueClient = new HueClient(_httpClient, bridgeIp, appKey);
            }
        }
        if (parameters is not null && parameters.ContainsKey(LightsIdsParameterName))
        {
            _lightsIds = JsonSerializer.Deserialize<List<LightId>>(parameters?[LightsIdsParameterName] ?? "") ?? new List<LightId>();
        }
        
        await ReportCurrentStateForLightsAsync();
        
        _eventsService.Subscribe(EventNames.LightChangeState, OnLightChangeState);
    }

    private async Task ReportCurrentStateForLightsAsync()
    {
        if (_hueClient is null || !_lightsIds.Any())
        {
            return;
        }
        
        var lights = await _hueClient.GetLightsAsync();
        foreach (var lightId in _lightsIds)
        {
            var light = lights.FirstOrDefault(l => l.Id == lightId.HueId);
            if (light is null)
            {
                continue;
            }
            _eventsService.Publish(new LightChangeStateEvent(lightId.EntityId, light.On?.TurnedOn)
            {
                Brightness = (byte?)light.Dimming?.Brightness,
                ColorTemperature = light.ColorTemperature?.Mirek
            });
        }
    }

    private void OnLightChangeState(IEvent? payload)
    {
        if (payload is null || payload is not LightChangeStateEvent lightChangeStateEvent)
        {
            return;
        }

        var lightId = _lightsIds.FirstOrDefault(l => l.EntityId == lightChangeStateEvent.EntityId);
        if (lightId is null)
        {
            return;
        }
        var _ = (_hueClient!.UpdateLightAsync(lightId.HueId, new PhilipsHueUpdateLight { 
            On = lightChangeStateEvent.TurnedOn is not null ? new() { TurnedOn = lightChangeStateEvent.TurnedOn.Value } : null,
            Dimming = lightChangeStateEvent.Brightness is not null ? new() { Brightness = lightChangeStateEvent.Brightness.Value } : null
        })).Result;
    }

    public Task StopAsync()
    {
        _eventsService.Unsubscribe(EventNames.LightChangeState, OnLightChangeState);
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