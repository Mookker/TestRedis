using StackExchange.Redis;
using Xunit;

namespace TestRedis.Tests
{
    public class HashTests
    {
        readonly IDatabase _redisDb;

        public HashTests()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redisDb = redis.GetDatabase();
        }
        
        class Person
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        
        /// <summary>
        /// Simple hash operations
        /// </summary>
        [Fact]
        public void SimpleGetSetDeleteHash_Success()
        {
            var person = new Person {Id = "1", FirstName = "Ivan", LastName = "Bykov"};

            _redisDb.HashSet($"Person:{person.Id}", "firstName", person.FirstName);
            _redisDb.HashSet($"Person:{person.Id}", "lastName", person.LastName);

            var hashPerson = _redisDb.HashGetAll($"Person:{person.Id}");

            foreach (var hashEntry in hashPerson)
            {
                switch (hashEntry.Name)
                {
                    case "firstName":
                        Assert.Equal(person.FirstName, hashEntry.Value);
                        break;
                    case "lastName":
                        Assert.Equal(person.LastName, hashEntry.Value);
                        break;
                }
            }
            
            _redisDb.KeyDelete($"Person:{person.Id}");
        }

        
        /// <summary>
        /// Checks increment works
        /// </summary>
        [Fact]
        public void IncrementHash_Success()
        {
            _redisDb.HashSet("views:person:1", "count", 0);
            for(int i = 0 ; i < 10; ++i)
                _redisDb.HashIncrement("views:person:1", "count");
            var value = _redisDb.HashGet("views:person:1", "count");
            
            Assert.True(value.TryParse(out int intVal));
            Assert.Equal(10, intVal);
            _redisDb.KeyDelete("views:person:1");
        }
    }
}