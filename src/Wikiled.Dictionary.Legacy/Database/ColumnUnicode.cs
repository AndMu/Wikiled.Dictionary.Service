namespace Wikiled.Dictionary.Legacy.Database
{
    /// <summary>
    /// Use this column to convert from unicode
    /// </summary>
    class ColumnUnicode : Column
    {
        private readonly int size = 150;

        public int Size
        {
            get { return size; }
        }

        public ColumnUnicode(string name, int size)
            :base(name)
        {
            this.size = size;
        }

        public override string GenerateCondition(string comparer)
        {
            return string.Format("CONVERT(varchar({1}), lower({0})) COLLATE SQL_Latin1_General_Cp437_BIN {3} CONVERT(varchar({1}), lower({2})) COLLATE SQL_Latin1_General_Cp437_BIN", ColumnName, size, Variable, comparer);
        }
    }
}
