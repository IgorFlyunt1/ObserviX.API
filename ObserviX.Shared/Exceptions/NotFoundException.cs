using System.Runtime.Serialization;

namespace ObserviX.Shared.Exceptions;

/// <summary>
///     The entity not found exception.
/// </summary>
[Serializable]
public sealed class NotFoundException : BaseException
{
    public new string? Message { get; set; }
    public new string? Source { get; set; }
    public string? SourceValue { get; set; }
    public int StatusCode { get; set; } = 404;
        
    public NotFoundException(string message)
        : base(message)
    {
        Message = message;
        Source = null;
        SourceValue = null;
        StatusCode = 404;
    }

    public NotFoundException(string message, string source)
        : this(message)
    {
        Source = source;
    }

    public NotFoundException(string message, string source, string sourceValue)
        : this(message, source)
    {
        SourceValue = sourceValue;
    }

    public NotFoundException(string message, string source, string sourceValue, int statusCode)
        : this(message, source, sourceValue)
    {
        StatusCode = statusCode;
    }


    private NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Message = info.GetString("Message");
        Source = info.GetString("Source");
        SourceValue = info.GetString("SourceValue");
    }

    // The serialization function
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Message", Message);
        info.AddValue("Source", Source);
        info.AddValue("SourceValue", SourceValue);
    }
}