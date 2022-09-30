﻿using PhilipsHue.Sdk.Exceptions;
using PhilipsHue.Sdk.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PhilipsHue.Sdk;

public class HueClient : IHueClient
{
    private readonly string _ip;
    private readonly HttpClient _httpClient;

    private string? _appKey;
    private string? _clientKey;

    private enum ResourceType
    {
        Light
    }

    private bool BridgeRegistered => !string.IsNullOrEmpty(_appKey);


    public HueClient(HttpClient httpClient, string ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) throw new ArgumentNullException(nameof(ip));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ip = ip;
    }
    

    public async Task<PhilipsHueRegistrationInfo?> RegisterAsync(string applicationName, string deviceName)
    {
        if (applicationName is null) throw new ArgumentNullException(nameof(applicationName));
        if (string.IsNullOrWhiteSpace(applicationName)) throw new ArgumentException($"{nameof(applicationName)} must have a value", nameof(applicationName));

        if (deviceName is null) throw new ArgumentNullException(nameof(deviceName));
        if (string.IsNullOrWhiteSpace(deviceName)) throw new ArgumentException($"{nameof(deviceName)} must have a value", nameof(deviceName));

        Unregister();

        var response = await _httpClient.PostAsJsonAsync(GetV1Uri(), new { devicetype = $"{applicationName}#{deviceName}", generateclientkey = true });
        if (!response.IsSuccessStatusCode)
        {
            throw new PhilipsHueException($"Response from the bridge has unexpected HTTP status code: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new PhilipsHueException("Empty response from the bridge");
        }

        var jsonObject = json.StartsWith('[') ? JsonSerializer.Deserialize<JsonArray>(json)?[0] as JsonObject : JsonSerializer.Deserialize<JsonObject>(json);
        if (jsonObject is null)
        {
            throw new PhilipsHueException("Response from the bridge is not in JSON format");
        }

        if (jsonObject.ContainsKey("error"))
        {
            var type = (int?)jsonObject["error"]?["type"];
            if (type is not null && type == 101)
            {
                throw new PhilipsHueLinkButtonNotPressedException();
            }
            else
            {
                throw new PhilipsHueException($"Error {jsonObject["error"]?["type"]}: {jsonObject["error"]?["description"]}");
            }
        }

        if (!jsonObject.ContainsKey("success"))
        {
            throw new PhilipsHueException("Unrecognized response from the bridge");
        }

        _appKey = (string?)jsonObject["success"]?["username"];
        _clientKey = (string?)jsonObject["success"]?["clientkey"];

        return BridgeRegistered ? new PhilipsHueRegistrationInfo(_appKey!, _clientKey!) : null;
    }

    private void Unregister()
    {
        if (!BridgeRegistered)
        {
            return;
        }

        _appKey = null;
        _clientKey = null;
    }

    private Uri GetV1Uri()
    {
        var appKeyPart = string.IsNullOrWhiteSpace(_appKey) ? "" : $"{_appKey}/";
        return new Uri($"https://{_ip}/api/{appKeyPart}");
    }
    private Uri GetUri(ResourceType type, Guid? id = null)
    {
        var resourceTypePart = type switch
        {
           ResourceType.Light => "/light",
           _ => ""
        };
        var resourceIdPart = id is null ? "" : $"/{id}";
        return new Uri($"https://{_ip}/clip/v2/resource{resourceTypePart}{resourceIdPart}");
    }

    public async Task<IEnumerable<PhilipsHueLight>> GetLightsAsync()
    {
        _httpClient.DefaultRequestHeaders.Add("hue-application-key", _appKey);
        
        var response = await _httpClient.GetAsync(GetUri(ResourceType.Light));
        if (!response.IsSuccessStatusCode)
        {
            throw new PhilipsHueException($"Response from the bridge has unexpected HTTP status code: {response.StatusCode}");
        }

        var philipsHueResponse = await response.Content.ReadFromJsonAsync<PhilipsHueResponse<PhilipsHueLight>>();

        return philipsHueResponse is not null ? philipsHueResponse.Data : new();
    }
}