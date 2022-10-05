using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Core.Extensions;

namespace Umbrella.Core.Models;

public class Extension
{
    public string Id { get; init; }

    public string? DisplayName { get; set; }

    public string? Image { get; set; }

    public string? HtmlForRegistration { get; set; }

    public Extension(string id)
    {
        Id = id;
    }
}
