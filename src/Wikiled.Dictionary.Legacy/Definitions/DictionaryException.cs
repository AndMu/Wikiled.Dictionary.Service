using System;
using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    [Serializable]
    public class DictionaryException : Exception
    {
        public DictionaryException()
        {
        }

        public DictionaryException(string message)
            : base(message)
        {
        }

        public DictionaryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected DictionaryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
