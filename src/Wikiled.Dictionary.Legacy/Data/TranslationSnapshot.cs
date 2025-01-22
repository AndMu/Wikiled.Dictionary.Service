using System;
using System.Runtime.Serialization;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Data
{
    [Serializable]
    [DataContract]
    public class TranslationSnapshot
    {
        [DataMember]
        public TranslationData[] Information { get; set; }

        [DataMember]
        public WorldLanguage[] Languages{ get; set; }

        [DataMember]
        public WorldLanguage[] LatinLanguages { get; set; }

        [DataMember]
        public WorldLanguage[] NonLatinLanguages { get; set; }
    }
}