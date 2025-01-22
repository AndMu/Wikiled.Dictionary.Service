using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Dictionary.Data
{
    public class Word
    {
        public Word(string text, Language language)
        {
            Guard.NotNullOrEmpty(() => text, text);
            Text = text;
            Language = language;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Language Language { get; }

        public string Text { get; }
    }
}
