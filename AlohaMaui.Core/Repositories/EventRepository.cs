using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Providers;
using Microsoft.Azure.Cosmos;

namespace AlohaMaui.Core.Repositories
{
    public interface IEventRepository
    {
        Task<Event> Find(Guid id);
        IEnumerable<Event> FindEvents(string query);
        IEnumerable<Event> FindPendingEvents();
        Task<Event> CreateEvent(Event entity);
        Task<IEnumerable<Event>> FindEventsForUser(Guid userId);
        Task<Event> Update(Event entity);
    }

    public class EventRepository : IEventRepository
    {
        private readonly ICosmosContainerProvider _containerProvider;

        public EventRepository(ICosmosContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public async Task<Event> Find(Guid id)
        {
            var container = _containerProvider.GetContainer();
            var result = await container.ReadItemAsync<Event>(id.ToString(), new PartitionKey(id.ToString()));
            return result;
        }

        public IEnumerable<Event> FindEvents(string query)
        {
            var container = _containerProvider.GetContainer();
            var events = container.GetItemLinqQueryable<Event>(allowSynchronousQueryExecution: true).ToList()
                .Where(x => x.Status == EventStatus.Approved);
            return events;
        }

        public IEnumerable<Event> FindPendingEvents()
        {
            var container = _containerProvider.GetContainer();
            var events = container.GetItemLinqQueryable<Event>(allowSynchronousQueryExecution: true).ToList()
                .Where(x => x.Status == EventStatus.Pending);
            return events;
        }

        public async Task<Event> CreateEvent(Event entity)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.CreateItemAsync(entity, new PartitionKey(entity.Id.ToString()));
            return newEntity;
        }

        public async Task<IEnumerable<Event>> FindEventsForUser(Guid userId)
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("SELECT * from events e WHERE e.userId = @id")
                    .WithParameter("@id", userId);

            var results = new List<Event>();
            using var iterator = container.GetItemQueryIterator<Event>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<Event> Update(Event entity)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.UpsertItemAsync(entity, new PartitionKey(entity.Id.ToString()));
            return newEntity;
        }
    }
}
