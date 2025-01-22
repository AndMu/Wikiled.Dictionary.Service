using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Translation;

namespace Wikiled.Dictionary.Legacy.Definitions.Redis.Transfer
{
    public class RedisLanguageDictionary : ILanguageDictionaryEx
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly List<Word> allWords = new List<Word>();

        private readonly IDictionaryQuery dbQuery;

        private readonly LanguageDictionary dictionary;

        private readonly IWordRepository repository;

        private readonly ITranslationHandlerFactory translationHandler;

        public RedisLanguageDictionary(
            Languages language,
            IWordRepository repository,
            IDictionaryQuery dbQuery,
            ITranslationHandlerFactory translationHandler)
        {
            Guard.NotNull(() => language, language);
            Guard.NotNull(() => dbQuery, dbQuery);
            Guard.NotNull(() => repository, repository);
            dictionary = new LanguageDictionary(language, dbQuery, translationHandler);
            Description = language;
            Language = language.Language.Convert();
            this.repository = repository;
            this.dbQuery = dbQuery;
            this.translationHandler = translationHandler;
        }

        public Languages Description { get; }

        public WorldLanguage Language { get; }

        public int LinkCounts { get; }

        public int TotalWords { get; }

        public WordLink this[Word word] => dictionary[word];

        public Word[] FindSimilar(string word, WorldLanguage withTranslation, int total)
        {
            throw new NotSupportedException();
        }

        public Word FindSimilarWord(string word, WorldLanguage? hasTranslation, int total = 4)
        {
            throw new NotSupportedException();
        }

        public Word FindWord(Word word)
        {
            return dictionary.FindWord(word);
        }

        public Word FindWordById(int id)
        {
            return dictionary.FindWordById(id);
        }

        public Word FindWordByText(string text)
        {
            return dictionary.FindWordByText(text);
        }

        public void InitTranslations(IEnumerable<ILanguageDictionaryEx> otherDictionaries)
        {
            if (Language != WorldLanguage.English)
            {
                return;
            }

            foreach (var currentDictionary in otherDictionaries.Where(
                item => item.Language != WorldLanguage.English))
            {
                foreach (var word in allWords)
                {
                    var translation = translationHandler
                        .Create(this, currentDictionary)
                        .GetTranslations(word);
                    repository.Save(translation);
                }
            }
        }

        public void Load()
        {
            int total = 0;
            dictionary.Load();
            foreach (var word in dbQuery.LoadAllWords(Language).OrderBy(item => item.Value.Text))
            {
                total++;
                repository.Save(word.Value);
                allWords.Add(word.Value);
            }

            logger.Info("Loaded: {0}", total);
        }

        public TranslationResult Translate(TranslationRequest request)
        {
            throw new NotSupportedException();
        }
    }
}
