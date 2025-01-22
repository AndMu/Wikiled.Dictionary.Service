using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;
using Wikiled.Dictionary.Legacy.Extensions;
using Wikiled.Dictionary.Legacy.Helpers;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public abstract class TranslationHandlerBase : ITranslationHandler
    {
        protected readonly IDataFactory dataFactory;

        protected readonly Logger log;

        private ConcurrentDictionary<int, int[]> leftTable = new ConcurrentDictionary<int, int[]>();

        private ConcurrentDictionary<int, List<int>> leftTmp;

        private string name;

        private ConcurrentDictionary<int, int[]> rightTable = new ConcurrentDictionary<int, int[]>();

        private ConcurrentDictionary<int, List<int>> rightTmp;

        protected TranslationHandlerBase(ILanguageDictionaryEx left, ILanguageDictionaryEx right, IDataFactory data)
        {
            Guard.NotNull(() => data, data);
            Guard.NotNull(() => left, left);
            Guard.NotNull(() => right, right);
            dataFactory = data;
            LeftLanguage = left;
            RightLanguage = right;
            if (LeftLanguage.Language == RightLanguage.Language)
            {
                throw new ArgumentException("Arguments can't match", nameof(right));
            }

            log = LogManager.GetLogger($"{LeftLanguage.Language} - {RightLanguage.Language}");
        }

        public int Count { get; private set; }

        public ILanguageDictionaryEx LeftLanguage { get; }

        public string Name => name ?? (name = LanguageExtension.TableName(LeftLanguage.Language, RightLanguage.Language));

        public ILanguageDictionaryEx RightLanguage { get; }

        public SimpleTranslationResult GetTranslations(Word word)
        {
            Guard.IsValid(() => word, word, item => word.Language != WorldLanguage.Auto, "Word");
            log.Debug("GetTranslations {0}", word);
            Word source = word.Language == LeftLanguage.Language
                              ? LeftLanguage.FindWord(word)
                              : RightLanguage.FindWord(word);

            if (source == null)
            {
                return new SimpleTranslationResult(word);
            }

            SimpleTranslationResult result = new SimpleTranslationResult(source);
            var anotherLanguage = word.Language == LeftLanguage.Language
                                      ? RightLanguage
                                      : LeftLanguage;

            ConcurrentDictionary<int, int[]> table = word.Language == LeftLanguage.Language
                                                         ? leftTable
                                                         : rightTable;
            int[] translations;

            if (!table.TryGetValue(source.Id.Value, out translations) ||
                translations == null ||
                translations.Length == 0)
            {
                return result;
            }

            List<TranslationPair> words = new List<TranslationPair>();

            foreach (var translation in translations)
            {
                int index = IndexUtility.GetIndex(translation);
                int rating = IndexUtility.GetRating(translation);
                var wordItem = anotherLanguage.FindWordById(index);
                TranslationPair wordFinal = new TranslationPair(wordItem);
                if (rating != 0)
                {
                    wordFinal.Coeficients.CustomerRating = (short)rating;
                }

                words.Add(wordFinal);
            }

            result.Translations = words.ToArray();
            return result;
        }

        public void Init()
        {
            leftTmp = new ConcurrentDictionary<int, List<int>>();
            rightTmp = new ConcurrentDictionary<int, List<int>>();

            ReceiveData();
            var concurrency = 4 * Environment.ProcessorCount;
            leftTable = new ConcurrentDictionary<int, int[]>(concurrency, leftTmp.Count);
            rightTable = new ConcurrentDictionary<int, int[]>(concurrency, rightTmp.Count);
            foreach (var item in leftTmp)
            {
                leftTable[item.Key] = item.Value.Distinct().ToArray();
            }

            foreach (var item in rightTmp)
            {
                rightTable[item.Key] = item.Value.Distinct().ToArray();
            }

            leftTmp = null;
            rightTmp = null;
        }

        public void UpdateRating(RatingPacket translationPacket)
        {
            Guard.NotNull(() => translationPacket, translationPacket);
            log.Debug("Update Rating: {0}", translationPacket);
            var leftLanguage = translationPacket.From.Language;
            var rightLanguage = translationPacket.To.Language;

            Word leftWord = leftLanguage == LeftLanguage.Language
                                ? LeftLanguage.FindWord(translationPacket.From)
                                : RightLanguage.FindWord(translationPacket.From);

            if (leftWord == null)
            {
                log.Warn("Failed resolving - {0}", translationPacket.From.Text);
                return;
            }

            Word rightWord = rightLanguage == LeftLanguage.Language
                                 ? LeftLanguage.FindWord(translationPacket.To)
                                 : RightLanguage.FindWord(translationPacket.To);

            if (rightWord == null)
            {
                log.Warn("Failed resolving - {0}", translationPacket.To.Text);
                return;
            }

            ConcurrentDictionary<int, int[]> source = leftLanguage == LeftLanguage.Language
                                                          ? leftTable
                                                          : rightTable;
            ConcurrentDictionary<int, int[]> target = rightLanguage == LeftLanguage.Language
                                                          ? leftTable
                                                          : rightTable;
            int[] data;
            if (source.TryGetValue(leftWord.Id.Value, out data))
            {
                Update(data, rightWord, translationPacket.Rating);
                source[leftWord.Id.Value] = data;
            }

            if (target.TryGetValue(rightWord.Id.Value, out data))
            {
                Update(data, leftWord, translationPacket.Rating);
                target[rightWord.Id.Value] = data;
            }

            UpdateRatingDatabase(leftLanguage, rightLanguage, leftWord, rightWord, translationPacket.Rating);
        }

        protected abstract string GetSql();

        protected abstract void UpdateRatingDatabase(
            WorldLanguage leftLanguage,
            WorldLanguage rightLanguage,
            Word leftWord,
            Word rightWord,
            int rating);

        private void AddTranslation(WordLink left, WordLink right, TranslationQueryResponse response)
        {
            if (left == null ||
                left.Value.Id == null ||
                right == null ||
                right.Value.Id == null)
            {
                return;
            }

            int leftId = left.Value.Id.Value;
            int rightId = right.Value.Id.Value;
            leftTmp.GetItemCreate(leftId).Add(IndexUtility.Merge(rightId, response.CustomerRating));
            rightTmp.GetItemCreate(rightId).Add(IndexUtility.Merge(leftId, response.CustomerRating));

            left[RightLanguage.Language] = true;
            right[LeftLanguage.Language] = true;
            Count++;
        }

        private void ReceiveData()
        {
            string sql = GetSql();
            log.Info("Loading Translation {0}...", Name);
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var translations = repository.ExecuteQuery<TranslationQueryResponse>(sql);
                foreach (var translation in translations)
                {
                    AddTranslation(LeftLanguage[LeftLanguage.FindWordByText(translation.LeftWord)], RightLanguage[RightLanguage.FindWordByText(translation.RightWord)], translation);
                }
            }
        }

        private void Update(int[] data, Word target, int newRating)
        {
            for (int i = 0; i < data.Length; i++)
            {
                int index = IndexUtility.GetIndex(data[i]);
                if (index != target.Id)
                {
                    continue;
                }

                data[i] = IndexUtility.Merge(index, newRating);
                break;
            }
        }
    }
}
