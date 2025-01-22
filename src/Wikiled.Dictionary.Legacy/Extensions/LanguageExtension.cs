using System;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Extensions
{
    public static class LanguageExtension
    {
        public static WorldLanguage ConvertToWorldLanguage(this Languages language)
        {
            Guard.NotNull(() => language, language);
            WorldLanguage wordLanguage = language.Language.Convert();
            if (wordLanguage == WorldLanguage.Unknown)
            {
                throw new ArgumentOutOfRangeException("language", language.Language);
            }

            return wordLanguage;
        }

        public static bool IsEnglish(this Languages language)
        {
            return ConvertToWorldLanguage(language) == WorldLanguage.English;
        }

        public static Column ColumnOnTable(this ILanguageDictionary language)
        {
            Guard.NotNull(() => language, language);
            return new Column($"{language.Description.Language}.word");
        }

        public static Column ColumnOnTable2(this ILanguageDictionary language)
        {
            Guard.NotNull(() => language, language);
            return new Column(language.Description.Language);
        }

        public static Column IdColumn(this ILanguageDictionary language)
        {
            Guard.NotNull(() => language, language);
            return new Column($"{language.Description.Language}.Id");
        }

        public static Column ColumnOnView(this ILanguageDictionary language)
        {
            Guard.NotNull(() => language, language);
            return new Column(language.Description.Language);
        }

        public static string TableName(WorldLanguage from, WorldLanguage to)
        {
            return TableName(from.ToString(), to.ToString());
        }

        public static string TableName(string from, string to)
        {
            int compare = String.Compare(@from, to, StringComparison.OrdinalIgnoreCase);
            if (compare > 0)
            {
                return String.Format("{0}{1}", @from, to);
            }

            return compare == 0 ? @from : String.Format("{0}{1}", to, @from);
        }
    }
}