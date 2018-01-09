using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace pg.Mongo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // use a connection string
            var client = new MongoClient("mongodb://localhost:27017");
            
            // or, to connect to a replica set, with auto-discovery of the primary, supply a seed list of members
            //var client = new MongoClient("mongodb://localhost:27017,localhost:27018,localhost:27019");

            var db = client.GetDatabase("mongocore");
            var coll = db.GetCollection<BsonDocument>("playground");
            var doc = new BsonDocument
            {
                { "name", "MongoDB" },
                { "type", "Database" },
                { "count", 1 },
                { "info", new BsonDocument
                    {
                        { "x", 203 },
                        { "y", 102 }
                    }     
                }
            };
            coll.InsertOne(doc);
            // await coll.InsertOneAsync(doc);
            Console.WriteLine("Added a single document");
            
            var docs = Enumerable.Range(0, 100).Select(i => new BsonDocument
            {
                {"name", "tasks"},
                {"counter", i}
            });
            coll.InsertMany(docs);
            var count = coll.Count(new BsonDocument());
            Console.WriteLine($"Added {count - 1} more documents");
            
            var foundDoc = coll.Find(new BsonDocument()).FirstOrDefault();
            Console.WriteLine($"First Document: {foundDoc.ToString()}");

            Console.WriteLine("Finding document with counter == 50");
            var filter = Builders<BsonDocument>.Filter.Eq("counter", 50);
            foundDoc = coll.Find(filter).First();
            Console.WriteLine(foundDoc);

            Console.WriteLine("Updating a document whose counter == 66, to 131");
            filter = Builders<BsonDocument>.Filter.Eq("counter", 66);
            var update = Builders<BsonDocument>.Update.Set("counter", 131);
            coll.UpdateOne(filter, update);
            filter = Builders<BsonDocument>.Filter.Eq("counter", 131);
            foundDoc = coll.Find(filter).First();
            Console.WriteLine(foundDoc);
            
            Console.Write("Enter a number (0 to 100): ");
            var enteredNum = Console.ReadLine();
            int num = Int32.Parse(enteredNum);
            Console.WriteLine($"Deleting document with counter == {num}");
            filter = Builders<BsonDocument>.Filter.Eq("counter", num);
            var result = coll.DeleteOne(filter);
            Console.WriteLine($"Deleted {result.DeletedCount} document");

            Console.WriteLine("::<== DONE ==>::");
        }
    }
}
