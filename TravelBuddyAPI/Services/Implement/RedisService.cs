using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class RedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConfiguration config)
        {
            var mux = ConnectionMultiplexer.Connect(config["Redis:Connection"]);
            _db = mux.GetDatabase();
        }

        public void SetStatus(string userId, string status)
        {
            _db.StringSet($"cv_status:{userId}", status);
        }

        public string GetStatus(string userId)
        {
            return _db.StringGet($"cv_status:{userId}");
        }

        public void SetResult(string userId, string json)
        {
            _db.StringSet($"cv_result:{userId}", json);
        }

        public string? GetResult(string userId)
        {
            return _db.StringGet($"cv_result:{userId}");
        }
    }

}
