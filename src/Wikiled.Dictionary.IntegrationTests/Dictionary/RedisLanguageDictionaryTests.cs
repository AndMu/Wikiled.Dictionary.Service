using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Dictionary.Data;
using Wikiled.Dictionary.Logic;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic.Pool;

namespace Wikiled.IntegrationTests.Dictionary
{
    [TestFixture]
    public class RedisLanguageDictionaryTests
    {
        private RedisLanguageDictionary instance;

        private RedisLinksPool pool;

        [SetUp]
        public void Setup()
        {
            pool = new RedisLinksPool(new[] { new RedisConfiguration("192.168.0.147", 6372) { ServiceName = "D" } });
            pool.Open();
            instance = new RedisLanguageDictionary(pool);
        }

        [TearDown]
        public void Cleanup()
        {
            pool.Dispose();
        }

        [TestCase("motina", Language.Lithuanian, Language.English, 3)]
        [TestCase("sd", Language.Lithuanian, Language.English, 0)]
        [TestCase("mother", Language.English, Language.Lithuanian, 16)]
        [TestCase("mother", Language.English, Language.French, 5)]
        public async Task Translate(string word, Language fromLanguage, Language toLanguage, int totalResults)
        {
            var request = new TranslationRequest();
            request.Word = word;
            request.From = fromLanguage;
            request.To = toLanguage;
            var result = await instance.Translate(request).ConfigureAwait(false);
            Assert.AreEqual(totalResults, result.Translations.Length);
        }
    }
}
