using System.Collections.Generic;
using Wikiled.Dictionary.Legacy.Data;

namespace Wikiled.Dictionary.Legacy.Translation
{
    public class WordComparer : IComparer<Word>
    {
        public int Compare(Word x, Word y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            if (x.Language > y.Language)
            {
                return 1;
            }

            if (y.Language > x.Language)
            {
                return -1;
            }

            return System.String.Compare(x.Text, y.Text, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
