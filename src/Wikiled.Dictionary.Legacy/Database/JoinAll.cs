using System.Collections.Generic;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Database
{
    public class JoinAllManager
    {
        private readonly List<JoinTable> usedJoins = new List<JoinTable>();
        private readonly Queue<JoinTable> sequency = new Queue<JoinTable>();        
        private readonly Dictionary<WorldLanguage, List<JoinTable>> joinTable;
        private readonly WorldLanguage fromLanguage;
        private readonly WorldLanguage toLanguage;
        
        public JoinAllManager(Dictionary<WorldLanguage, List<JoinTable>> joinTable, WorldLanguage from, WorldLanguage to)
        {
            this.joinTable = joinTable;
            fromLanguage = from;
            toLanguage = to;
            
        }

        public void Generate()
        {
            Generate(fromLanguage);
        }

        private void Generate(WorldLanguage language)
        {
            foreach (JoinTable join in joinTable[language])
            {
                if (usedJoins.Contains(join))
                {
                    continue;
                }

                usedJoins.Add(join);
                WorldLanguage otherLanguage =
                    join.Other.Language != language ? join.Other.Language : join.English.Language;

                if (otherLanguage != toLanguage)
                {
                    Generate(otherLanguage);                    
                }

                if (otherLanguage == toLanguage ||
                    sequency.Count > 0)
                {
                    sequency.Enqueue(join);
                    return;
                }
            }
        }

        public JoinTable[] CreateSequency
        {
            get
            {
                return sequency.ToArray();
            }
        }
    }
}
