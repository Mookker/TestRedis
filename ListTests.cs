using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using StackExchange.Redis;
using Xunit;

namespace TestRedis.Tests
{
    public class ListTests
    {
        readonly IDatabase _redisDb;

        public ListTests()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redisDb = redis.GetDatabase();
        }

        class City
        {
            protected bool Equals(City other)
            {
                return string.Equals(Id, other.Id) && string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((City) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                }
            }

            public string Id {get;set;}
            public string Name {get;set;}
        }

        [Fact]
        public void SimpleGetSetDeleteHash_Success()
        {
            
            var cityArray = new[] 
            {
                new City {Id = "1", Name = "Mykolaiv"},
                new City {Id = "2", Name = "Kyiv"},
                new City {Id = "3", Name = "Moskow"},
                new City {Id = "4", Name = "London"},
                new City {Id = "5", Name = "New York"},
            };

            var listKey = "CityList";
            foreach(var city in cityArray)
            {
                var jsonCity = JsonConvert.SerializeObject(city);
                _redisDb.ListRightPush("CityList", jsonCity);
            }

            var listLen = _redisDb.ListLength(listKey);            
            Assert.Equal(cityArray.Length, listLen);

            var retrievedList = new List<City>();
            for(int i = 0; i< listLen; ++i)
            {
                var cityJson = _redisDb.ListLeftPop(listKey);
                var city = JsonConvert.DeserializeObject<City>(cityJson);
                retrievedList.Add(city);
            }
            
            listLen = _redisDb.ListLength(listKey);            
            Assert.Equal(0, listLen);


            Assert.Equal(cityArray, retrievedList);
            _redisDb.KeyDelete(listKey);
        }
    }
}