using System;
using System.Runtime.Serialization;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Data
{
    [Serializable]
    [DataContract]
    public class TranslationData : ICloneable
    {
        public TranslationData(WorldLanguage from, WorldLanguage to, int total)
        {
            FromLanguage = from;
            ToLanguage = to;
            Total = total;
        }

        [DataMember]
        public WorldLanguage ToLanguage { get; set; }

        [DataMember]
        public WorldLanguage FromLanguage { get; set; }

        [DataMember]
        public int Total { get; set; }

        public object Clone()
        {
            return new TranslationData(FromLanguage, ToLanguage, Total);
        }
    }
}