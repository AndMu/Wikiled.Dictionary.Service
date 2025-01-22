using System;
using System.Runtime.Serialization;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Data
{
    [DataContract]
    public class TranslationRequest : ICloneable
    {
        public TranslationRequest(Word source, WorldLanguage target)
        {
            Source = source;
            TargetLanguage = target;
        }

        [DataMember]
        public Word Source { get; set; }
        
        [DataMember]
        public WorldLanguage TargetLanguage { get; set; }

        public object Clone()
        {
            return new TranslationRequest(new Word(Source.Text, Source.Language), TargetLanguage);
        }

        public override string ToString()
        {
            return "Translation request: " + Source + " To: " + TargetLanguage;
        }
    }
}
