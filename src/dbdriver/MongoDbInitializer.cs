using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using MongoCore.DbDriver.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCore.DbDriver
{
    public interface IDbInitializer
    {
        void EnsureSeedData();
    }
    
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly string _connection;
        public MongoDbInitializer(string connection)
        {
            _connection = connection;
        }

        public void EnsureSeedData()
        {
            var context = new MongoDbContext(_connection);
            IMongoDatabase db = context.Database("mongocore");
            IMongoCollection<UserDocument> userCollection = context.Collection<UserDocument>("mongocore", "usertasks");
        
            var count = userCollection.Count(new BsonDocument());
            if (count > 0) {
                Console.WriteLine($">>>>> {count} documents already exist in collection");
                var filter = Builders<UserDocument>.Filter.Eq("Username", "sapaul");
                var user = userCollection.Find(filter).First();
                Console.WriteLine($"\n[Admin Info] {user.FirstName} {user.LastName}");
                if (PasswordHasher.Match("sapaul", user.PasswordHash))
                    Console.WriteLine("Password Match test #1 passed");
                if (!PasswordHasher.Match("sapauul", user.PasswordHash))
                    Console.WriteLine("Password Match test #2 passed");
                return;
            }

            var task1 = new TaskDocument
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Remember the milk!",
                Description = "",
                Done = true,
            };
            var task2 = new TaskDocument
            {
                Id = ObjectId.GenerateNewId(),
                Name = "update bio",
                Description = "",
                Done = false
            };
            userCollection.InsertOne(new UserDocument
            {
                Admin = true,
                FirstName = "Santanu",
                LastName = "Paul",
                Username = "sapaul",
                Email = "sapaul@example.com",
                PasswordHash = PasswordHasher.Generate("sapaul"),
                Tasks = new List<TaskDocument>(new[]{task1, task2})
            });
            userCollection.InsertOne(new UserDocument
            {
                Admin = false,
                FirstName = "Joe",
                LastName = "Doe",
                Username = "jodoe",
                Email = "jodoe@example.com",
                PasswordHash = PasswordHasher.Generate("jodoe"),
                Tasks = new List<TaskDocument>(new[]{new TaskDocument
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = "John's task 1",
                    Done = false
                }})
            });
            userCollection.InsertOne(new UserDocument
            {
                Admin = false,
                FirstName = "Jane",
                LastName = "Doe",
                Username = "jndoe",
                Email = "janedoe@example.com",
                PasswordHash = PasswordHasher.Generate("jndoe"),
                Tasks = new List<TaskDocument>(new[]{
                    new TaskDocument {
                        Id = ObjectId.GenerateNewId(),
                        Name = "Jane's task 1",
                        Description = "some description",
                        Done = false
                    }, new TaskDocument
                    {
                        Id = ObjectId.GenerateNewId(),
                        Name = "Jane's task 2",
                        Description = "some more description",
                        Done = true 
                    }})
            });
            
            count = userCollection.Count(new BsonDocument());
            Console.WriteLine($">>>>> {count} documents created in collection");
        }
        
    }
}