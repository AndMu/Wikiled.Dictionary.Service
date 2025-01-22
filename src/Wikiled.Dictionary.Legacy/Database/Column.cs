namespace Wikiled.Dictionary.Legacy.Database
{
    public class Column
    {
        public string Variable { get; set; }
        
        public string ColumnName { get; set;}

        public Column(string name)
        {
            ColumnName = name;
            Variable = "@Source";
        }

        public virtual string GenerateCondition(string comparer)
        {
            return string.Format("{0} {2} {1}", ColumnName, Variable, comparer);
        }
        public string Filter
        {
            get
            {
                return GenerateCondition("=");
            }
        }
    }
}
