using PhilipsHue.Sdk.Exceptions;
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
    private CancellationTokenSource? _eventStreamCancellationTokenSource;

    public event Action<IEnumerable<PhilipsHueEventResponse>> OnEventMessage;

    
    private enum ResourceType
    {
        Light,
        ZigbeeConnectivity,
        Room,
        Zone
    }

    
    private bool BridgeRegistered => !string.IsNullOrEmpty(_appKey);


    public HueClient(HttpClient httpClient, string ip, string? appKey = null)
    {
        if (string.IsNullOrWhiteSpace(ip)) throw new ArgumentNullException(nameof(ip));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        
        _ip = ip;
        _appKey = appKey;
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

    public async Task<IEnumerable<PhilipsHueLight>> GetLightsAsync()
    {
        return await GetResourceAsync<PhilipsHueLight>(ResourceType.Light);
    }

    public async Task<PhilipsHueLight> GetLightAsync(Guid id)
    {
        var lights = await GetResourceAsync<PhilipsHueLight>(ResourceType.Light, id);
        return lights is not null && lights.Any() ? lights.First() : throw new PhilipsHueNotFoundException($"Light with id {id} not found");
    }

    public async Task<PhilipsHuePutResponse> UpdateLightAsync(Guid id, PhilipsHueUpdateLight data)
    {
        _httpClient.DefaultRequestHeaders.Remove("hue-application-key");
        _httpClient.DefaultRequestHeaders.Add("hue-application-key", _appKey);

        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var response = await _httpClient.PutAsJsonAsync(GetUri(ResourceType.Light, id), data, options);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new PhilipsHueException($"Response from the bridge has unexpected HTTP status code: {response.StatusCode}");
        }

        var philipsHueResponse = await response.Content.ReadFromJsonAsync<PhilipsHuePutResponse>();

        return philipsHueResponse is not null ? philipsHueResponse : new();
    }

    public async Task<IEnumerable<PhilipsHueRoom>> GetRoomsAsync()
    {
        return await GetResourceAsync<PhilipsHueRoom>(ResourceType.Room);
    }

    public async Task<PhilipsHueRoom> GetRoomAsync(Guid id)
    {
        var rooms = await GetResourceAsync<PhilipsHueRoom>(ResourceType.Room, id);
        return rooms is not null && rooms.Any() ? rooms.First() : throw new PhilipsHueNotFoundException($"Room with id {id} not found");
    }

    public async Task<IEnumerable<PhilipsHueZone>> GetZonesAsync()
    {
        return await GetResourceAsync<PhilipsHueZone>(ResourceType.Zone);
    }

    public async Task<PhilipsHueZone> GetZoneAsync(Guid id)
    {
        var zones = await GetResourceAsync<PhilipsHueZone>(ResourceType.Zone, id);
        return zones is not null && zones.Any() ? zones.First() : throw new PhilipsHueNotFoundException($"Zone with id {id} not found");
    }



    public async Task StartListeningForEventsAsync(CancellationToken? cancellationToken = null)
    {
        StopListeningForEvents();

        _eventStreamCancellationTokenSource = cancellationToken.HasValue
            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken!.Value)
            : new CancellationTokenSource();


        var tokenToCancel = _eventStreamCancellationTokenSource.Token;

        try
        {
            while (!tokenToCancel.IsCancellationRequested)
            {
                _httpClient.DefaultRequestHeaders.Remove("hue-application-key");
                _httpClient.DefaultRequestHeaders.Add("hue-application-key", _appKey);
                using (var reader = new StreamReader(await _httpClient.GetStreamAsync(GetEventsUri(), tokenToCancel)))
                {
                    while (!reader.EndOfStream)
                    {
                        var json = await reader.ReadLineAsync();
                        if (json is not null)
                        {
                            var events = JsonSerializer.Deserialize<IEnumerable<PhilipsHueEventResponse>>(json);
                            if (events is not null && events.Any())
                            {
                                OnEventMessage?.Invoke(events);
                            }
                        }
                    }
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore becasue task is canceled
        }
    }

    public void StopListeningForEvents()
    {
        _eventStreamCancellationTokenSource?.Cancel();
    }

    public void Dispose()
    {
        StopListeningForEvents();
    }


    public async Task<IEnumerable<T>> GetResourceAsync<T>(ResourceType type, Guid? id = null)
    {
        _httpClient.DefaultRequestHeaders.Remove("hue-application-key");
        _httpClient.DefaultRequestHeaders.Add("hue-application-key", _appKey);

        var response = await _httpClient.GetAsync(GetUri(type, id));
        if (!response.IsSuccessStatusCode)
        {
            throw new PhilipsHueException($"Response from the bridge has unexpected HTTP status code: {response.StatusCode}");
        }

        var philipsHueResponse = await response.Content.ReadFromJsonAsync<PhilipsHueResponse<T>>();

        return philipsHueResponse is not null ? philipsHueResponse.Data : new();
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
           ResourceType.Room => "/room",
           ResourceType.Zone => "/zone",
            ResourceType.ZigbeeConnectivity => "/zigbee_connectivity",
            _ => ""
        };
        var resourceIdPart = id is null ? "" : $"/{id}";
        return new Uri($"https://{_ip}/clip/v2/resource{resourceTypePart}{resourceIdPart}");
    }
    
    private Uri GetEventsUri() => new($"https://{_ip}/eventstream/clip/v2");
}