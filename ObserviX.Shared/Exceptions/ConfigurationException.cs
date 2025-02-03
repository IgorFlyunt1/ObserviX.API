using System.Runtime.Serialization;

namespace ObserviX.Shared.Exceptions;

/// <summary>
    /// Represents an exception that occurs when a configuration error is encountered.
    /// </summary>
    [Serializable]
    public sealed class ConfigurationException : BaseException
    {
        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        public new string? Message { get; set; }

        /// <summary>
        /// Gets or sets the source of the configuration error.
        /// </summary>
        public new string? Source { get; set; }

        /// <summary>
        /// Gets or sets the configuration key that triggered the exception.
        /// </summary>
        public string? ConfigKey { get; set; }

        /// <summary>
        /// Gets or sets the status code representing the configuration error.
        /// Defaults to 500.
        /// </summary>
        public int StatusCode { get; set; } = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ConfigurationException(string message)
            : base(message)
        {
            Message = message;
            Source = null;
            ConfigKey = null;
            StatusCode = 500;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message and source.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="source">The source of the error.</param>
        public ConfigurationException(string message, string source)
            : this(message)
        {
            Source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message, source, and configuration key.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="source">The source of the error.</param>
        /// <param name="configKey">The configuration key related to the error.</param>
        public ConfigurationException(string message, string source, string configKey)
            : this(message, source)
        {
            ConfigKey = configKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message, source, configuration key, and status code.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="source">The source of the error.</param>
        /// <param name="configKey">The configuration key related to the error.</param>
        /// <param name="statusCode">The status code for the error.</param>
        public ConfigurationException(string message, string source, string configKey, int statusCode)
            : this(message, source, configKey)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
        /// <param name="context">The StreamingContext that contains contextual information.</param>
        private ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Message = info.GetString("Message");
            Source = info.GetString("Source");
            ConfigKey = info.GetString("ConfigKey");
        }

        /// <summary>
        /// When overridden in a derived class, sets the SerializationInfo with information about the exception.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
        /// <param name="context">The StreamingContext that contains contextual information.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Message", Message);
            info.AddValue("Source", Source);
            info.AddValue("ConfigKey", ConfigKey);
        }
    }