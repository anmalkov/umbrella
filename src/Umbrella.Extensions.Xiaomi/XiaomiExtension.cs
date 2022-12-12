using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using Umbrella.Core.Events;
using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Services;
using Xiaomi.Sdk;
using Xiaomi.Sdk.Models;

namespace Umbrella.Extensions.Xiaomi;

internal record DeviceId(string XiaomiDeviceId, string EntityId);

public class XiaomiExtension : IExtension
{
    private const string UsernameParameterName = "username";
    private const string PasswordParameterName = "password";
    private const string ServerCountryCodeParameterName = "server";
    private const string GatewayIpParameterName = "gatewayIp";
    private const string DevicesIdsParameterName = "ids";

    private readonly HttpClient _httpClient;
    private readonly IRegistrationService _registrationService;
    private readonly IEventsService _eventsService;
    
    private IXiaomiClient _xiaomiClient;
    private IEnumerable<DeviceId> _devicesIds;

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


    public XiaomiExtension(IRegistrationService coreService, IEventsService eventsService)
        : this(coreService, eventsService, null)
    {
    }

    public XiaomiExtension(IRegistrationService coreService, IEventsService eventsService, HttpClient? httpClient)
    {
        if (httpClient is null)
        {
            _httpClient = new HttpClient();
        }
        else
        {
            _httpClient = httpClient;
        }
        
        _registrationService = coreService;
        _eventsService = eventsService;
    }

    public XiaomiExtension(IRegistrationService coreService, IEventsService eventsService, HttpClient? httpClient, IXiaomiClient xiaomiClient)
        : this(coreService, eventsService, httpClient)
    {
        _xiaomiClient = xiaomiClient;
    }
        

    public async Task RegisterAsync(Dictionary<string, string?>? parameters)
    {
        var username = ExtensionsHelper.GetParameterValue(parameters, UsernameParameterName, parameterRequired: true);
        var password = ExtensionsHelper.GetParameterValue(parameters, PasswordParameterName, parameterRequired: true);
        var serverCountryCode = ExtensionsHelper.GetParameterValue(parameters, ServerCountryCodeParameterName, parameterRequired: true);
        if (serverCountryCode != "cn" && serverCountryCode != "de" && serverCountryCode != "us" && serverCountryCode != "ru" &&
            serverCountryCode != "tw" && serverCountryCode != "sg" && serverCountryCode != "in" && serverCountryCode != "i2")
        {
            throw new ArgumentException($"Parameter '{ServerCountryCodeParameterName}' is incorrect");
        }

        _xiaomiClient = new XiaomiClient(_httpClient);

        (var gatewayId, var devicesIds) = await AddAllDevicesAsync(username!, password!, serverCountryCode);
        if (string.IsNullOrWhiteSpace(gatewayId))
        {
            throw new Exception("Gateway not found");
        }

        parameters!.Add(GatewayIpParameterName, gatewayId);
        parameters.Add(DevicesIdsParameterName, JsonSerializer.Serialize(devicesIds));

        parameters.Remove(UsernameParameterName);
        parameters.Remove(PasswordParameterName);
        parameters.Remove(ServerCountryCodeParameterName);
    }

    public Task UnregisterAsync(Dictionary<string, string?>? parameters)
    {
        return Task.CompletedTask;
    }


    public async Task StartAsync(Dictionary<string, string?>? parameters)
    {
        if (parameters is not null && parameters.ContainsKey(GatewayIpParameterName))
        {
            var gatewayIp = parameters[GatewayIpParameterName];
            if (!string.IsNullOrWhiteSpace(gatewayIp))
            {
                _xiaomiClient = new XiaomiClient(_httpClient, gatewayIp);
            }
        }
        if (parameters is not null && parameters.ContainsKey(DevicesIdsParameterName))
        {
            _devicesIds = JsonSerializer.Deserialize<IEnumerable<DeviceId>>(parameters?[DevicesIdsParameterName] ?? "") ?? new List<DeviceId>();
        }

        await ReportCurrentStateForDevicesAsync();

        _eventsService.Subscribe(EventNames.ChangeEntityState, OnChangeDeviceState);

        StartListeningForEvents();
    }

    public Task StopAsync()
    {
        _eventsService.Unsubscribe(EventNames.ChangeEntityState, OnChangeDeviceState);

        StopListeningForEvents();
        
        return Task.CompletedTask;
    }


