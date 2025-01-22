using System.Collections.Generic;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public interface ILanguageDictionaryEx : ILanguageDictionary
    {
        void InitTranslations(IEnumerable<ILanguageDictionaryEx> otherDictionaries);
    }
}
