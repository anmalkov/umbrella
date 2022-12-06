using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Xiaomi.Sdk.Commands;
using Xiaomi.Sdk.Communication;
using Xiaomi.Sdk.Encryption;
using Xiaomi.Sdk.Exceptions;
using Xiaomi.Sdk.Models;
using Xiaomi.Sdk.Services;

namespace Xiaomi.Sdk;

public class XiaomiClient : IXiaomiClient
{
    private readonly HttpClient _httpClient;
    private readonly IXiaomiCloudClient _cloudClient;
    
    private CancellationTokenSource? _eventStreamCancellationTokenSource;
    private string? _ip;
    private ITransport? _transport = null;
    private bool _listeningForEvents => _transport is not null;
    private ConcurrentQueue<XiaomiResponse> _responses = new();

    private event Action<XiaomiResponse> _onCommandResponse;

    public event Action<XiaomiEventResponse> OnEventMessage;

    public XiaomiClient(HttpClient httpClient, string? ip = null)
    {
        if (ip is not null && string.IsNullOrWhiteSpace(ip))
        {
            throw new ArgumentNullException(nameof(ip));
        }
        _ip = ip;

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _cloudClient = new XiaomiCloudClient(httpClient);
    }

    public async Task<IEnumerable<XiaomiCloudDevice>> GetDevicesFromCloudAsync(string username, string password, string serverCountryCode)
    {
        return await _cloudClient.GetDevicesAsync(username, password, serverCountryCode);
    }

    public async Task<IEnumerable<XiaomiDevice>> GetAllDevicesAsync()
    {
        var response = await SendCommandAndWaitForReply(new GetIdListCommand());
        var devices = new List<XiaomiDevice>();
        if (response is null)
        {
            return devices;
        }

        var ids = new List<string> { response.Sid };
        if (!string.IsNullOrWhiteSpace(response?.Data))
        {
            var receivedIds = JsonSerializer.Deserialize<string[]>(response.Data);
            if (receivedIds is not null)
            {
                ids.AddRange(receivedIds);
            }
        }

        foreach (var id in ids)
        {
            devices.Add(await GetDeviceAsync(id));
        }

        return devices;
    }

    public async Task<XiaomiDevice?> GetDeviceAsync(string id)
    {
        var response = await SendCommandAndWaitForReply(new ReadCommand(id));
        if (response is null)
        {
            return null;
        }

        var properties = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(response?.Data))
        {
            using var jsonDocument = JsonDocument.Parse(response.Data);
            properties = jsonDocument.RootElement.EnumerateObject().ToDictionary(e => e.Name, e => e.Value.ValueKind.ToString() switch { 
                "Number" => e.Value.GetInt32().ToString(),
                _ => e.Value.GetString() ?? ""
            });
        }

        return new XiaomiDevice(
            response.Sid,
            response.Model,
            properties
        );
    }

    private async Task<XiaomiResponse?> SendCommandAndWaitForReply(ICommand command)
    {
        if (!_listeningForEvents)
        {
            StartListeningForEvents();
        }

        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

        var task = Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
            }
        });

        XiaomiResponse? receivedResponse = null;
        _onCommandResponse += (response) =>
        {
            if (response.Command == command.GetResponseCommandName())
            {
                receivedResponse = response;
                tokenSource.Cancel();
            }
        };
        _transport!.SendCommand(command);
        await task.WaitAsync(TimeSpan.FromSeconds(5));
        
        return receivedResponse;
    }

    public void StartListeningForEvents(CancellationToken? cancellationToken = null)
    {
        StopListeningForEvents();

        _eventStreamCancellationTokenSource = cancellationToken.HasValue
            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken!.Value)
            : new CancellationTokenSource();

        var tokenToCancel = _eventStreamCancellationTokenSource.Token;

        _transport = new UdpTransport(_ip, "test");

        StartListening(tokenToCancel);
    }

    private async Task StartListening(CancellationToken tokenToCancel)
    {
        try
        {
            while (!tokenToCancel.IsCancellationRequested)
            {
                var json = await _transport!.ReceiveAsync();
                var response = JsonSerializer.Deserialize<XiaomiResponse>(json);

                switch (response?.Command)
                {
                    case GetIdListCommand.ResponseCommandName:
                    case ReadCommand.ResponseCommandName:
                        if (_onCommandResponse is not null)
                        {
                            _onCommandResponse(response);
                        }
                        break;
                    case "report":
                        OnEventMessage?.Invoke(new XiaomiEventResponse());
                        break;
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore becasue task is canceled
        }
        finally
        {
            _transport?.Dispose();
            _transport = null;
        }
    }

    public void StopListeningForEvents()
    {
        _eventStreamCancellationTokenSource?.Cancel();
    }
}