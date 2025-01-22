using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Translation
{
    [DataContract]
    public enum WordSources
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        GoogleTranslated = 30,
        [EnumMember]
        Confirmed = 31
    }
}