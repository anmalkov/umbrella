using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaomi.Sdk.Commands;

public class ReadCommand : ICommand
{
    public const string ResponseCommandName = "read_ack";
    
    private readonly string _sid;

    
    public ReadCommand(string sid)
    {
        _sid = sid;
    }
    
    
    public string? GetResponseCommandName() => ResponseCommandName;

    public override string ToString()
    {
        return $"{{\"cmd\":\"read\",\"sid\":\"{_sid}\"}}";
    }
}