    private void StartListeningForEvents()
    {
        if (_xiaomiClient is null)
        {
            return;
        }

        _xiaomiClient.OnEventMessage += events => ProcessEventMessages(events);
        _xiaomiClient.StartListeningForEvents();
    }

    private void StopListeningForEvents()
    {
        _xiaomiClient?.StopListeningForEvents();
    }

    private void ProcessEventMessages(XiaomiEventResponse @event)
    {
        var device = @event.Device;

        var deviceId = _devicesIds.FirstOrDefault(d => d.XiaomiDeviceId == device.Id);
        if (deviceId is null)
        {
            return;
        }

        var state = GetDeviceState(device);
        if (state is null)
        {
            return;
        }

        _eventsService.Publish(new EntityStateChangedEvent(deviceId.EntityId, state));
    }

    private async Task ReportCurrentStateForDevicesAsync()
    {
        if (_xiaomiClient is null || !_devicesIds.Any())
        {
            return;
        }

        var devices = await _xiaomiClient.GetAllDevicesAsync();
        foreach (var deviceId in _devicesIds)
        {
            var device = devices.FirstOrDefault(d => d.Id == deviceId.XiaomiDeviceId);
            if (device is null)
            {
                continue;
            }

            var state = GetDeviceState(device);
            if (state is null)
            {
                continue;
            }
            
            _eventsService.Publish(new EntityStateChangedEvent(deviceId.EntityId, state));
        }
    }

    private static IEntityState? GetDeviceState(XiaomiDevice device)
    {
        if (device.Properties is null)
        {
            return null;
        }

        var model = device.Model.ToLower();
        if (model.Contains("gateway"))
        {
        }
        else if (model.Contains("sensor_ht"))
        {
            return new TemperatureEntityState
            {
                BatteryLevel = GetBatteryLevel(device.Properties),
                Temperature = GetTemperature(device.Properties),
                Humidity = GetHumidity(device.Properties)
            };
        }
        else if (model.Contains("magnet"))
        {
            return new BinaryEntityState
            {
                BatteryLevel = GetBatteryLevel(device.Properties),
                IsOn = GetMagnetSensorStatus(device.Properties)
            };
        }
        else if (model.Contains("motion"))
        {
            return new BinaryEntityState
            {
                BatteryLevel = GetBatteryLevel(device.Properties),
                IsOn = GetMotionSensorStatus(device.Properties)
            };
        }
        else if (model.Contains("sensor_switch"))
        {
        }
        else if (model.Contains("light"))
        {
        }
        else if (model.Contains("vacuum"))
        {
        }
        
        return null;
    }

    private static bool? GetMotionSensorStatus(IDictionary<string, string> properties)
    {
        var status = GetPropertyStringValue(properties, "status");
        if (string.IsNullOrWhiteSpace(status))
        {
            var noMotion = GetPropertyStringValue(properties, "no_motion");
            return !string.IsNullOrWhiteSpace(noMotion) ? false : null;
        }
        return status.ToLower() == "motion";
    }
    
    private static bool? GetMagnetSensorStatus(IDictionary<string, string> properties)
    {
        var status = GetPropertyStringValue(properties, "status");
        if (string.IsNullOrWhiteSpace(status))
        {
            return null;
        }
        return status.ToLower() == "open";
    }

    private static byte? GetBatteryLevel(IDictionary<string, string> properties)
    {
        var voltage = GetPropertyIntegerValue(properties, "voltage");
        return voltage switch
        {
            null => null,
            <= 2735 => 0,
            >= 3100 => 100,
            _ => (byte?)((((voltage - 2735) / (3100 - 2735)) * 100))
        };
    }

    private static double? GetTemperature(IDictionary<string, string> properties)
    {
        var temperature = GetPropertyIntegerValue(properties, "temperature");
        return temperature is not null ? temperature / 100 : null;
    }

    private static double? GetHumidity(IDictionary<string, string> properties)
    {
        var humidity = GetPropertyIntegerValue(properties, "humidity");
        return humidity is not null ? humidity / 100 : null;
    }

    private static int? GetPropertyIntegerValue(IDictionary<string, string> properties, string propertyName)
    {
        if (!properties.ContainsKey(propertyName) || string.IsNullOrWhiteSpace(properties[propertyName]))
        {
            return null;
        };
        return int.TryParse(properties[propertyName], out var number) ? number : null;
    }

