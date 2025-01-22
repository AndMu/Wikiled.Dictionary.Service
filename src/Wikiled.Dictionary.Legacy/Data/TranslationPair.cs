using System;
using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Data
{
    [Serializable]
    [DataContract]
    public class TranslationPair : ICloneable
    {
        public TranslationPair(Word value)
        {
            Value = value;
            Coeficients = new TranslationCoeficients();
        }

        [DataMember]
        public Word Value { get; set; }

        [DataMember]
        public TranslationCoeficients Coeficients { get; set; }

        public object Clone()
        {
            return new TranslationPair(Value)
            {
                Coeficients = Coeficients
            };
        }
    }
}
