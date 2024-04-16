using MongoDB.Bson;
using MongoDB.Driver;

namespace DailyActivities.Data
{
    public class MongoDBGenericDAO 
    {
        MongoClient? _client = null;
        string? _connectionString;
        string? _databaseName;

        public MongoDBGenericDAO(string? connectString, string? databaseName)
        {
            _connectionString = connectString;
            _databaseName = databaseName;
        }

        public async Task<IList<T>> Find<T>(string collection, BsonDocument filter)
        {
            _client = new MongoClient(_connectionString);
            var mongoDatabase = _client.GetDatabase(_databaseName);
            var _collection = mongoDatabase.GetCollection<T>(collection);
            var ret = await _collection.FindAsync(_ => true).Result.ToListAsync();
            return ret;
        }

        public async Task<bool> HardDelete<T>(string collection, string id)
        {
            _client = new MongoClient(_connectionString);
            var mongoDatabase = _client.GetDatabase(_databaseName);
            var _collection = mongoDatabase.GetCollection<T>(collection);
            var filter = new BsonDocument();
            filter.Add("_id", new BsonObjectId(new ObjectId(id)));
            try
            {
                var res = await _collection.FindOneAndDeleteAsync(filter);
                return true;

            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> SoftDelete<T>(string collection, string id)
        {
            _client = new MongoClient(_connectionString);
            var mongoDatabase = _client.GetDatabase(_databaseName);
            var _collection = mongoDatabase.GetCollection<T>(collection);

            var filter = new BsonDocument();
            filter.Add("_id", new BsonObjectId(new ObjectId(id)));
            var obj = _collection.FindAsync(filter).Result.FirstOrDefault();
            if (obj == null)
                return false;
            var document = obj.ToBsonDocument();
            document.Set("deleted" , true);
            document.Set("updateAt", DateTime.UtcNow);
            var res = await _collection.FindOneAndUpdateAsync(filter, document);
            return true;
        }

        public async Task<dynamic> Save<T>(string collection, dynamic obj)
        {
            _client = new MongoClient(_connectionString);
            var mongoDatabase = _client.GetDatabase(_databaseName);
            var _collection = mongoDatabase.GetCollection<T>(collection);
            await _collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<dynamic> Update<T>(string collection, string id, T obj)
        {
            _client = new MongoClient(_connectionString);
            var mongoDatabase = _client.GetDatabase(_databaseName);
            var _collection = mongoDatabase.GetCollection<T>(collection);
            var filter = new BsonDocument();
            filter.Add("_id", new BsonObjectId(new ObjectId(id)));
            var document = obj.ToBsonDocument();
            document.Set("updateAt", DateTime.UtcNow);
            await _collection.FindOneAndUpdateAsync(filter, document);
            return document;
            
        }

        public async Task<IList<T>> FindDeleted<T>(string collection, BsonDocument filter)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<T>> FindNotDeleted<T>(string collection, BsonDocument filter)
        {
            throw new NotImplementedException();
        }
    }
}
