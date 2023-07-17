using System.Runtime.Serialization;

namespace OracleHelper
{
    /// <summary>
    /// Oracle Helper的Exceptions
    /// </summary>
    public class OracleHelperException : Exception, ISerializable
    {
        public OracleHelperException(string ex)
            : base(ex)
        {

        }
        public OracleHelperException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
