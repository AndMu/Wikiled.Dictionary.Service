using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Data;
using Wikiled.Redis.Keys;
using Wikiled.Redis.Logic;
using Wikiled.Redis.Logic.Pool;

namespace Wikiled.Dictionary.Logic
{
    public class RedisLanguageDictionary : ILanguageDictionary
    {
        private readonly IRedisLink redis;

        public RedisLanguageDictionary(IRedisLinksPool pool)
        {
            Guard.NotNull(() => pool, pool);
            redis = pool.GetKey("D");
            Guard.NotNull(() => redis, redis);
        }

        public string Name => "Dic";

        public async Task<TranslationResult> Translate(TranslationRequest request)
        {
            Guard.NotNull(() => request, request);
            var key = new RepositoryKey(this, new ObjectKey(request.From.ToString(), request.To.ToString(), request.Word.ToLower()));
            var results = redis.Client.GetRecords<string>(key).ToArray();
            var items = await results.ToTask().ConfigureAwait(false);
            return new TranslationResult(request, items);
        }
    }
}
