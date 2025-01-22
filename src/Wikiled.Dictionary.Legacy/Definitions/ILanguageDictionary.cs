using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public interface ILanguageDictionary
    {
        TranslationResult Translate(TranslationRequest request);

        Word FindSimilarWord(string word, WorldLanguage? hasTranslation, int total = 4);

        Word[] FindSimilar(string word, WorldLanguage withTranslation, int total);

        Word FindWord(Word word);

        Word FindWordById(int id);

        Word FindWordByText(string text);

        void Load();

        Languages Description { get; }

        WorldLanguage Language { get; }

        int LinkCounts { get; }

        int TotalWords { get; }

        WordLink this[Word word] { get; }
    }
}