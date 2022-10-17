using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Exceptions;

/// <summary>
/// Indicates that user has not pressed the link button on the bridge.
/// </summary>
public sealed class PhilipsHueNotFoundException : PhilipsHueException
{
    public PhilipsHueNotFoundException() : base("Resource not found") { }
    public PhilipsHueNotFoundException(string? message) : base(message) { }
    public PhilipsHueNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}
