using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Service
{
    public class RedisService
    {
        private readonly string _server;
        private ConnectionMultiplexer _connect;

        public RedisService(IConfiguration configuration)
        {
            _server = configuration["Redis:Server"];
        }
        public void Connect()
        {
            _connect = ConnectionMultiplexer.Connect(_server);
        }

        public IDatabase GetDb(int db)
        {
            return _connect.GetDatabase(db);
        }
    }
}
