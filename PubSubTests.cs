using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace TestRedis.Tests
{
    public class PubSubTests
    {
        private readonly ISubscriber _subscriber;
        private string _messageArrived = null;
        private const string ChannelName = "testChannel";

        public PubSubTests()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _subscriber = redis.GetSubscriber();
            _subscriber.Subscribe(ChannelName, OnEvent);
        }

        private void OnEvent(RedisChannel channel, RedisValue message)
        {
            _messageArrived = message;
        }

        [Fact]
        public async Task PubSub_Success()
        {
            string message = "hello world";
            await _subscriber.PublishAsync(ChannelName, message);
            await Task.Delay(1000);
            Assert.Equal(message, _messageArrived);
        }
    }
}