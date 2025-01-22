namespace Wikiled.Dictionary.Legacy.Helpers
{
    public static class IndexUtility
    {
        const int IndexMask = 0xFFFFFFF;

        public static int Merge(int index, int? rating)
        {
            if (index > IndexMask)
            {
                throw new WikiException("Index is too large");
            }

            if (rating.HasValue)
            {
                return (rating.Value << 28) + index;
            }

            return index;
        }

        public static int GetIndex(int index)
        {
            return index & IndexMask;
        }

        public static int GetRating(int index)
        {
            return index >> 28;
        }
    }
}
