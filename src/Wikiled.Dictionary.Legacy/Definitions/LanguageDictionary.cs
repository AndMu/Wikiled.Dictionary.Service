using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Translation;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public class LanguageDictionary : ILanguageDictionaryEx
    {
        private readonly Dictionary<WorldLanguage, ITranslationHandler> handlers = new Dictionary<WorldLanguage, ITranslationHandler>();

        private readonly Dictionary<int, WordLink> idTable = new Dictionary<int, WordLink>();

        private readonly Logger log;

        private readonly NGramConstructor nGramConstructor = new NGramConstructor();

        private readonly Dictionary<string, string[]> similar = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);

        private Tuple<string, string[]> similarEnd;

        private Tuple<string, string[]> similarStart;

        private Dictionary<string, WordLink> table = new Dictionary<string, WordLink>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ITranslationHandlerFactory translationHandler;

        private readonly IDictionaryQuery dictionaryQuery;

        public LanguageDictionary(Languages languageDescription, IDictionaryQuery data, ITranslationHandlerFactory translationHandler)
        {
            Guard.NotNull(() => translationHandler, translationHandler);
            Guard.NotNull(() => data, data);
            Guard.NotNull(() => languageDescription, languageDescription);
            dictionaryQuery = data;
            Description = languageDescription;
            Language = languageDescription.Language.Convert();
            this.translationHandler = translationHandler;
            Description = languageDescription;
            log = LogManager.GetLogger(Language + "Dictionary");
        }

        public int TotalWords { get; private set; }

        public WordLink this[Word word]
        {
            get
            {
                if (word == null)
                {
                    return null;
                }

                if (!word.Id.HasValue)
                {
                    throw new ArgumentOutOfRangeException("word", "word should have id");
                }

                return idTable[word.Id.Value];
            }
        }

        public Languages Description { get; }

        public WorldLanguage Language { get; }

        public int LinkCounts => handlers.Count;

        public override string ToString()
        {
            return Language.ToString();
        }

        public Word FindWord(Word word)
        {
            return word.Id.HasValue ? FindWordById(word.Id.Value) : FindWordByText(word.Text);
        }

        public Word[] FindSimilar(string word, WorldLanguage withTranslation, int total)
        {
            log.Debug("FindNearWords {0} {1} {2}", word, withTranslation, total);
            List<Word> list = new List<Word>();

            if (string.IsNullOrEmpty(word))
            {
                return new Word[] { };
            }

            string[] items;
            if (!similar.TryGetValue(word, out items))
            {
                if (word.Length == 1)
                {
                    if (similarStart != null &&
                        similarStart.Item1[0] >= word[0])
                    {
                        items = similarStart.Item2;
                    }
                    else if (similarEnd != null &&
                        similarEnd.Item1[0] <= word[0])
                    {
                        items = similarEnd.Item2;
                    }
                    else
                    {
                        return new Word[] { };
                    }
                }
                else
                {
                    return FindSimilar(word.Substring(0, word.Length - 1), withTranslation, total);
                }
            }

            foreach (var item in items)
            {
                var wordItem = FindWordByText(item);
                if (wordItem == null)
                {
                    continue;
                }

                var foundWord = this[wordItem];
                if (withTranslation == WorldLanguage.Auto ||
                    foundWord[withTranslation])
                {
                    list.Add(foundWord.Value);
                }

                if (list.Count >= total)
                {
                    break;
                }
            }

            return list.ToArray();
        }

        public Word FindSimilarWord(string word, WorldLanguage? hasTranslation, int total = 4)
        {
            log.Debug("FindSimilarWord {0} {1}", word, hasTranslation);
            if (string.IsNullOrWhiteSpace(word))
            {
                return null;
            }

            var item = FindWordByText(word);
            if (item != null)
            {
                var foundWord = this[item];
                if (!hasTranslation.HasValue ||
                    foundWord[hasTranslation.Value])
                {
                    return item;
                }

                return null;
            }

            var foundItem = FindSimilar(word, hasTranslation ?? WorldLanguage.Auto, 1).FirstOrDefault();
            if (foundItem != null)
            {
                return foundItem;
            }

            return FindSimilarWord(word.Substring(0, word.Length - 1), hasTranslation, --total);
        }

        public Word FindWordByText(string word)
        {
            Guard.NotNull(() => word, word);
            log.Debug("FindWord {0}", word);
            WordLink simpleWord;
            if (!table.TryGetValue(word, out simpleWord))
            {
                table.TryGetValue(word.RemoveDiacritics(), out simpleWord);
            }

            return simpleWord?.Value;
        }

        public void InitTranslations(IEnumerable<ILanguageDictionaryEx> otherDictionaries)
        {
            log.Debug("InitTranslations");
            foreach (var languageDictionary in otherDictionaries)
            {
                if (languageDictionary.Language == Language)
                {
                    continue;
                }

                Register(translationHandler.Create(this, languageDictionary));
            }
        }

        public void Load()
        {
            log.Debug("Load");
            table.Clear();
            var allWords = dictionaryQuery.LoadAllWords(Language).ToArray();
            table = new Dictionary<string, WordLink>(allWords.Length, StringComparer.InvariantCultureIgnoreCase);
            WordLink lastLink = null;
            TotalWords = allWords.Length;
            foreach (var link in allWords)
            {
                idTable[link.Value.Id.Value] = link;
                AddWord(link.Value.Text, link);
                if (lastLink != null)
                {
                    lastLink.Next = link;
                    link.Previous = lastLink;
                }

                lastLink = link;
            }

            var remaining = nGramConstructor.GetRemaining().ToArray();
            foreach (var item in remaining)
            {
                similar[item.Item1] = item.Item2;
            }

            similarStart = remaining.FirstOrDefault();
            similarEnd = remaining.LastOrDefault();
            table = table.Compact();
        }

        public Word FindWordById(int id)
        {
            log.Debug("ResolveWord {0}", id);
            WordLink simpleWord;
            if (idTable.TryGetValue(id, out simpleWord))
            {
                return simpleWord.Value;
            }

            return null;
        }

        public TranslationResult Translate(TranslationRequest request)
        {
            Guard.NotNull(() => request, request);
            log.Debug(request.ToString());
            Guard.IsValid(() => request, request, item => request.TargetLanguage != WorldLanguage.Auto, "Invalid");
            Guard.IsValid(() => request, request, item => request.Source.Language != WorldLanguage.Auto, "Invalid");

            TranslationResult result = new TranslationResult(request);
            ITranslationHandler translationHandler;
            if (!handlers.TryGetValue(request.TargetLanguage, out translationHandler))
            {
                return result;
            }

            var translation = translationHandler.GetTranslations(request.Source);
            result.Translations = translation.Translations;

            var nearWords = FindNearWords(translation.Source, request.TargetLanguage, 20);
            result.Similar = nearWords.Select(item => item).ToArray();
            if (result.Similar.Length == 0)
            {
                var similarItems = FindSimilarWord(translation.Source.Text, request.TargetLanguage);
                if (similarItems != null)
                {
                    nearWords = FindNearWords(similarItems, request.TargetLanguage, 20);
                    result.Similar = nearWords.Select(item => item).ToArray();
                }
            }

            result.Translations = result.Translations
                .OrderByDescending(item => item.Coeficients.CustomerRating)
                .ThenBy(item => item.Value.Text)
                .ToArray();

            return result;
        }

        private void AddWord(string label, WordLink word)
        {
            if (string.IsNullOrEmpty(label))
            {
                return;
            }

            table[string.Intern(label)] = word;
            table[string.Intern(label.RemoveDiacritics())] = word;
            var similarities = nGramConstructor.AddWord(label);
            foreach (var similarity in similarities)
            {
                similar[similarity.Item1] = similarity.Item2;
            }
        }

        private Word[] FindNearWords(Word wordItem, WorldLanguage withTranslation, int total)
        {
            if (wordItem?.Id == null)
            {
                return new Word[] { };
            }

            WordLink link = this[wordItem];
            WordLink previous = link.Previous;
            WordLink next = link.Next;
            List<Word> nearWords = new List<Word>();
            for (int i = 0; i < total * 100; i++)
            {
                if (previous != null)
                {
                    if (previous[withTranslation])
                    {
                        nearWords.Add(previous.Value);
                    }

                    previous = previous.Previous;
                }

                if (nearWords.Count >= total)
                {
                    break;
                }

                if (next != null)
                {
                    if (next[withTranslation])
                    {
                        nearWords.Add(next.Value);
                    }

                    next = next.Next;
                }

                if (nearWords.Count >= total)
                {
                    break;
                }
            }

            return nearWords.ToArray();
        }

        private void Register(ITranslationHandler translation)
        {
            Guard.NotNull(() => translation, translation);
            WorldLanguage language = translation.LeftLanguage.Language == Language
                                         ? translation.RightLanguage.Language
                                         : translation.LeftLanguage.Language;
            handlers[language] = translation;
        }
    }
}