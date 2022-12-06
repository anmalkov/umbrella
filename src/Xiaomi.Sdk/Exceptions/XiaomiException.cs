using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xiaomi.Sdk.Exceptions;

public class XiaomiException : Exception
{
    public XiaomiException() : base() { }
    public XiaomiException(string? message) : base(message) { }
    public XiaomiException(string? message, Exception? innerException) : base(message, innerException) { }
}

