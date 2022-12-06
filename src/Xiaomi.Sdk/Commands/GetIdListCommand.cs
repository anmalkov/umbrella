using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaomi.Sdk.Commands;

public class GetIdListCommand : ICommand
{
    public const string ResponseCommandName = "get_id_list_ack";
    

    public string? GetResponseCommandName() => ResponseCommandName;

    public override string ToString()
    {
        return "{\"cmd\":\"get_id_list\"}";
    }
}
