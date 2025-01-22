using Wikiled.Dictionary.Legacy.Database;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public interface ILanguageDictionaryFactory
    {
        ILanguageDictionaryEx Create(Languages languageDescription);
    }
}