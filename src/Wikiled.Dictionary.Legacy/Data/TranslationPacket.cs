using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Data
{
    [DataContract]
    public class TranslationPacket
    {
        public TranslationPacket()
        {
        }

        public TranslationPacket(TranslationRequest request, Word word)
        {
            Request = request;
            Word = word;
        }

        [DataMember]
        public TranslationRequest Request { get; set; }

        [DataMember]
        public Word Word { get; set; }
    }
}
