using System;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Xiaomi.Sdk.Encryption;
using Xiaomi.Sdk.Exceptions;
using Xiaomi.Sdk.Models;
using Xiaomi.Sdk.Services;

namespace Xiaomi.Sdk;

public class XiaomiCloudClient : IXiaomiCloudClient
{
    private readonly HttpClient _httpClient;
    private readonly CookiesService _cookiesService = new();
    private readonly Random _random = new();
    private readonly string _agentName;

    private string? _userId = null;
    private string? _accessToken = null;
    private string? _securityCode = null;

    private bool _loggedIn => !string.IsNullOrWhiteSpace(_userId) && !string.IsNullOrWhiteSpace(_accessToken) && !string.IsNullOrWhiteSpace(_securityCode);

    public XiaomiCloudClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _agentName = GenerateAgentName();
    }

    
    public async Task<IEnumerable<XiaomiCloudDevice>> GetAllDevicesAsync(string username, string password, string serverCountryCode)
    {
        if (!_loggedIn)
        {
            await LoginToCloudAsync(username, password);
        }

        return await GetCloudDevicesAsync(serverCountryCode);
    }
    

    private async Task<IEnumerable<XiaomiCloudDevice>> GetCloudDevicesAsync(string serverCountryCode)
    {
        var serverPart = serverCountryCode == "cn" ? "" : serverCountryCode + ".";
        var url = $"https://{serverPart}api.io.mi.com/app/home/device_list";
        var parameters = new Dictionary<string, string>
        {
            { "data", "{\"getVirtualModel\":true,\"getHuamiDevices\":1,\"get_split_device\":false,\"support_smart_home\":true}" }
        };
        try
        {
            var rawJson = await SendEncryptedRequestToCloudAsync(url, parameters);
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return new List<XiaomiCloudDevice>();
            }

            var data = JsonDocument.Parse(rawJson);
            rawJson = data.RootElement.GetProperty("result").GetProperty("list").GetRawText();
            var devices = JsonSerializer.Deserialize<IEnumerable<XiaomiCloudDevice>>(rawJson);
            return devices ?? new List<XiaomiCloudDevice>();
        }
        catch (XiaomiException ex)
        {
            throw new XiaomiException($"Failed to get devices from cloud. {ex.Message}", ex);
        }
    }

    private async Task<string?> SendEncryptedRequestToCloudAsync(string url, Dictionary<string, string> parameters)
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept-Encoding", "identity" },
            { "User-Agent", _agentName },
            { "x-xiaomi-protocal-flag-cli", "PROTOCAL-HTTP2" },
            { "MIOT-ENCRYPT-ALGORITHM", "ENCRYPT-RC4" },
            { "Accept", "*/*" },
            { "Connection", "keep-alive" }
        };

        _cookiesService.AddCookie("userId", _userId!);
        _cookiesService.AddCookie("yetAnotherServiceToken", _accessToken!);
        _cookiesService.AddCookie("serviceToken", _accessToken!);
        _cookiesService.AddCookie("locale", "en_GB");
        _cookiesService.AddCookie("timezone", "GMT+02:00");
        _cookiesService.AddCookie("is_daylight", "1");
        _cookiesService.AddCookie("dst_offset", "3600000");
        _cookiesService.AddCookie("channel", "MI_APP_STORE");

        var millis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var nonce = GenerateNonce(millis);
        var signedNonce = GetSignedNonce(nonce);
        var encodedParameters = EncodeParameters(url, "POST", parameters, nonce, signedNonce);
        var response = await SendRequestToCloudAsync(url, headers, encodedParameters, isPostRequest: true);
        if (!response.IsSuccessStatusCode)
        {
            throw new XiaomiException("HTTP status: {response.StatusCode}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var rawJson = Decrypt(content, signedNonce);
        return rawJson;
    }
    
    private static string GenerateNonce(long milliseconds)
    {
        var minutes = (int)(milliseconds / 60000);
        var minutesBytes = BitConverter.GetBytes(minutes);
        if (BitConverter.IsLittleEndian)
        {
            minutesBytes = minutesBytes.Reverse().ToArray();
        }
        var nonce = RandomNumberGenerator.GetBytes(8).Concat(minutesBytes).ToArray();
        return Convert.ToBase64String(nonce);
    }

    private string GetSignedNonce(string nonce)
    {
        var payload = Convert.FromBase64String(_securityCode!).Concat(Convert.FromBase64String(nonce)).ToArray();
        var hash = SHA256.Create().ComputeHash(payload);
        return Convert.ToBase64String(hash);
    }

    private IDictionary<string, string> EncodeParameters(string url, string method, IDictionary<string, string> parameters, string nonce, string signedNonce)
    {
        var encodedParameters = new Dictionary<string, string>();
        parameters.Add("rc4_hash__", GetSignature(url, method, parameters, signedNonce));
        foreach (var parameter in parameters)
        {
            encodedParameters.Add(parameter.Key, Encrypt(parameter.Value, signedNonce));
        }
        encodedParameters.Add("signature", GetSignature(url, method, encodedParameters, signedNonce));
        encodedParameters.Add("ssecurity", _securityCode!);
        encodedParameters.Add("_nonce", nonce);
        return encodedParameters;
    }

    private static string Encrypt(string value, string password)
    {
        var algo = new Rc4(Convert.FromBase64String(password));
        algo.Cipher(new byte[1024]);
        var encryptedValue = algo.Cipher(value);
        return Convert.ToBase64String(encryptedValue);
    }

    private static string Decrypt(string value, string password)
    {
        var algo = new Rc4(Convert.FromBase64String(password));
        algo.Cipher(new byte[1024]);
        var encryptedValue = algo.Cipher(Convert.FromBase64String(value));
        return Encoding.UTF8.GetString(encryptedValue);
    }

    private static string GetSignature(string url, string method, IDictionary<string, string> parameters, string signedNonce)
    {
        var signatureParameters = new List<string>
        {
            method.ToUpper(),
            url.Split("com")[1].Replace("/app/", "/")
        };
        foreach (var parameter in parameters)
        {
            signatureParameters.Add($"{parameter.Key}={parameter.Value}");
        }
        signatureParameters.Add(signedNonce);
        var signature = string.Join('&', signatureParameters);
        var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(signature));
        return Convert.ToBase64String(hash);
    }

    private async Task LoginToCloudAsync(string username, string password)
    {
        var deviceId = GenerateDeviceId();
        
        _cookiesService.AddCookie("sdkVersion", "accountsdk-18.8.15");
        _cookiesService.AddCookie("deviceId", deviceId);
        _cookiesService.AddCookie("userId", username);
        
        var sign = await GetSignAsync(username);
        (var url, _userId, _securityCode) = await GetAccessTokenUrlAsync(username, password, sign);
        _accessToken = await GetAccessTokenAsync(url!);
    }

    private async Task<string> GetSignAsync(string username)
    {
        var url = "https://account.xiaomi.com/pass/serviceLogin?sid=xiaomiio&_json=true";
        var headers = new Dictionary<string, string> {{ "User-Agent", _agentName }};
        _cookiesService.AddCookie("userId", username);
        var response = await SendRequestToCloudAsync(url, headers, isPostRequest: false);
        if (!response.IsSuccessStatusCode)
        {
            throw new XiaomiException("Username is incorrect. HTTP status: {response.StatusCode}");
        }
        
        _cookiesService.AddCookiesFrom(response);
        var json = await GetJsonDocumentFrom(response);
        var sign = json.RootElement.GetProperty("_sign").GetString();
        if (string.IsNullOrWhiteSpace(sign))
        {
            throw new XiaomiException("Username is incorrect");
        }
        
        return sign;
    }

    private async Task<(string? url, string? userId, string? securityCode)> GetAccessTokenUrlAsync(string username, string password, string sign)
    {
        var url = "https://account.xiaomi.com/pass/serviceLoginAuth2";
        var headers = new Dictionary<string, string> { { "User-Agent", _agentName } };
        var parameters = new Dictionary<string, string>
        {
            { "sid", "xiaomiio" },
            { "hash", GeneratePasswordHash(password) },
            { "callback", "https://sts.api.io.mi.com/sts" },
            { "qs", "%3Fsid%3Dxiaomiio%26_json%3Dtrue" },
            { "user", username },
            { "_sign", sign },
            { "_json", "true" }
        };
        var response = await SendRequestToCloudAsync(url, headers, parameters, isPostRequest: true);
        if (!response.IsSuccessStatusCode)
        {
            throw new XiaomiException("Username or password is incorrect. HTTP status: {response.StatusCode}");
        }

        _cookiesService.AddCookiesFrom(response);

        var json = await GetJsonDocumentFrom(response);
        var ssecurity = json.RootElement.GetProperty("ssecurity").GetString();
        if (string.IsNullOrWhiteSpace(ssecurity) || ssecurity.Length > 4)
        {
            var notificationUrl = json.RootElement.GetProperty("notificationUrl").GetString();
            if (!string.IsNullOrWhiteSpace(notificationUrl))
            {
                throw new XiaomiException($"Two factor authentication is required. Please use following url: {notificationUrl}");
            }
            throw new XiaomiException("Username or password is incorrect. Unknown response from server");
        }
        
        var userId = json.RootElement.GetProperty("userId").GetInt64();
        //var cUserId = json.RootElement.GetProperty("cUserId").GetString();
        //var passToken = json.RootElement.GetProperty("passToken").GetString();
        var location = json.RootElement.GetProperty("location").GetString();
        //var code = json.RootElement.GetProperty("code").GetInt32();

        return (location, userId.ToString(), ssecurity);
    }
    
    private async Task<string> GetAccessTokenAsync(string url)
    {
        var headers = new Dictionary<string, string> { { "User-Agent", _agentName } };
        var response = await SendRequestToCloudAsync(url, headers, isPostRequest: false);
        if (!response.IsSuccessStatusCode)
        {
            throw new XiaomiException($"Failed to get access token. HTTP status: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        if (content != "ok")
        {
            throw new XiaomiException($"Failed to get access token. Unexpected response from server: {content}");
        }

        _cookiesService.AddCookiesFrom(response);
        var accessToken = _cookiesService.GetCookie("serviceToken");
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new XiaomiException("Failed to get access token");
        }

        return accessToken;
    }

    private static string GeneratePasswordHash(string password)
    {
        return string.Concat(MD5.HashData(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2"))).ToUpper();
    }

    private async Task<HttpResponseMessage> SendRequestToCloudAsync(string url, IDictionary<string, string>? headers = null,
        IDictionary<string, string>? parameters = null, bool isPostRequest = false)
    {
        var request = new HttpRequestMessage(isPostRequest ? HttpMethod.Post : HttpMethod.Get, url);
        if (parameters is not null)
        {
            request.Content = new FormUrlEncodedContent(parameters);
        }
        request.Headers.Clear();
        if (headers is not null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        _cookiesService.AddCookiesToRequest(request);
        return await _httpClient.SendAsync(request);
    }

    private static async Task<JsonDocument> GetJsonDocumentFrom(HttpResponseMessage response)
    {
        const string startText = "&&&START&&&";
        var content = await response.Content.ReadAsStringAsync();
        if (content.StartsWith(startText))
        {
            content = content[(startText.Length)..];
        }
        return JsonDocument.Parse(content);
    }

    private string GenerateDeviceId()
    {
        return string.Join("", Enumerable.Repeat(0, 6).Select(n => (char)_random.Next(97, 122)));
    }

    private string GenerateAgentName()
    {
        var id = string.Join("", Enumerable.Repeat(0, 13).Select(n => (char)_random.Next(65, 69)));
        return $"Android-7.1.1-1.0.0-ONEPLUS A3010-136-{id} APP/xiaomi.smarthome APPV/62830";
    }
}