using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public interface ITranslationHandlerFactory
    {
        ITranslationHandler Create(ILanguageDictionaryEx left, ILanguageDictionaryEx right);
    }
}