    private static string? GetPropertyStringValue(IDictionary<string, string> properties, string propertyName)
    {
        return properties.ContainsKey(propertyName) ? properties[propertyName] : null;
    }
    
    private void OnChangeDeviceState(IEvent? payload)
    {
        if (payload is null || payload is not ChangeEntityStateEvent changeEntityStateEvent)
        {
            return;
        }

        //var state = changeEntityStateEvent.State as LightEntityState;
        //if (state is null)
        //{
        //    return;
        //}

        //var lightId = _lightsIds.FirstOrDefault(l => l.EntityId == changeEntityStateEvent.EntityId);
        //if (lightId is null)
        //{
        //    return;
        //}
        //var _ = (_hueClient!.UpdateLightAsync(lightId.HueId, new PhilipsHueUpdateLight
        //{
        //    On = state.TurnedOn is not null ? new() { TurnedOn = state.TurnedOn.Value } : null,
        //    Dimming = state.Brightness is not null ? new() { Brightness = state.Brightness.Value } : null
        //})).Result;
    }
    
    private async Task<(string? gatewayId, IEnumerable<DeviceId> devicesIds)> AddAllDevicesAsync(string username, string password, string serverCountryCode)
    {
        if (_xiaomiClient is null)
        {
            return (null, new List<DeviceId>());
        }

        var devices = await _xiaomiClient.GetAllDevicesFromCloudAsync(username!, password!, serverCountryCode);
        var gateway = devices.FirstOrDefault(d => d.Model.ToLower().Contains("gateway"));
        
        int deviceIndex = 1;
        var devicesIds = new List<DeviceId>();
        foreach (var device in devices)
        {
            var entity = MapCloudDeviceToEntity(device, deviceIndex++);
            if (entity is null)
            {
                continue;
            }

            await _registrationService.RegisterEntityAsync(entity, Id);
            var deviceId = GetLocalDeviceIdFromCloudDevice(device);
            devicesIds.Add(new DeviceId(deviceId, entity.Id));
        }
        return (gateway is not null ? gateway.LocalIp : null, devicesIds);
    }

    private static string GetLocalDeviceIdFromCloudDevice(XiaomiCloudDevice device)
    {
        return device.Id.StartsWith("lumi.") ? device.Id[("lumi.".Length)..] : device.Id;
    }

    private IEntity? MapCloudDeviceToEntity(XiaomiCloudDevice device, int index)
    {
        var model = device.Model.ToLower();
        if (model.Contains("gateway"))
        {
        }
        else if (model.Contains("sensor_ht"))
        {
            return MapTemperatureSensorToEntity(device, index);
        }
        else if (model.Contains("sensor_magnet"))
        {
            return MapMagnetSensorToEntity(device, index);
        }
        else if (model.Contains("sensor_motion"))
        {
            return MapMotionSensorToEntity(device, index);
        }
        else if (model.Contains("sensor_switch"))
        {
        }
        else if (model.Contains("light"))
        {
        }
        else if (model.Contains("vacuum"))
        {
        }
        return null;
    }

    private IEntity MapTemperatureSensorToEntity(XiaomiCloudDevice device, int index)
    {
        var name = !string.IsNullOrWhiteSpace(device.Name) ? device.Name : $"Temperature {index}";
        return new TemperatureEntity(ExtensionsHelper.GenerateEntityId(Id, EntityType.Temperature, name))
        {
            Name = name,
            Enabled = true
        };
    }
    
    private IEntity MapMotionSensorToEntity(XiaomiCloudDevice device, int index)
    {
        var name = !string.IsNullOrWhiteSpace(device.Name) ? device.Name : $"Motion {index}";
        return new BinaryEntity(ExtensionsHelper.GenerateEntityId(Id, EntityType.Binary, name), BinaryEntityType.Motion)
        {
            Name = name,
            Enabled = true
        };
    }
    
    private IEntity MapMagnetSensorToEntity(XiaomiCloudDevice device, int index)
    {
        var name = !string.IsNullOrWhiteSpace(device.Name) ? device.Name : $"Magnet {index}";
        return new BinaryEntity(ExtensionsHelper.GenerateEntityId(Id, EntityType.Binary, name), BinaryEntityType.Opening)
        {
            Name = name,
            Enabled = true
        };
    }
}
