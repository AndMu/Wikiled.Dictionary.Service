namespace Wikiled.Dictionary.Legacy.Definitions
{
    public interface ILanguagesFactory
    {
        /// <summary>
        /// Get all dictionaries
        /// </summary>
        /// <returns></returns>
        ILanguageDictionaryEx[] CreateAllDictionaries();
    }
}