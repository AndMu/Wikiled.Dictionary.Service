using System.Data;
using System.Data.SqlClient;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Extensions
{
    public static class DictionaryExtension
    {
        public static SqlParameter GetWordParameter(this ILanguageDictionary dictionary, string word)
        {
            return new SqlParameter("@Source", word) { SqlDbType = SqlDbType.NVarChar};
        }

        public static SqlParameter GetFromParameter(this WorldLanguage language)
        {
            return new SqlParameter("@From", SqlDbType.VarChar, 50)
                       {
                           Value = language
                       };
        }

        public static SqlParameter GetToParameter(this WorldLanguage language)
        {
            return new SqlParameter("@To", SqlDbType.VarChar, 50)
            {
                Value = language
            };
        }
    }
}
