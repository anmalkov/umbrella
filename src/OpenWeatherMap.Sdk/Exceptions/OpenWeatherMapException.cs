using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherMap.Sdk.Exceptions;

/// <summary>
/// Base exception for all exceptions thrown by the OpenWeatherMap SDK.
/// </summary>
public class OpenWeatherMapException : Exception
{
	public OpenWeatherMapException() : base() { }
	public OpenWeatherMapException(string? message) : base(message) { }
    public OpenWeatherMapException(string? message, Exception? innerException) : base(message, innerException) { }
}
