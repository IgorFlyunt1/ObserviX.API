using System.Runtime.Serialization;

namespace ObserviX.Shared.Exceptions;

/// <summary>
///     Base functionality for exceptions.
/// </summary>
[Serializable]
public class BaseException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseException"/> class.
    /// </summary>
    protected BaseException()
    {
    }
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected BaseException(string message)
        : base(message)
    {
    }
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseException"/> class with a specified source and error message.
    /// </summary>
    /// <param name="source">The source of the error.</param>
    /// <param name="message">The message that describes the error.</param>
    protected BaseException(string source, string message)
        : this(message)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Source = source;
    }
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The serialized object data about the exception being thrown.</param>
    /// <param name="context">The context refference that contains contextual information about the source or destination.</param>
    protected BaseException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}