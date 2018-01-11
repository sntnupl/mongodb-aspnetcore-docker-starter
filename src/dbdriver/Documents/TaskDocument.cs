using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoCore.DbDriver.Documents
{
    [BsonIgnoreExtraElements]
    public class TaskDocument
    {
        public ObjectId Id { get; set; }   
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
    }
}