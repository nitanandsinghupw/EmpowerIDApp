using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace App.Utility
{
    public class ConnectionHelper
    {
        private static Lazy<ConnectionMultiplexer>? lazyConnection;

        public static void Initialize(IConfiguration configuration)
        {
            string? con = configuration.GetConnectionString("RedisCacheConnection");

            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(con);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
