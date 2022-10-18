using PhilipsHue.Sdk;
using PhilipsHue.Sdk.Models;
using System.Text.Json;
using System.Transactions;
using System.Xml.Linq;
using Umbrella.Core.Events;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;

namespace Umbrella.Extensions.Hue;

internal record LightId(Guid HueId, Guid HueDeviceId, string EntityId);
internal record AreaId(string id, List<Guid> ChildrenHueRid);

public class HueExtension : IExtension
{
    private const string BridgeIpParameterName = "bridgeIp";
    private const string AppKeyParameterName = "appKey";
    private const string ClientKeyParameterName = "clientKey";
    private const string LightsIdsParameterName = "ids";
    private const string ApplicationName = "Umbrella";
    private const string DeviceName = "ExtensionHue";

    private readonly HttpClient _httpClient;
    private readonly IRegistrationService _registrationService;
    private readonly IEventsService _eventsService;

    private IHueClient? _hueClient;
    private List<LightId> _lightsIds = new();
    private CancellationTokenSource? _eventStreamCancellationTokenSource;

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
        _registrationService = coreService;
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

        var areaIds = await AddAllAreas();

        var lightsIds = await AddAllLights(areaIds);
        parameters.Add(LightsIdsParameterName, JsonSerializer.Serialize(lightsIds));

