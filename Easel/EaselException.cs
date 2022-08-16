using System;
using System.Runtime.Serialization;

namespace Easel;

public class EaselException : Exception
{
    public EaselException() { }
    protected EaselException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public EaselException(string message) : base(message) { }
    public EaselException(string message, Exception innerException) : base(message, innerException) { }
}