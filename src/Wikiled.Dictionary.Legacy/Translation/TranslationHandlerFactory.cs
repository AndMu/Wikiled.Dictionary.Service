using System;
using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public class TranslationHandlerFactory : ITranslationHandlerFactory
    {
        private readonly Dictionary<string, ITranslationHandler> translations = new Dictionary<string, ITranslationHandler>(StringComparer.OrdinalIgnoreCase);

        private readonly IDataFactory dataFactory;

        public TranslationHandlerFactory(IDataFactory dataFactory)
        {
            Guard.NotNull(() => dataFactory, dataFactory);
            this.dataFactory = dataFactory;
        }

        public ITranslationHandler Create(ILanguageDictionaryEx left, ILanguageDictionaryEx right)
        {
            if (left.Language == WorldLanguage.English)
            {
                return CreateDictionary(new EnglishTranslationHandler(left, right, dataFactory));
            }

            return right.Language == WorldLanguage.English
                ? CreateDictionary(new EnglishTranslationHandler(right, left, dataFactory))
                : CreateDictionary(new RegularTranslationHandler(left, right, dataFactory));
        }

        private ITranslationHandler CreateDictionary(ITranslationHandler translation)
        {
            lock (translations)
            {
                if (translations.ContainsKey(translation.Name))
                {
                    return translations[translation.Name];
                }

                translations[translation.Name] = translation;
            }

            translation.Init();
            return translation;
        }
    }
}
