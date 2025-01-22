using System.Collections.Generic;
using Wikiled.Dictionary.Legacy.Data;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public interface IDictionaryQuery
    {
        IEnumerable<WordLink> LoadAllWords(WorldLanguage language);

        Word ResolveMainWord(ILanguageDictionary dictionary, string word);

        void PutToProcess(ILanguageDictionary dictionary, string word, WorldLanguage toLanguage);

        List<Word> TranslateToLanguage(ILanguageDictionary dictionary, string word, ILanguageDictionary toLanguage);

        Word InsertResolveMainWord(ILanguageDictionary dictionary, string word);

        void DeleteTranslation(ILanguageDictionary dictionary, string from, string to, ILanguageDictionary toLanguage);

        bool WordExist(ILanguageDictionary dictionary, string word);

        List<Word> FindSimilar(ILanguageDictionary dictionary, string word, string command);

        void SetGoogleCount(TranslationRequest request, ILanguageDictionary dictionary, Word word, long total);

        void SetGoogleCount(ILanguageDictionary dictionary, Word word, long total);

        void ConfirmGoogleTranslation(TranslationRequest request, ILanguageDictionary dictionary, Word word);

        void SetGoogleCoef(TranslationRequest request, ILanguageDictionary dictionary, Word word, double total);
    }
}