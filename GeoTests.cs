using System;
using System.Linq;
using StackExchange.Redis;
using Xunit;

namespace TestRedis.Tests
{
    public class GeoTests
    {
        readonly IDatabase _redisDb;
        public GeoTests()
        {

            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redisDb = redis.GetDatabase();
        }

        [Fact]
        public void SimpleGeoTest()
        {
            const string prefix = "Simple";
            InitUkraine(prefix);

            var pos = _redisDb.GeoPosition($"{prefix}:Ukraine", "Mykolaiv");
            Assert.Equal(0, Math.Round(pos.Value.Longitude - 32.00, 4));
            Assert.Equal(0, Math.Round(pos.Value.Latitude- 46.58, 4));

            CleanUp(prefix);
        }

        [Fact]
        public void GeoDistanceTest()
        {
            const string prefix = "Distance";
            InitUkraine(prefix);

            var distance = _redisDb.GeoDistance($"{prefix}:Ukraine", "Mykolaiv", "Kyiv", GeoUnit.Kilometers);
            Assert.NotNull(distance);
            Assert.InRange(distance.Value, 400.0, 500.0);

            CleanUp(prefix);
        }
        [Fact]
        public void GeoRadiusTest()
        {
            const string prefix = "Radius";
            InitUkraine(prefix);

            var nearest = _redisDb.GeoRadius($"{prefix}:Ukraine", "Mykolaiv", 500, GeoUnit.Kilometers);
            Assert.NotNull(nearest);
            Assert.Contains(nearest, item => item.Member == "Kyiv");
            Assert.True(nearest.All(item => item.Member != "Lviv"));

            CleanUp(prefix);
        }
        private void InitUkraine(string prefix)
        {
            _redisDb.GeoAdd($"{prefix}:Ukraine", 32.00, 46.58, "Mykolaiv");
            _redisDb.GeoAdd($"{prefix}:Ukraine", 30.3125, 50.27, "Kyiv");
            _redisDb.GeoAdd($"{prefix}:Ukraine", 50.00, 36.1345, "Kharkiv");
            _redisDb.GeoAdd($"{prefix}:Ukraine", 24.00, 49.5, "Lviv");
        }

        private void CleanUp(string prefix)
        {

            _redisDb.KeyDelete($"{prefix}:Ukraine");
        }
    }
}