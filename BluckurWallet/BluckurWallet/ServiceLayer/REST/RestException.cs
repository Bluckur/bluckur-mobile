using System;

namespace BluckurWallet.ServiceLayer.Rest
{
    /// <summary>
    /// Exception thrown by Rest consumers when the request failed.
    /// <para>
    /// This exception can be used to wrap deeper 'problems' that aren't relevant for the front end.
    /// Think of errors such as incorrect data, connection being actively refused or timed out.
    /// </para>
    /// </summary>
    public class RestException : Exception
    {
        public RestException() { }

        public RestException(string message) : base(message) { }

        public RestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
