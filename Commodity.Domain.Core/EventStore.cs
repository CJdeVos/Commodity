using System;
using Commodity.Interfaces;
using System.Linq;
using Commodity.Domain.Core.Interfaces;
using System.Collections.Generic;
using MongoDB.Bson;
using Ninject;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Commodity.Domain.Core
{
    public class EventStore : IEventStore
    {
        //private readonly IEventStorage _eventStorage;
        //private readonly IEventPublisher _eventPublisher;
        //public EventStore(IEventStorage eventStorage, IEventPublisher eventPublisher)
        //{
        //    _eventStorage = eventStorage;
        //    _eventPublisher = eventPublisher;
        //}

        //private TAggregate CreateNewAggregate<TAggregate>(Guid id)
        //    where TAggregate : Aggregate
        //{
        //    var constructorInfo = typeof(TAggregate).GetConstructor(new[] { typeof(Guid) });
        //    if (constructorInfo == null)
        //        throw new Exception("Constructor of aggregate root to pass Guid id not found.");

        //    TAggregate aggregateRoot = (TAggregate)constructorInfo.Invoke(new[] { (object)id });
        //    return aggregateRoot;
        //}

        //public TAggregate Load<TAggregate>(Guid aggregateRootId) 
        //    where TAggregate : Aggregate
        //{
        //    if (aggregateRootId == Guid.Empty)
        //        throw new ArgumentNullException("aggregateRootId");

        //    var eventStream = _eventStorage.GetEventStream(aggregateRootId);
        //    if (eventStream == null)
        //        return default(TAggregate);

        //    return CreateNewAggregate<TAggregate>(aggregateRootId);

        //    // without playing?
        //    // play events?
        //    //aggr.Replay(eventStream);

        //    //return aggregateRoot;
        //}

        //public TAggregate LoadNew<TAggregate>() where TAggregate : Aggregate
        //{
        //    return CreateNewAggregate<TAggregate>(Guid.NewGuid());
        //}

        //public void Persist<TAggregate>(TAggregate aggregate)
        //    where TAggregate : Aggregate
        //{
        //    if (aggregate == null)
        //        throw new ArgumentNullException("aggregate");

        //    // test for uncommitted save unsaved changes
        //    var uncommittedEvents = aggregate.GetUncommittedEvents().ToArray();
        //    if (uncommittedEvents.Any() == false)
        //        return;

        //    _eventStorage.Persist(aggregate.AggregateId, uncommittedEvents);

        //    // publish events
        //    _eventPublisher.Publish(aggregate.AggregateId, uncommittedEvents);

        //    // commit actual aggregate
        //    aggregate.Commit();
        //}

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
