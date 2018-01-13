using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using MongoDB.Driver;

namespace MongoCore.DbDriver
{
    internal class MongoDbContext
    {
        private readonly MongoClient _client;
        private readonly List<string> _dbs;
        private readonly Dictionary<string, List<string>> _dbCollections;

        public bool HasDatabase(string dbname)
        {
            return !string.IsNullOrEmpty(_dbs.Find(db => db.Equals(dbname)));
        }

        public bool HasCollection(string dbname, string collectionname)
        {
            if (!HasDatabase(dbname)) return false;
            var collections = _dbCollections[dbname];
            return ( !string.IsNullOrEmpty(collections.Find(c => c.Equals(collectionname))) );
        }

        public IMongoDatabase Database(string name)
        {
            var result = _dbs.Find(db => db.Equals(name));
            if (string.IsNullOrEmpty(result)) {
                _dbs.Add(name);
                _dbCollections[name] = new List<string>();
            }
            return _client.GetDatabase(name);
        }

        public IMongoCollection<T> Collection<T>(string dbName, string collectionName)
        {
            if (_dbCollections.TryGetValue(dbName, out List<string> collections)) {
                var result = collections.Find(c => c.Equals(collectionName));
                if (string.IsNullOrEmpty(result)) {
                    collections.Add(collectionName);
                }
                var db = Database(dbName);
                return db.GetCollection<T>(collectionName);
            }
            return null;
        }

        public MongoDbContext(string connection)
        {
	    Console.WriteLine($">>>>>>>> MONGODB Connection: {connection}");
            _client = new MongoClient(connection);
            _dbs = new List<string>();
            _dbCollections = new Dictionary<string, List<string>>();
        }
    }
}
