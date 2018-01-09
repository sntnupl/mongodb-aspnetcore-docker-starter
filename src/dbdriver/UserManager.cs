using System;
using System.Threading.Tasks;
using MongoCore.DbDriver.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCore.DbDriver
{
    public class UserManager : IUserManager
    {
        private readonly IMongoCollection<UserDocument> _collection;
        
        public UserManager(string connection)
        {
            var ctxt = new MongoDbContext(connection);
            var db = ctxt.Database("mongocore");
            _collection = ctxt.Collection<UserDocument>("mongocore", "usertasks");
        }
        
        // :::: API ::::
        public async Task<UserDocument> FindByEmailAsync(string email)
        {
            var filter = Builders<UserDocument>.Filter.Eq("Email", email);
            var result = await _collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<UserDocument> FindByUsernameAsync(string username)
        {
            var filter = Builders<UserDocument>.Filter.Eq("Username", username);
            var result = await _collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<UserDocument> FindByIdAsync(ObjectId id)
        {
            var filter = Builders<UserDocument>.Filter.Eq("id", id.ToString());
            var result = await _collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
    }
}