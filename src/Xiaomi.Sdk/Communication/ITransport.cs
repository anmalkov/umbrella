using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xiaomi.Sdk.Commands;

namespace Xiaomi.Sdk.Communication;

internal interface ITransport : IDisposable
{
    public int SendCommand(ICommand command);

    public Task<string> ReceiveAsync();
}