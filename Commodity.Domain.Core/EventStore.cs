using System;
using Commodity.Interfaces;
using System.Linq;
using Commodity.Domain.Core.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using Ninject;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace Commodity.Domain.Core
{
    internal static class EventStoreExtensions
    {
        internal static async Task<BsonDocument> FindStream(this IMongoCollection<BsonDocument> collection, string streamName)
        {
            return await collection.Find(doc => doc["_id"] == streamName).SingleOrDefaultAsync();
        }
    }

    public class EventStore : IEventStore
    {
        private readonly IMongoDatabase _database;
        public EventStore([Named("Events")]IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<EventStream> GetEventStream(string streamName, int startVersion, int? untilVersion)
        {
            // currently we always have just one page
            var collection = _database.GetCollection<BsonDocument>("events");
            BsonDocument streamDocument = await collection.FindStream(streamName);
            
            // streamdocument currently contains all events within 1 document
            return new EventStream(CreateEnumerator(streamDocument["Events"].AsBsonArray));
        }

        private IEnumerator<IAggregateEvent> CreateEnumerator(BsonArray bsonArray)
        {
            foreach (BsonValue bsonValue in bsonArray)
            {
                yield return ConstructEvent(bsonValue);
            }
        }

        public async void AppendToEventStream(string streamName, int expectedVersion, IEnumerable<IAggregateEvent> events)
        {

            var x = events.Select(@event => @event.ToBsonDocument()).ToList();

            
            //BsonSerializer.Deserialize()
            var collection = _database.GetCollection<BsonDocument>("events");

            BsonDocument streamDocument = await collection.FindStream(streamName);
            if (streamDocument == null)
            {
                if (expectedVersion > 0)
                    throw new Exception("Stream not found but expected a version>0.");
            }
            else
            {
                var version = streamDocument["committedVersion"].AsInt32;
                if (version != expectedVersion) 
                    throw new Exception(String.Format("Stream found, but version is different. Current version is {0}, expected is {1}.", version, expectedVersion));
            }
            
            var eventsAsBsonDocuments = events.Select(@event => @event.ToBsonDocument()).ToList();

            // Update the document
            UpdateResult res = await collection.UpdateOneAsync(
                Builders<BsonDocument>.Filter.Eq("_id", streamName),
                Builders<BsonDocument>.Update
                .PushEach("Events", eventsAsBsonDocuments)
                .Set("committedVersion", expectedVersion+ eventsAsBsonDocuments.Count())
            );

            if (res.MatchedCount == 0)
            {
                BsonDocument newDoc = new BsonDocument()
                {
                    {"_id", streamName},
                    {
                        "Events",
                        new BsonArray(eventsAsBsonDocuments)
                    },
                    {"committedVersion", eventsAsBsonDocuments.Count}
                };
                // create the document
                await collection.InsertOneAsync(newDoc);
            }
        }

        private IAggregateEvent ConstructEvent(BsonValue bsonValue)
        {
            return BsonSerializer.Deserialize<IAggregateEvent>(bsonValue.AsBsonDocument);
        }

        public string[] GetEventNamesUsedInStore()
        {
            throw new NotImplementedException();
        }
    }
}
