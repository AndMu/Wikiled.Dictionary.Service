using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy
{
    public class LanguagesFactory : LanguagesFactoryBase
    {
        private readonly ILanguageDictionaryFactory languageDictionaryFactory;

        public LanguagesFactory(IDataFactory dataFactory, ILanguageDictionaryFactory languageDictionaryFactory)
            : base(dataFactory)
        {
            Guard.NotNull(() => dataFactory, dataFactory);
            Guard.NotNull(() => languageDictionaryFactory, languageDictionaryFactory);
            this.languageDictionaryFactory = languageDictionaryFactory;
        }

        protected override ILanguageDictionaryEx Construct(Languages language)
        {
            return languageDictionaryFactory.Create(language);
        }
    }
}
