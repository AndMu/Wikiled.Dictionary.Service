using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public interface ITranslationHandler
    {
        void Init();

        int Count { get; }

        void UpdateRating(RatingPacket rating);

        SimpleTranslationResult GetTranslations(Word word);

        ILanguageDictionaryEx LeftLanguage { get; }

        ILanguageDictionaryEx RightLanguage { get; }

        string Name { get; }
    }
}