        await AddAllGroups(lightsIds);
    }

    private async Task AddAllGroups(IEnumerable<LightId> lightIds)
    {
        if (_hueClient is null)
        {
            return;
        }

        var zones = await _hueClient.GetZonesAsync();
        int zoneIndex = 1;
        foreach (var zone in zones)
        {
            var entityIds = zone.Children.Where(z => lightIds.Any(l => l.HueId == z.Rid)).Select(z => lightIds.First(l => l.HueId == z.Rid).EntityId);
            var group = MapZoneToGroup(zone, zoneIndex++, entityIds);
            await _registrationService.AddGroupAsync(group, Id);
        }
    }

    private async Task<IEnumerable<AreaId>> AddAllAreas()
    {
        if (_hueClient is null)
        {
            return new List<AreaId>();
        }

        var rooms = await _hueClient.GetRoomsAsync();
        int roomIndex = 1;
        var areasIds = new List<AreaId>();
        foreach (var room in rooms)
        {
            var area = MapRoomToArea(room, roomIndex++);
            await _registrationService.AddAreaAsync(area, Id);
            areasIds.Add(new AreaId(area.Id, room.Children.Where(c => c.Rtype == "device").Select(c => c.Rid).ToList()));
        }
        return areasIds;
    }

    private async Task<List<LightId>> AddAllLights(IEnumerable<AreaId> areaIds)
    {
        if (_hueClient is null)
        {
            return new();
        }
        
        var lights = await _hueClient.GetLightsAsync();
        int lightIndex = 1;
        var lightsIds = new List<LightId>();
        foreach (var light in lights)
        {
            var areaId = areaIds.Where(a => a.ChildrenHueRid.Any(d => d == light.Owner.Rid)).Select(a => a.id).FirstOrDefault();
            var entity = MapLightToEntity(light, lightIndex++, areaId);
            await _registrationService.RegisterEntityAsync(entity, Id);
            lightsIds.Add(new LightId(light.Id, light.Owner.Rid, entity.Id));
        }
        return lightsIds;
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
        
        _eventsService.Subscribe(EventNames.ChangeEntityState, OnChangeLightState);

        StartListeningForEvents();
    }

    public Task StopAsync()
    {
        _eventsService.Unsubscribe(EventNames.ChangeEntityState, OnChangeLightState);

        StopListeningForEvents();

        return Task.CompletedTask;
    }


    private void StartListeningForEvents()
    {
        if (_hueClient is null) {
            return;
        }

        _hueClient.OnEventMessage += async (events) => await ProcessEventMessages(events);
        _hueClient.StartListeningForEventsAsync();
    }

    private async Task ProcessEventMessages(IEnumerable<PhilipsHueEventResponse> events)
    {
        foreach (var hueEvent in events)
        {
            foreach (var data in hueEvent.Data)
            {
                if (data.Type == PhilipsHueType.Light || data.Type == PhilipsHueType.ZigbeeConnectivity)
                {
                    var lightId = data.Type == PhilipsHueType.Light
                        ? _lightsIds.FirstOrDefault(l => l.HueId == data.Id)
                        : _lightsIds.FirstOrDefault(l => l.HueDeviceId == data.Owner.Rid);
                    if (lightId is null) {
                        continue;
                    }
                    var state = new LightEntityState
                    {
                        TurnedOn = data.On?.TurnedOn,
                        Brightness = (byte?)data.Dimming?.Brightness,
                        ColorTemperature = data.ColorTemperature?.Mirek,
                        Connected = string.IsNullOrEmpty(data.Status) ? null : data.Status == "connected"
                    };
                    if (state.Connected.HasValue && state.Connected.Value)
                    {
                        var light = await _hueClient!.GetLightAsync(lightId.HueId);
                        if (light is not null)
                        { 
                            state.TurnedOn = light.On.TurnedOn;
                            state.Brightness = (byte?)light.Dimming?.Brightness;
                            state.ColorTemperature = light.ColorTemperature?.Mirek;
                        }
                    }
                    _eventsService.Publish(new EntityStateChangedEvent(lightId.EntityId, state));
                }
            }
        }
    }

    private void StopListeningForEvents()
    {
        _hueClient?.StopListeningForEvents();
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
            var state = new LightEntityState
            {
                TurnedOn = light.On?.TurnedOn,
                Brightness = (byte?)light.Dimming?.Brightness,
                ColorTemperature = light.ColorTemperature?.Mirek
            };
            _eventsService.Publish(new EntityStateChangedEvent(lightId.EntityId, state));
        }
    }
    
    private void OnChangeLightState(IEvent? payload)
    {
        if (payload is null || payload is not ChangeEntityStateEvent changeEntityStateEvent)
        {
            return;
        }

        var state = changeEntityStateEvent.State as LightEntityState;
        if (state is null)
        {
            return;
        }

        var lightId = _lightsIds.FirstOrDefault(l => l.EntityId == changeEntityStateEvent.EntityId);
        if (lightId is null)
        {
            return;
        }
        var _ = (_hueClient!.UpdateLightAsync(lightId.HueId, new PhilipsHueUpdateLight
        {
            On = state.TurnedOn is not null ? new() { TurnedOn = state.TurnedOn.Value } : null,
            Dimming = state.Brightness is not null ? new() { Brightness = state.Brightness.Value } : null
        })).Result;
    }

    
    private string GenerateEntityId(string name)
    {
        name = name.ToLower().Replace(' ', '_');
        
        return $"light.{Id}.{name}";
    }

    private string GenerateAreaId(string name)
    {
        name = name.ToLower().Replace(' ', '_');

        return $"area.{name}";
    }
    
    private string GenerateGroupId(string name)
    {
        name = name.ToLower().Replace(' ', '_');

        return $"group.{name}";
    }

    private LightEntity MapLightToEntity(PhilipsHueLight light, int index, string? areaId)
    {
        var name = light.Metadata?.Name ?? $"Light {index}";
        return new LightEntity(GenerateEntityId(name))
        {
            Name = name,
            AreaId = areaId,
            Enabled = true,
            MinColorTemperature = light.ColorTemperature?.MirekSchema?.MirekMinimum,
            MaxColorTemperature = light.ColorTemperature?.MirekSchema?.MirekMaximum,
        };
    }

    private Area MapRoomToArea(PhilipsHueRoom room, int index)
    {
        var name = room.Metadata?.Name ?? $"Area {index}";
        return new Area(GenerateAreaId(name), name);
    }

    private Group MapZoneToGroup(PhilipsHueZone zone, int index, IEnumerable<string> entities)
    {
        var name = zone.Metadata?.Name ?? $"Group {index}";
        return new Group(GenerateGroupId(name), name) {
            Entities = new List<string>(entities)
        };
    }

}