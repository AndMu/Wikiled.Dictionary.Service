using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Translation;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public class LanguageDictionaryFactory : ILanguageDictionaryFactory
    {
        private readonly ITranslationHandlerFactory translationHandler;

        private readonly IDictionaryQuery dictionaryQuery;

        public LanguageDictionaryFactory(IDictionaryQuery dictionaryQuery, ITranslationHandlerFactory translationHandler)
        {
            Guard.NotNull(() => dictionaryQuery, dictionaryQuery);
            Guard.NotNull(() => translationHandler, translationHandler);
            this.dictionaryQuery = dictionaryQuery;
            this.translationHandler = translationHandler;
        }

        public ILanguageDictionaryEx Create(Languages languageDescription)
        {
            return new LanguageDictionary(languageDescription, dictionaryQuery, translationHandler);
        }
    }
}
