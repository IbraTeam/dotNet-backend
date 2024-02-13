using StackExchange.Redis;

namespace dotNetBackend.models.DbFirst
{
    public class RedisContext
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("my-redis-container,abortConnect=false");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static IDatabase GetDatabase()
        {
            return Connection.GetDatabase();
        }
    }
}
