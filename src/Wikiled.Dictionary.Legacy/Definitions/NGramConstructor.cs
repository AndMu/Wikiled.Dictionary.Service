using System;
using System.Collections.Generic;
using System.Linq;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    public class NGramConstructor
    {
        private readonly Dictionary<string, List<string>> words = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> wordsList = new List<string>(); 

        public IEnumerable<Tuple<string, string[]>> AddWord(string value)
        {
            var word = string.Intern(value);
            for (int i = 0; i < word.Length; i++)
            {
                var ngram = string.Intern(word.Substring(0, i + 1));
                if (words.Count == i)
                {
                    words.Add(ngram, new List<string> {word});
                    wordsList.Add(ngram);
                    continue;
                }

                if (wordsList[i] == ngram)
                {
                    words[ngram].Add(word);
                    continue;
                }

                int total = words.Count;
                for (int j = i; j < total; j++)
                {
                    var currentTag = wordsList[j];
                    List<string> list = words[currentTag];
                    yield return new Tuple<string, string[]>(currentTag, list.ToArray());
                    words.Remove(currentTag);
                }

                wordsList.RemoveRange(i, wordsList.Count - i);
                words.Add(ngram, new List<string> { word });
                wordsList.Add(ngram);
            }
        }

        public IEnumerable<Tuple<string, string[]>> GetRemaining()
        {
            return words.Select(word => new Tuple<string, string[]>(word.Key, word.Value.ToArray()));
        }
    }
}
