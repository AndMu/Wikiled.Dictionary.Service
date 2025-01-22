using System;
using System.Collections.Generic;
using Wikiled.Dictionary.Legacy.Data;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public static class CoreLanguageExtension
    {
        private static readonly Dictionary<string, WorldLanguage> Table
            = new Dictionary<string, WorldLanguage>(StringComparer.OrdinalIgnoreCase);

        static CoreLanguageExtension()
        {
            foreach (WorldLanguage name in Enum.GetValues(typeof (WorldLanguage)))
            {
                Table[name.ToString()] = name;
            }
        }

        public static WorldLanguage Convert(this string language)
        {
            if (String.IsNullOrEmpty(language))
            {
                return WorldLanguage.Unknown;
            }

            WorldLanguage wordLanguage;
            return !Table.TryGetValue(language, out wordLanguage) ? WorldLanguage.Unknown : wordLanguage;
        }

        public static string GetLowered(this string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Trim().ToLower();
        }

        public static string GetTranslationLabel(this Word word, WorldLanguage targetLanguage)
        {
            if (word == null)
            {
                return string.Empty;
            }

            return string.Format("{0} ({1} - {2})",
                                 word.Text,
                                 word.Language,
                                 targetLanguage);
        }
    }
}
