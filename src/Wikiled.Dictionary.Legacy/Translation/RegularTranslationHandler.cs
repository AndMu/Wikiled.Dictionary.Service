using System;
using System.Data.SqlClient;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public class RegularTranslationHandler : TranslationHandlerBase
    {
        public RegularTranslationHandler(ILanguageDictionaryEx left, ILanguageDictionaryEx right, IDataFactory dataFactory)
            : base(left, right, dataFactory)
        {
            if (RightLanguage.Language == WorldLanguage.English)
            {
                throw new ArgumentException("English can't be supplied as a parameter", "right");
            }

            if (LeftLanguage.Language == WorldLanguage.English)
            {
                throw new ArgumentException("English can't be supplied as a parameter", "left");
            }
        }

        protected override string GetSql()
        {
            return $"SELECT {LeftLanguage.Language} AS LeftWord, {RightLanguage.Language} AS RightWord, [CustomerRating], [GoogleRating], [Source], [GoogleCoof] FROM {Name}";
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
                var totalUpdates = repository.ExecuteCommand(
                    GenerateUpdateDatabase(leftLanguage, rightLanguage, rating),
                    new SqlParameter("@LeftWord", leftWord.Text),
                    new SqlParameter("@RightWord", rightWord.Text));

                if (totalUpdates != 1)
                {
                    log.Warn("Update rating count - {0}", totalUpdates);
                }
            }
        }

        private string GenerateUpdateDatabase(WorldLanguage leftLanguage, WorldLanguage rightLanguage, int rating)
        {
            return string.Format("UPDATE {1} [CustomerRating] = {0} WHERE {2} = @LeftWord AND {3} = @RightWord",
                                 rating,
                                 Name,
                                 leftLanguage,
                                 rightLanguage);
        }
    }
}
