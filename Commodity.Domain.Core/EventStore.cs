using Commodity.Interfaces;
using System.Linq;
using Commodity.Domain.Core.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using Ninject;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Commodity.Domain.Core
{
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
            BsonDocument streamDocument = await collection.Find(doc => doc["_id"] == streamName).SingleAsync();
            
            // streamdocument currently contains all events within 1 document
            return new EventStream(CreateEnumerator(streamDocument["Events"].AsBsonArray));
        }

        private IEnumerator<EventData> CreateEnumerator(BsonArray bsonArray)
        {
            foreach (BsonValue bsonValue in bsonArray)
            {
                yield return ConstructEventData(bsonValue);
            }
        }

        public async void AppendToEventStream(string streamName, int expectedVersion, IEnumerable<IAggregateEvent> events)
        {

            var x = events.Select(@event => @event.ToBsonDocument()).ToList();

            
            //BsonSerializer.Deserialize()
            var collection = _database.GetCollection<BsonDocument>("events");
            UpdateResult res = await collection.UpdateOneAsync(
                Builders<BsonDocument>.Filter.Eq("_id", streamName),
                Builders<BsonDocument>.Update.Push("Events", "testwaarde")
            );
            if (res.MatchedCount==0)
            {
                BsonDocument newDoc = new BsonDocument()
                {
                    { "Events", new BsonArray()
                    {
                        "initialwaarde"
                    } }
                };
                // create the document
                await collection.InsertOneAsync(newDoc);
            }
//            BsonDocument streamDocument = await collection.Find(doc => doc["_id"] == streamName).FirstOrDefaultAsync();

            // append?

            //throw new NotImplementedException();
        }

        private EventData ConstructEventData(BsonValue bsonValue)
        {
            return null;
        }
    }
}
