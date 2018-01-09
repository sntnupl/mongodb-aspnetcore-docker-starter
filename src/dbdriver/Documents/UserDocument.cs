using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoCore.DbDriver.Documents
{
    [BsonIgnoreExtraElements]
    public class UserDocument
    {
        public ObjectId id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool Admin { get; set; }
        public List<TaskDocument> Tasks { get; set; }
    }
}