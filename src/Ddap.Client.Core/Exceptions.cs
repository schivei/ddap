namespace Ddap.Client.Core;

/// <summary>
/// Base exception for DDAP client errors
/// </summary>
public class DdapClientException : Exception
{
    public DdapClientException() { }

    public DdapClientException(string message)
        : base(message) { }

    public DdapClientException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a connection to the API fails
/// </summary>
public class DdapConnectionException : DdapClientException
{
    public DdapConnectionException() { }

    public DdapConnectionException(string message)
        : base(message) { }

    public DdapConnectionException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when an API request fails
/// </summary>
public class DdapApiException : DdapClientException
{
    public int? StatusCode { get; set; }

    public DdapApiException() { }

    public DdapApiException(string message)
        : base(message) { }

    public DdapApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public DdapApiException(string message, Exception innerException)
        : base(message, innerException) { }
}
