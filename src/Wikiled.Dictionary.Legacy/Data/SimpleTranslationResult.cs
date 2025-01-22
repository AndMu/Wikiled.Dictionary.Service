namespace Wikiled.Dictionary.Legacy.Data
{
    public class SimpleTranslationResult
    {
        public SimpleTranslationResult(Word source)
        {
            Source = source;
            Translations = new TranslationPair[] { };
        }

        public TranslationPair[] Translations { get; set; }

        public Word Source { get; private set; }
    }
}
