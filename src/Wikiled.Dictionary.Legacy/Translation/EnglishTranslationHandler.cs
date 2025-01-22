using System;
using System.Text;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public class EnglishTranslationHandler : TranslationHandlerBase
    {
        public EnglishTranslationHandler(ILanguageDictionaryEx english, ILanguageDictionaryEx right, IDataFactory dataFactory)
            : base(english, right, dataFactory)
        {
            if (LeftLanguage.Language != WorldLanguage.English)
            {
                throw new ArgumentException("English is not supplied as a parameter", nameof(english));
            }
        }

        protected override string GetSql()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("[{0}] \r\n", RightLanguage.Description.JoinTable);
            GenerateJoinWithOther(builder, LeftLanguage);
            GenerateJoinWithOther(builder, RightLanguage);
            string sql =
                $"SELECT [{LeftLanguage.Language}].Word AS LeftWord, [{RightLanguage.Language}].Word AS RightWord, CustomerRating FROM {builder}";
            return sql;
        }

        protected override void UpdateRatingDatabase(
           WorldLanguage leftLanguage,
           WorldLanguage rightLanguage,
           Word leftWord,
           Word rightWord,
           int rating)
        {
            log.Debug("UpdateRatingDatabase");
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                var englishWord = leftLanguage == WorldLanguage.English ? leftWord : rightWord;
                var anotherWord = leftLanguage == WorldLanguage.English ? rightWord : leftWord;
                var sql = string.Format("UPDATE {1} SET [CustomerRating] = {0} WHERE EnglishId = {2} AND {4} = {3}",
                                           rating,
                                           RightLanguage.Description.JoinTable,
                                           englishWord.Id,
                                           anotherWord.Id,
                                           RightLanguage.Description.JoinField);
                int totalUpdates = repository.ExecuteCommand(sql);
                if (totalUpdates != 1)
                {
                    log.Warn("Update rating count - {0}", totalUpdates);
                }
            }
        }

        private void GenerateJoinWithOther(StringBuilder builder, ILanguageDictionaryEx relationship)
        {
            builder.AppendFormat("JOIN [{0}] \r\n", relationship.Language);
            builder.AppendFormat(" \tON [{0}].[ID] = [{1}].[{2}] \r\n",
                        relationship.Language,
                        RightLanguage.Description.JoinTable,
                        relationship.Description.JoinField);
        }
    }
}
