using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Core.Utility.Cache;
using Wikiled.Postal.Logic;
using Wikiled.Redis.Config;
using Wikiled.Redis.Logic.Pool;

namespace Wikiled.IntegrationTests.Postal
{
    [TestFixture]
    public class PopulationManagerTests
    {
        private PostalManager instance;

        private RedisLinksPool redis;

        [SetUp]
        public void Setup()
        {
            redis = new RedisLinksPool(new[] { new RedisConfiguration("192.168.0.147", 6370) { ServiceName = "WikiPost" } });
            redis.Open();
            instance = CreateManager();
        }

        [TearDown]
        public void Cleanup()
        {
            redis.Close();
        }

        [Test]
        public async Task FindLocation()
        {
            var postals = await instance.FindLocation("E14 7FX").ToArray();
            Assert.AreEqual(1, postals.Length);
        }

        [TestCase("E14 7", 6130)]
        [TestCase("E14 7fx", 53)]
        public async Task FindSimilarAddress(string code, int total)
        {
            var postals = await instance.FindSimilarAddress(code, int.MaxValue).ToArray();
            Assert.AreEqual(total, postals.Length);
        }

        [TestCase("E14 7", 0)]
        [TestCase("E14 7fx", 53)]
        [TestCase("E147fx", 53)]
        public async Task FindAddress(string code, int total)
        {
            var postals = await instance.FindAddress(code).ToArray();
            Assert.AreEqual(total, postals.Length);
        }

        private PostalManager CreateManager()
        {
            return new PostalManager(redis, new NullLocalCache());
        }
    }
}
