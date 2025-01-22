using System;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Service
{
    public class LocalTranslationManager : ITranslationService
    {
        public WorldLanguage[] AllLanguages { get; }

        public bool Exist(WorldLanguage language)
        {
            throw new NotImplementedException();
        }

        public TranslationResult Translate(TranslationRequest request)
        {
            throw new NotImplementedException();
        }

        public TranslationData[] GetTranslationInfo(WorldLanguage language)
        {
            throw new NotImplementedException();
        }

        public Word[] FindSimilar(Word word, WorldLanguage withTranslation, int total)
        {
            throw new NotImplementedException();
        }

        public Word ResolveWord(WorldLanguage language, int id)
        {
            throw new NotImplementedException();
        }

        public bool IsLatin(WorldLanguage language)
        {
            throw new NotImplementedException();
        }
    }
}
