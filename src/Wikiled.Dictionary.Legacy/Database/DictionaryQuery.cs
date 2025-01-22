using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Definitions;
using Wikiled.Dictionary.Legacy.Extensions;
using Wikiled.Dictionary.Legacy.Helpers;

namespace Wikiled.Dictionary.Legacy.Database
{
    public class DictionaryQuery : IDictionaryQuery
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IDataFactory dataFactory;

        public DictionaryQuery(IDataFactory dataFactory)
        {
            Guard.NotNull(() => dataFactory, dataFactory);
            this.dataFactory = dataFactory;
        }

        public Word ResolveMainWord(ILanguageDictionary dictionary, string word)
        {
            Guard.NotNull(() => dictionary, dictionary);
            log.Debug("ResolveMainWord {0} {1} ", dictionary.Language, word);
            var param = dictionary.GetWordParameter(word);

            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var result = repository.ExecuteQuery<Word>(QueryCreator.GenerateSelectQuery(dictionary.Language), param).FirstOrDefault();
                if (result != null)
                {
                    result.Language = dictionary.Language;
                    return (Word)result.Clone();
                }

                return null;
            }
        }

        /// <summary>
        /// Put word to process queue
        /// </summary>
        public void PutToProcess(ILanguageDictionary dictionary, string word, WorldLanguage toLanguage)
        {
            var fromWord = new SqlParameter("@FromWord", word) { SqlDbType = SqlDbType.NVarChar };
            var from = new SqlParameter("@From", dictionary.Language) { SqlDbType = SqlDbType.NVarChar };
            SqlParameter to = new SqlParameter("@To", toLanguage) { SqlDbType = SqlDbType.NVarChar };

            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                repository.ExecuteProcedure("InsertProcess", fromWord, from, to);
            }
        }

        /// <summary>
        /// Translate to language with given joins
        /// </summary>
        /// <returns></returns>
        public List<Word> TranslateToLanguage(ILanguageDictionary dictionary, string word, ILanguageDictionary toLanguage)
        {
            var param = dictionary.GetWordParameter(word);
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var words = repository.ExecuteQuery<Word>(QueryCreator.GenerateSelect(dictionary, toLanguage), param).ToList();
                for (int i = 0; i < words.Count; i++)
                {
                    words[i] = (Word)words[i].Clone();
                    words[i].Language = toLanguage.Language;
                }

                return words;
            }
        }

        public Word InsertResolveMainWord(ILanguageDictionary dictionary, string word)
        {
            var mainWord = ResolveMainWord(dictionary, word);
            log.Debug("InsertResolveMainWord {0} {1} ", dictionary.Language, word);
            if (mainWord != null)
            {
                return mainWord;
            }

            var param = dictionary.GetWordParameter(word);
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                repository.ExecuteCommand($"INSERT INTO {dictionary.Language} (word) VALUES (@Word)", param);
            }

            mainWord = ResolveMainWord(dictionary, word);
            if (mainWord == null)
            {
                throw new WikiException($"Failed to insert: {word}");
            }

            return mainWord;
        }

        public void DeleteTranslation(ILanguageDictionary dictionary, string from, string to, ILanguageDictionary toLanguage)
        {
            var param1 = new SqlParameter($"@{dictionary.ColumnOnTable2().ColumnName}", from);
            var param2 = new SqlParameter($"@{toLanguage.ColumnOnTable2().ColumnName}", to);
            param1.SqlDbType = SqlDbType.NVarChar;
            param2.SqlDbType = SqlDbType.NVarChar;
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                repository.ExecuteCommand(QueryCreator.GenerateDelete(dictionary, toLanguage), param1, param2);
            }
        }

        public bool WordExist(ILanguageDictionary dictionary, string word)
        {
            var param = dictionary.GetWordParameter(word);
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                return repository.ExecuteQuery<bool>(QueryCreator.GenerateQuearyExist(dictionary), param).FirstOrDefault();
            }
        }

        public List<Word> FindSimilar(ILanguageDictionary dictionary, string word, string command)
        {
            var param = dictionary.GetWordParameter(word);
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var words = repository.ExecuteQuery<Word>(command, param).ToList();
                for (int i = 0; i < words.Count; i++)
                {
                    words[i] = (Word)words[i].Clone();
                    words[i].Language = dictionary.Language;
                }

                return words;
            }
        }

        public void ConfirmGoogleTranslation(TranslationRequest request, ILanguageDictionary dictionary, Word word)
        {
            using (var repository = dataFactory.CreateRawDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var from = new SqlParameter("@ToWord", word.Text) { SqlDbType = SqlDbType.NVarChar };
                var to = new SqlParameter("@FromWord", request.Source.Text) { SqlDbType = SqlDbType.NVarChar };
                repository.ExecuteCommand(QueryCreator.ConfirmGoogleTranslation(request.Source.Language, request.TargetLanguage), from, to);
            }
        }

        public void SetGoogleCount(TranslationRequest request, ILanguageDictionary dictionary, Word word, long total)
        {
            using (var repository = dataFactory.CreateRawDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var from = new SqlParameter("@ToWord", word.Text) { SqlDbType = SqlDbType.NVarChar };
                var to = new SqlParameter("@FromWord", request.Source.Text) { SqlDbType = SqlDbType.NVarChar };
                var totalParam = new SqlParameter("@DataValue", SqlDbType.BigInt)
                                     {
                                         Value = total
                                     };
                repository.ExecuteCommand(QueryCreator.GenerateGoogleRating(request.Source.Language, request.TargetLanguage), from, to, totalParam);
            }
        }

        public void SetGoogleCount(ILanguageDictionary dictionary, Word word, long total)
        {
            using (var repository = dataFactory.CreateRawDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var param = dictionary.GetWordParameter(word.Text);
                var totalParam = new SqlParameter("@DataValue", SqlDbType.BigInt)
                {
                    Value = total
                };

                repository.ExecuteCommand(QueryCreator.GenerateGoogleRating(word.Language), param, totalParam);
            }
        }

        public void SetGoogleCoef(TranslationRequest request, ILanguageDictionary dictionary, Word word, double total)
        {
            using (var repository = dataFactory.CreateRawDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var from = new SqlParameter("@ToWord", word.Text) { SqlDbType = SqlDbType.NVarChar };
                var to = new SqlParameter("@FromWord", request.Source.Text) { SqlDbType = SqlDbType.NVarChar };
                var totalParam = new SqlParameter("@DataValue", SqlDbType.Float)
                {
                    Value = total
                };

                repository.ExecuteCommand(QueryCreator.GenerateGoogleCoof(request.Source.Language, request.TargetLanguage), from, to, totalParam);
            }
        }

        public IEnumerable<WordLink> LoadAllWords(WorldLanguage language)
        {
            string sql = $"SELECT DISTINCT ID, Word AS [Text], [GoogleRating] AS [GoogleCount] FROM {language} ORDER BY [Text]";
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var allItems = repository
                    .ExecuteQuery<Word>(sql)
                    .Select(item =>
                {
                    item.Language = language;
                    return new WordLink(item);
                });

                foreach (var link in allItems)
                {
                    yield return link;
                }
            }
        }
    }
}
