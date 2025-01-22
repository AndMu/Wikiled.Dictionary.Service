using Moq;
using NUnit.Framework;
using System;
using Wikiled.Dictionary.Logic;
using Wikiled.Redis.Logic;
using Wikiled.Redis.Logic.Pool;

namespace Wikiled.Dictionary.Tests.Logic
{
    [TestFixture]
    public class RedisLanguageDictionaryTests
    {
        private Mock<IRedisLink> mockRedisLink;

        private Mock<IRedisLinksPool> mockPool;

        private RedisLanguageDictionary instance;

        [SetUp]
        public void Setup()
        {
            mockRedisLink = new Mock<IRedisLink>();
            mockPool = new Mock<IRedisLinksPool>();
            mockPool.Setup(item => item.GetKey("D")).Returns(mockRedisLink.Object);
            instance = CreateRedisLanguageDictionary();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new RedisLanguageDictionary(null));
            
        }

        private RedisLanguageDictionary CreateRedisLanguageDictionary()
        {
            return new RedisLanguageDictionary(mockPool.Object);
        }
    }
}
