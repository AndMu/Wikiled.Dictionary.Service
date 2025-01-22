using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Data
{
    [DataContract]
    public class RatingPacket
    {
        [DataMember]
        public Word From { get; set; }

        [DataMember]
        public Word To { get; set; }

        [DataMember]
        public short Rating { get; set; }
    }
}