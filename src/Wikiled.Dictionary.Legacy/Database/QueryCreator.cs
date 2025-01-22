using System;
using System.Text;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Definitions;
using Wikiled.Dictionary.Legacy.Extensions;

namespace Wikiled.Dictionary.Legacy.Database
{
    public static class QueryCreator
    {
        public static string GenerateSelectQuery(WorldLanguage language)
        {
            return string.Format("SELECT Id, Word AS [Text], [GoogleRating] AS [GoogleCount] FROM {0} WHERE word = @Word", language);
        }

        public static string GenerateSelectWordQuery(WorldLanguage language)
        {
            return string.Format("SELECT Word AS [Text] FROM {0} WHERE id = @Id", language);
        }

        public static string GenerateViewSimilars2(ILanguageDictionary source, ILanguageDictionary target)
        {
            var builder = new StringBuilder();
            string tableName = LanguageExtension.TableName(source.Language, target.Language);
            builder.AppendFormat("SELECT distinct Top 20 Id, {1} AS [Text] FROM {0} \r\n", tableName, source.Language);
            builder.AppendFormat("JOIN {0} ON {0}.Word = {1}.{0} \r\n",
                source.Language,
                tableName);
            builder.AppendFormat("WHERE {1}.{0} > @Word ORDER BY {0}",
                source.Language,
                tableName);
            return builder.ToString();
        }

        public static string GenerateDelete(ILanguageDictionary source, ILanguageDictionary target)
        {
            StringBuilder builder = new StringBuilder();
            string tableName = LanguageExtension.TableName(source.Language, target.Language);
            builder.AppendFormat("DELETE FROM {0} \r\n", tableName);
            builder.AppendFormat("WHERE {0} = @{0} AND {1} = @{1}",
                source.ColumnOnTable2().ColumnName,
                target.ColumnOnTable2().ColumnName);
            return builder.ToString();
        }

        public static string GenerateSelect(ILanguageDictionary source, ILanguageDictionary target)
        {
            StringBuilder builder = new StringBuilder();
            string tableName = LanguageExtension.TableName(source.Language, target.Language);
            builder.AppendFormat("SELECT Id, Word AS [Text], CAST(CASE WHEN [Source] = 30 THEN 1 ELSE 0 END AS BIT) AS IsGoogleConfirmed, {0}.GoogleRating AS [GoogleCount], [CustomerRating], [GoogleCoof]  FROM {0} \r\n", tableName);
            builder.AppendFormat("JOIN {0} ON {0}.Word = {1}.{0} \r\n",
                target.Language,
                tableName);
            builder.AppendFormat("WHERE {1} ORDER BY {0}",
                source.Language,
                source.ColumnOnTable2().Filter);
            return builder.ToString();
        }

        public static string GenerateUpdateRating(
            ILanguageDictionary source,
            ILanguageDictionary target,
            TranslationPacket packet)
        {
            throw new NotImplementedException("Suspended");
        }

        public static string GenerateGoogleRating(WorldLanguage language)
        {
            return string.Format("UPDATE [{0}] SET GoogleRating = @DataValue WHERE word = @Word", language);
        }

        public static string GenerateGoogleCoof(WorldLanguage fromLanguage, WorldLanguage toLanguage)
        {
            return string.Format("UPDATE [{0}] SET GoogleCoof = @DataValue WHERE [{1}] = @FromWord AND [{2}] = @ToWord",
                                 LanguageExtension.TableName(fromLanguage, toLanguage),
                                 fromLanguage,
                                 toLanguage);
        }

        public static string GenerateGoogleRating(WorldLanguage fromLanguage, WorldLanguage toLanguage)
        {
            return string.Format("UPDATE [{0}] SET GoogleRating = @DataValue WHERE [{1}] = @FromWord AND [{2}] = @ToWord",
                                 LanguageExtension.TableName(fromLanguage, toLanguage),
                                 fromLanguage,
                                 toLanguage);
        }

        public static string ConfirmGoogleTranslation(WorldLanguage fromLanguage, WorldLanguage toLanguage)
        {
            return string.Format("UPDATE [{0}] SET [Source] = 30 WHERE [{1}] = @FromWord AND [{2}] = @ToWord and [Source] < 30",
                LanguageExtension.TableName(fromLanguage, toLanguage),
                fromLanguage,
                toLanguage);
        }

        public static string GenerateGetFullTranslation(WorldLanguage language)
        {
            return string.Format("SELECT Id, Word AS [Text] FROM {0} WHERE word = @Word", language);
        }

        public static string GenerateViewSimilars(string from, string to)
        {
            return string.Format("SELECT Top 20 * FROM TransView{0}{1} WHERE word > @Word ORDER BY word",
                from,
                to);
        }

        public static string GenerateQuearyExist(ILanguageDictionary dictionary)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} ", dictionary.Language, dictionary.ColumnOnTable().Filter);
        }
    }
}
