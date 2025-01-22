using System;
using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Helpers
{
    [Serializable]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class WikiException : Exception
    {
        public WikiException(string message)
            : base(message)
        { }

        public WikiException()
        { }

        public WikiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public WikiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Any other custom data to be transferred
            // info.AddValue(CustomDataName, DataValue);
        }
    }
}
