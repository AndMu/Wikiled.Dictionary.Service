using System.Collections.Concurrent;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Data
{
    public class WordLink
    {
        private readonly ConcurrentDictionary<WorldLanguage, WorldLanguage> languages =
           new ConcurrentDictionary<WorldLanguage, WorldLanguage>();

        public WordLink()
        {
        }

        public WordLink(Word word)
        {
            Value = word;
        }

        public Word Value
        {
            get; private set;
        }

        public WordLink Previous
        {
            get; set;
        }

        public WordLink Next
        {
            get; set;
        }

        public bool this[WorldLanguage language]
        {
            get
            {
                return languages.ContainsKey(language);
            }
            set
            {
                languages[language] = language;
            }
        }
    }
}
