using System.Net.Http;
using System.Text.Json;
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
            devicesIds.Add(new DeviceId(device.Id, entity.Id));
        }
        return (gateway is not null ? gateway.LocalIp : null, devicesIds);
    }

    private IEntity? MapCloudDeviceToEntity(XiaomiCloudDevice device, int index)
    {
        if (device.Model.ToLower().Contains("gateway"))
        {
        }
        else if (device.Model.ToLower().Contains("sensor_ht"))
        {
            return MapTemperatureSensorToEntity(device, index);
        }
        else if (device.Model.ToLower().Contains("sensor_magnet"))
        {
            return MapMagnetSensorToEntity(device, index);
        }
        else if (device.Model.ToLower().Contains("sensor_motion"))
        {
            return MapMotionSensorToEntity(device, index);
        }
        else if (device.Model.ToLower().Contains("sensor_switch"))
        {
        }
        else if (device.Model.ToLower().Contains("light"))
        {
        }
        else if (device.Model.ToLower().Contains("vacuum"))
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
