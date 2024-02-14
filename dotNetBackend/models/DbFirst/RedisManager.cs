using StackExchange.Redis;

namespace dotNetBackend.models.DbFirst
{
    public class RedisManager
    {
        public static string? ConnectionString { set; get; }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect((ConnectionString ?? "localhost:6379"));
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
