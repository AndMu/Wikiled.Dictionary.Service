using System;
using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Data
{
    [Serializable]
    [DataContract]
    public class TranslationResult
    {
        public TranslationResult()
        {
        }

        public TranslationResult(TranslationRequest request)
        {
            Request = request;
            Translations = new TranslationPair[] {};
            Similar = new Word[] {};
        }

        [DataMember]
        public TranslationRequest Request { get; set; }

        [DataMember]
        public Word[] Similar { get; set; }

        [DataMember]
        public TranslationPair[] Translations { get; set; }
    }
}
