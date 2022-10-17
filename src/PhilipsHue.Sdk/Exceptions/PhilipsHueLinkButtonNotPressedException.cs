using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Exceptions;

/// <summary>
/// Indicates that user has not pressed the link button on the bridge.
/// </summary>
public sealed class PhilipsHueLinkButtonNotPressedException : PhilipsHueException
{
    public PhilipsHueLinkButtonNotPressedException() : base("Link button not pressed") { }
    public PhilipsHueLinkButtonNotPressedException(string? message) : base(message) { }
    public PhilipsHueLinkButtonNotPressedException(string? message, Exception? innerException) : base(message, innerException) { }
}
