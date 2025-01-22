using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Service
{
    public interface ITranslationService
    {
        WorldLanguage[] AllLanguages { get; }

        bool Exist(WorldLanguage language);

        TranslationResult Translate(TranslationRequest request);

        TranslationData[] GetTranslationInfo(WorldLanguage language);

        Word[] FindSimilar(Word word, WorldLanguage withTranslation, int total);

        Word ResolveWord(WorldLanguage language, int id);

        bool IsLatin(WorldLanguage language);
    }
}