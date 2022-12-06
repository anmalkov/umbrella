using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xiaomi.Sdk.Commands;

namespace Xiaomi.Sdk.Communication;

internal class UdpTransport : ITransport
{
    private readonly string _gatewayPassword;
    private readonly IPAddress _gatewayIp;
    private readonly string _multicastAddress;
    private readonly int _serverPort;
    private readonly UdpClient _udpClient;

    public string Token { get; set; }

    public UdpTransport(string gatewayIp, string gatewayPassword, string multicastAddress = "224.0.0.50", int serverPort = 9898)
    {
        _gatewayPassword = gatewayPassword;
        _gatewayIp = IPAddress.Parse(gatewayIp);
        _multicastAddress = multicastAddress;
        _serverPort = serverPort;

        _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _serverPort));
        _udpClient.JoinMulticastGroup(IPAddress.Parse(_multicastAddress));
    }

    public int SendCommand(ICommand command)
    {
        var buffer = Encoding.ASCII.GetBytes(command.ToString());
        return _udpClient.Send(buffer, buffer.Length, new IPEndPoint(_gatewayIp, _serverPort));
    }

    //public int SendWriteCommand(string sid, string type, Command data)
    //{
    //    var key = CryptoProvider.BuildKey(Token, _gwPassword).ToHex();

    //    return SendCommand(new WriteCommand(sid, type, key, data));
    //}

    public async Task<string> ReceiveAsync()
    {
        var data = (await _udpClient.ReceiveAsync()).Buffer;
        return Encoding.ASCII.GetString(data);
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
    }
}