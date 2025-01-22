using System;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Database
{
    public class JoinTable
    {
        public JoinTable(ILanguageDictionary english, ILanguageDictionary other)
        {
            Guard.NotNull(() => english, english);
            Guard.NotNull(() => other, other);
            if (english.Language != WorldLanguage.English)
            {
                throw new ArgumentOutOfRangeException("english");
            }

            Other = other;
            English = english;
        }


        internal ILanguageDictionary Other { get; }

        internal ILanguageDictionary English { get; }
    }
}
