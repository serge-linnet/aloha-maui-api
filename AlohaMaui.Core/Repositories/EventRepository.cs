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
    }
}
