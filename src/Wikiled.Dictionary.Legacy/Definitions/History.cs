using System;
using System.Collections.Generic;
using Wikiled.Dictionary.Legacy.Data;

namespace Wikiled.Dictionary.Legacy.Definitions
{
    /// <summary>
    /// Object which keeps search history
    /// </summary>
    [Serializable]
    public class History
    {
        private TranslationRequest[] translations;

        private readonly Queue<string> queue = new Queue<string>();

        private readonly Dictionary<string, TranslationRequest> dictionary = new Dictionary<string, TranslationRequest>(StringComparer.OrdinalIgnoreCase);

        private const int size = 10;

        private void Remove(string name)
        {
            translations = null;
            dictionary.Remove(name);
        }

        public void Reset()
        {
            queue.Clear();
            dictionary.Clear();
        }

        public void Add(TranslationRequest info)
        {
            if (info == null)
            {
                return;
            }

            var tag = info.ToString();
            translations = null;
            if (dictionary.ContainsKey(tag))
            {
                dictionary[tag] = info;
                return;
            }

            queue.Enqueue(tag);
            dictionary.Add(tag, info);
            if (queue.Count > size)
            {
                Remove(queue.Dequeue());
            }
        }

        public TranslationRequest[] Translations
        {
            get
            {
                if (translations != null && 
                    translations.Length == dictionary.Count)
                {
                    return translations;
                }

                var list = new List<TranslationRequest>();
                Queue<string>.Enumerator enumer = queue.GetEnumerator();
                while (enumer.MoveNext())
                {
                    if (dictionary.ContainsKey(enumer.Current))
                    {
                        list.Add(dictionary[enumer.Current]);
                    }
                }

                translations = list.ToArray();
                return translations;
            }
        }
    }
}
