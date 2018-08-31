using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace TestRedis.Tests
{
    public class SimpleTests
    {
        readonly IDatabase _redisDb;

        public SimpleTests()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redisDb = redis.GetDatabase();
        }

        /// <summary>
        /// Simple get/set string check
        /// </summary>
        [Fact]
        public void SimpleGetSetDelete_Success()
        {
            _redisDb.StringSet("TestKey", "123");
            var testKey = _redisDb.StringGet("TestKey");
            Assert.False(testKey.IsNull);
            Assert.Equal("123", testKey);

            _redisDb.KeyDelete("TestKey");
            testKey = _redisDb.StringGet("TestKey");
            Assert.True(testKey.IsNull);
        }
        
        
        /// <summary>
        /// Simple expiration check
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleExpiration_Expires()
        {
            _redisDb.StringSet("TestExpirationKey", "123", TimeSpan.FromMilliseconds(1));
            await Task.Delay(1);
            
            var testKey = _redisDb.StringGet("TestExpirationKey");
            Assert.True(testKey.IsNull);
        }
    }
}