using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Data.Factory;
using Wikiled.Dictionary.Legacy.Database;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public class Manager : IManager
    {
        private readonly Dictionary<WorldLanguage, ILanguageDictionaryEx> dictTable =
            new Dictionary<WorldLanguage, ILanguageDictionaryEx>();

        private readonly List<ILanguageDictionaryEx> dictionaries = new List<ILanguageDictionaryEx>();

        private readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Lazy<TranslationSnapshot> snapshot;

        private readonly object syncRoot = new object();

        private readonly ILanguagesFactory languagesFactory;

        private readonly IDataFactory dataFactory;

        public Manager(ILanguagesFactory factory, IDataFactory dataFactory)
        {
            Guard.NotNull(() => factory, factory);
            Guard.NotNull(() => dataFactory, dataFactory);
            languagesFactory = factory;
            this.dataFactory = dataFactory;
            snapshot = new Lazy<TranslationSnapshot>(GetSnapshot);
            RegisterDictionaries();
        }

        public ILanguageDictionary this[WorldLanguage language]
        {
            get
            {
                lock (syncRoot)
                {
                    if (!Exist(language))
                    {
                        throw new ArgumentOutOfRangeException(
                            "language",
                            $"Unknown dictionary - {language}");
                    }

                    return dictTable[language];
                }
            }
        }

        public WorldLanguage[] AllLanguages
        {
            get; private set;
        }

        public TranslationSnapshot Snapshot => snapshot.Value;

        public bool Exist(WorldLanguage language)
        {
            log.Debug("Exist: {0}", language);
            return language == WorldLanguage.Auto || dictTable.ContainsKey(language);
        }

        public Word[] FindSimilar(Word word, WorldLanguage withTranslation, int total)
        {
            var wordItem = word.Language == WorldLanguage.Auto
                ? FindSimilarWord(word.Text, withTranslation)
                : word;
            return wordItem == null
                ? new Word[] { }
                : this[wordItem.Language].FindSimilar(wordItem.Text, withTranslation, total).ToArray();
        }

        public void Load(bool full)
        {
            lock (syncRoot)
            {
                log.Debug("Load: {0}", full);
                Parallel.ForEach(
                    dictionaries,
                    new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount / 2},
                    currentLanguage =>
                    {
                        currentLanguage.Load();
                        log.Info("Loading {0}", currentLanguage.Language);
                    }
                );

                if (full)
                {
                    Parallel.ForEach(
                        dictionaries,
                        new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount / 2},
                        currentLanguage =>
                        {
                            currentLanguage.InitTranslations(dictionaries);
                        });
                }
            }

            GC.Collect();
        }

        public TranslationResult Translate(TranslationRequest request)
        {
            Guard.NotNull(() => request, request);
            log.Debug("Translate: {0}", request);

            if (request.TargetLanguage == WorldLanguage.Auto ||
                request.TargetLanguage == WorldLanguage.Unknown ||
                request.TargetLanguage == request.Source.Language)
            {
                log.Warn("Can't use target auto language: ", request.Source.Text);
                return new TranslationResult(request);
            }

            if (request.Source.Language == WorldLanguage.Auto)
            {
                if (string.IsNullOrEmpty(request.Source.Text))
                {
                    return new TranslationResult(request);
                }

                var found = FindSimilarWord(request.Source.Text, null);
                if (found == null)
                {
                    log.Warn("Couldn't find similar for auto: ", request.Source.Text);
                    return new TranslationResult(request);
                }

                request = new TranslationRequest(found, request.TargetLanguage);
            }

            return this[request.Source.Language].Translate(request);
        }

        private Word FindSimilarWord(string word, WorldLanguage? hasTranslation)
        {
            foreach (var worldLanguage in dictTable)
            {
                if (hasTranslation.HasValue &&
                    hasTranslation.Value == ((ILanguageDictionary)worldLanguage.Value).Language)
                {
                    continue;
                }

                var item = worldLanguage.Value.FindWordByText(word);
                if (item == null)
                {
                    continue;
                }

                var link = worldLanguage.Value[item];

                if (hasTranslation != null && // ignore if word doesn't have translation
                    !link[hasTranslation.Value])
                {
                    continue;
                }

                return (Word)item.Clone();
            }

            foreach (var worldLanguage in dictTable)
            {
                if (hasTranslation.HasValue &&
                    hasTranslation.Value == ((ILanguageDictionary)worldLanguage.Value).Language)
                {
                    continue;
                }

                var item = worldLanguage.Value.FindSimilarWord(word, hasTranslation);
                if (item != null)
                {
                    return (Word)item.Clone();
                }
            }

            return null;
        }

        private TranslationSnapshot GetSnapshot()
        {
            try
            {
                using (var repository = dataFactory.CreateRawDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
                {
                    TranslationSnapshotFactory factory = new TranslationSnapshotFactory(this);
                    return factory.Construct(repository.Table<InfoTable>());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return new TranslationSnapshot();
        }

        private void RegisterDictionaries()
        {
            foreach (ILanguageDictionaryEx dictionary in languagesFactory.CreateAllDictionaries())
            {
                RegisterDictionary(dictionary);
            }

            AllLanguages = dictTable.Select(item => item.Key).ToArray();
        }

        private void RegisterDictionary(ILanguageDictionaryEx dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            var type = ((ILanguageDictionary)dictionary).Language;

            if (type != WorldLanguage.Auto &&
                Exist(type))
            {
                throw new ArgumentException("Dictionary already registered");
            }

            dictTable.Add(type, dictionary);
            dictionaries.Add(dictionary);
        }
    }
}