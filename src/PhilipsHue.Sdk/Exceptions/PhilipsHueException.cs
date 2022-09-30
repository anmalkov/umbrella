using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipsHue.Sdk.Exceptions;

/// <summary>
/// Base exception for all exceptions thrown by the Philips Hue SDK.
/// </summary>
public class PhilipsHueException : Exception
{
	public PhilipsHueException() : base() { }
	public PhilipsHueException(string? message) : base(message) { }
    public PhilipsHueException(string? message, Exception? innerException) : base(message, innerException) { }
}
