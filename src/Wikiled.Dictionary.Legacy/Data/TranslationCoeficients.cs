using System;
using System.Runtime.Serialization;

namespace Wikiled.Dictionary.Legacy.Data
{
    [DataContract]
    public class TranslationCoeficients : ICloneable
    {
        [DataMember]
        public double? GoogleCoeficients { get; set; }

        [DataMember]
        public long? GoogleCount { get; set; }

        [DataMember]
        public short? CustomerRating { get; set; }

        [DataMember]
        public double? OverallRating { get; set; }

        [DataMember]
        public bool IsGoogleConfirmed { get; set; }

        public object Clone()
        {
            return new TranslationCoeficients
            {
                GoogleCoeficients = GoogleCoeficients,
                GoogleCount = GoogleCount,
                CustomerRating = CustomerRating,
                OverallRating = OverallRating,
                IsGoogleConfirmed = IsGoogleConfirmed
            };
        }
    }
}
