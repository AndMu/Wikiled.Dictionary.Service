using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Dictionary.Data
{
    public class TranslationResult
    {
        public TranslationResult(TranslationRequest request, string[] translations)
        {
            Guard.NotNull(() => request, request);
            Guard.NotNull(() => translations, translations);
            Request = request;
            Translations = translations;
        }

        public TranslationRequest Request { get; }

        public string[] Translations { get; }
    }
}
