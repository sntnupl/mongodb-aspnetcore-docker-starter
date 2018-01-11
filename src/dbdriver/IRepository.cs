using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoCore.DbDriver.Documents;
using MongoDB.Bson;

namespace MongoCore.DbDriver
{
    public interface IRepository
    {
        Task<bool> UserExistsAsync(ObjectId idUser);
        IEnumerable<UserDocument> GetAllUsers();
        Task<IEnumerable<UserDocument>> GetAllUsersAsync();
        
        UserDocument GetUser(ObjectId idUser);
        Task<UserDocument> GetUserAsync(ObjectId idUser);

        Task<IEnumerable<TaskDocument>> GetAllTasksAsync(ObjectId idUser);
        Task<TaskDocument> GetTaskAsync(ObjectId idUser, ObjectId idTask);
        
        Task<TaskDocument> AddTaskAsync(ObjectId idUser, TaskDocument task);
        Task<bool> DeleteTaskAsync(ObjectId idUser, ObjectId idTask);
        Task<bool> UpdateTaskAsync(ObjectId idUser, ObjectId idTask, TaskDocument task);

    }
}