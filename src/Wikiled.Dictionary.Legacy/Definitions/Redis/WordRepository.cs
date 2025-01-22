using System.Collections.Generic;
using System.Threading.Tasks;
using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Redis.Keys;
using Wikiled.Redis.Logic;

namespace Wikiled.Dictionary.Legacy.Definitions.Redis
{
    public class WordRepository : IWordRepository
    {
        private readonly IRedisLink redis;

        public WordRepository(IRedisLink redis)
        {
            this.redis = redis;
        }

        public void Save(Word word)
        {
            redis.Client.AddRecord(new RepositoryKey(this, new ObjectKey(word.Language.ToString())), word.Text.ToLower());
        }

        public void Save(SimpleTranslationResult translation)
        {
            List<Task> tasks = new List<Task>();
            foreach (var translationValue in translation.Translations)
            {
                var keyOne = new RepositoryKey(this, new ObjectKey(translationValue.Value.Language.ToString(), translation.Source.Language.ToString(), translationValue.Value.Text.ToLower()));
                var keyTwo = new RepositoryKey(this, new ObjectKey(translation.Source.Language.ToString(), translationValue.Value.Language.ToString(), translation.Source.Text.ToLower()));
                tasks.Add(redis.Client.AddRecord(keyOne, translation.Source.Text.ToLower()));
                tasks.Add(redis.Client.AddRecord(keyTwo, translationValue.Value.Text.ToLower()));
            }

            Task.WhenAll(tasks).Wait();
        }

        public string Name => "Dic";
    }
}
