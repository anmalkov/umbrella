using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Core.Extensions;

public interface IExtension
{
    string Id { get; }
    string DisplayName { get; }
    string Image { get; }
    string HtmlForRegistration { get; }

    Task RegisterAsync(Dictionary<string, string?>? parameters);
    Task UnregisterAsync(Dictionary<string, string?>? parameters);
}
