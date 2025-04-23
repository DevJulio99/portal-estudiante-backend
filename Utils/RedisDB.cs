using StackExchange.Redis;

namespace MyPortalStudent.Utils
{
    public class RedisDB: IRedisDB
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        public RedisDB(IConnectionMultiplexer redis) {
            _redis = redis;
            _database = redis.GetDatabase();
        }
        public void SetString(string key, string value, TimeSpan? expiry = null) 
        {
            _database.StringSet(key, value, expiry);
        }
        public string GetString(string key)
        {
            return _database.StringGet(key);
        }
        public bool DeleteKey(string key)
        {   
            return _database.KeyDelete(key);
        }

        public bool KeyExists(string key)
        {
            return _database.KeyExists(key);
        }
    }
    public interface IRedisDB 
    {
        void SetString(string key, string value, TimeSpan? expiry = null);
        string GetString(string key);
        bool DeleteKey(string key);
        bool KeyExists(string key);
    }
}
