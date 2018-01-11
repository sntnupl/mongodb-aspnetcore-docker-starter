using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoCore.DbDriver.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCore.DbDriver
{
    public class AppRepository : IRepository
    {
        private readonly IMongoCollection<UserDocument> _collection;

        public AppRepository(string connection)
        {
            var ctxt = new MongoDbContext(connection);
            var db = ctxt.Database("mongocore");
            _collection = ctxt.Collection<UserDocument>("mongocore", "usertasks");
        }

        public async Task<bool> UserExistsAsync(ObjectId idUser)
        {
            var filter = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var user = await _collection.Find(filter).FirstOrDefaultAsync();
            return null != user;
        }

        // :::: API ::::
        public IEnumerable<UserDocument> GetAllUsers()
        {
            var users = _collection.Find(new BsonDocument()).ToList();
            return users;
        }
        
        public async Task<IEnumerable<UserDocument>> GetAllUsersAsync()
        {
            var users = await _collection.Find(new BsonDocument()).ToListAsync();
            return users;
        }

        public UserDocument GetUser(ObjectId idUser)
        {
            var filter = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var result = _collection.Find(filter).FirstOrDefault();
            return result;
        }

        public async Task<UserDocument> GetUserAsync(ObjectId idUser)
        {
            var filter = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var result = await _collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<TaskDocument>> GetAllTasksAsync(ObjectId idUser)
        {
            var filter = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var user = await _collection.Find(filter).FirstOrDefaultAsync();
            return user?.Tasks;
        }

        public async Task<TaskDocument> GetTaskAsync(ObjectId idUser, ObjectId idTask)
        {
            var filter = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var user = await _collection.Find(filter).FirstOrDefaultAsync();
            var result = user?.Tasks.First(task => task.Id.Equals(idTask));
            return result;
        }

        public async Task<TaskDocument> AddTaskAsync(ObjectId idUser, TaskDocument newTask)
        {
            var filterUser = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var user = await _collection.Find(filterUser).FirstOrDefaultAsync();
            if (null == user) return null;

            newTask.Id = ObjectId.GenerateNewId();
            newTask.Done = false;

            var updateCommand = Builders<UserDocument>.Update.Push("Tasks", newTask);
            await _collection.FindOneAndUpdateAsync(filterUser, updateCommand);
            return newTask;
        }

        public async Task<bool> DeleteTaskAsync(ObjectId idUser, ObjectId idTask)
        {
            var filterUser = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var user = await _collection.Find(filterUser).FirstOrDefaultAsync();
            if (null == user) return false;

            user.Tasks.RemoveAll(t => t.Id.Equals(idTask));
            var r = await _collection.ReplaceOneAsync(filterUser, user);
            return (r.ModifiedCount > 0);
        }

        public async Task<bool> UpdateTaskAsync(ObjectId idUser, ObjectId idTask, TaskDocument updatedTask)
        {
            var filterUser = Builders<UserDocument>.Filter.Eq("_id", idUser);
            var user = await _collection.Find(filterUser).FirstOrDefaultAsync();
            if (null == user) return false;
            
            user.Tasks.RemoveAll(t => t.Id.Equals(idTask));
            user.Tasks.Add(updatedTask);
            var r = await _collection.ReplaceOneAsync(filterUser, user);
            return (r.ModifiedCount > 0);
            
            /*var updateCommand = Builders<UserDocument>.Update.Push("Tasks", updatedTask);
            var updatedUser = await _collection.FindOneAndUpdateAsync(filterUser, updateCommand);
            var taskUpdated = updatedUser?.Tasks.Find(t => t.Id.Equals(idTask));
            return taskUpdated != null;*/
        }
    }
}