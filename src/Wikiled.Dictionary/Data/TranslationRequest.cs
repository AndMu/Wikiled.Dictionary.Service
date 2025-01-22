using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Wikiled.Dictionary.Data
{
    public class TranslationRequest
    {
        [Required]
        public string Word { get; set; }
         
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Language From { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public Language To { get; set; }

        public override string ToString()
        {
            return $"{From}-{To}:{Word}";
        }
    }
}
