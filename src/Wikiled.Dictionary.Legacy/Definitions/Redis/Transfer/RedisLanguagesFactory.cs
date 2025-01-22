using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Translation;

namespace Wikiled.Dictionary.Legacy.Definitions.Redis.Transfer
{
    public class RedisLanguagesFactory : LanguagesFactoryBase
    {
        private readonly IWordRepository repository;

        private readonly IDataFactory dbFactory;

        public RedisLanguagesFactory(IWordRepository repository, IDataFactory dbFactory)
            :base(dbFactory)
        {
            Guard.NotNull(() => dbFactory, dbFactory);
            Guard.NotNull(() => repository, repository);
            this.repository = repository;
            this.dbFactory = dbFactory;
        }

        protected override ILanguageDictionaryEx Construct(Languages language)
        {
            return new RedisLanguageDictionary(language, repository, new DictionaryQuery(dbFactory), new TranslationHandlerFactory(dbFactory));
        }
    }
}
