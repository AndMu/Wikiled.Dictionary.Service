using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy
{
    public abstract class LanguagesFactoryBase : ILanguagesFactory
    {
        private readonly IDataFactory dataFactory;

        protected LanguagesFactoryBase(IDataFactory dataFactory)
        {
            Guard.NotNull(() => dataFactory, dataFactory);
            this.dataFactory = dataFactory;
        }

        /// <summary>
        /// Get all dictionaries
        /// </summary>
        /// <returns></returns>
        public ILanguageDictionaryEx[] CreateAllDictionaries()
        {
            var dictionariesList = new List<ILanguageDictionaryEx>();
            using (var repository = dataFactory.CreateDataContextConnection<WikiledModelContainer>("Wikiled.Entity"))
            {
                foreach (var language in repository.Table<Languages>())
                {
                    var dictionary = Construct(language);
                    dictionariesList.Add(dictionary);
                }

                return dictionariesList.ToArray();
            }
        }

        protected abstract ILanguageDictionaryEx Construct(Languages language);
    }
}