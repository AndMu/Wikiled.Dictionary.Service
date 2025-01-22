using Wikiled.Dictionary.Legacy.Data;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public interface IManager
    {
        void Load(bool full);

        ILanguageDictionary this[WorldLanguage language] { get; }

        TranslationResult Translate(TranslationRequest request);

        Word[] FindSimilar(Word word, WorldLanguage withTranslation, int total);

        bool Exist(WorldLanguage language);

        WorldLanguage[] AllLanguages { get; }

        TranslationSnapshot Snapshot { get; }
    }
}