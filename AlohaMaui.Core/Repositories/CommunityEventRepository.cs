using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Filters;
using AlohaMaui.Core.Providers;
using Microsoft.Azure.Cosmos;
using System.Diagnostics.Metrics;
using static AlohaMaui.Core.Repositories.CommunityEventRepository;

namespace AlohaMaui.Core.Repositories
{

    public interface ICommunityEventRepository
    {
        Task<CommunityEvent> Find(Guid id);
        Task<IEnumerable<CommunityEvent>> FindPublicEvents(PublicCommunityEventsFilter filter);
        Task<CommunityEvent> CreateEvent(CommunityEvent entity);
        Task<CommunityEvent> UpdateEvent(CommunityEvent entity);
        Task<IEnumerable<CommunityEvent>> FindEventsForUser(Guid userId);
        Task<CommunityEvent> Update(CommunityEvent entity);
        Task<IEnumerable<CommunityEvent>> FindEventsForAdmin(ManageCommunityEventsFilter filter);
        Task<IEnumerable<string>> FindAllCountries();
    }

    public class CommunityEventRepository : ICommunityEventRepository
    {
        private readonly ICosmosContainerProvider _containerProvider;

        public CommunityEventRepository(ICosmosContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public async Task<CommunityEvent> Find(Guid id)
        {
            var container = _containerProvider.GetContainer();
            var result = await container.ReadItemAsync<CommunityEvent>(id.ToString(), new PartitionKey(id.ToString()));
            return result;
        }

        public async Task<IEnumerable<CommunityEvent>> FindPublicEvents(PublicCommunityEventsFilter filter)
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("" +
                "SELECT * from events e " +
                "WHERE " +
                "   e.type = @type" +
                "   AND e.Status = @status" +
                "   AND (IS_NULL(@from) OR e.StartsAt >= @from)" +
                "   AND (IS_NULL(@to) OR e.EndsAt <= @to)" +
                "   AND (IS_NULL(@familyFriendly) OR e.FamilyFriendly = @familyFriendly)" +
                "   AND (IS_NULL(@country) OR e.Place.Country = @country) " +
                "   AND (IS_NULL(@isOffline) OR e.IsOffline = @isOffline) " +
                "ORDER BY e.StartsAt"
                )
                .WithParameter("@type", EventType.Community)
                .WithParameter("@status", EventStatus.Approved)
                .WithParameter("@from", filter.From)
                .WithParameter("@to", filter.To?.Date)
                .WithParameter("@familyFriendly", filter.FamilyFriendly)
                .WithParameter("@country", filter.Country)
                .WithParameter("@isOffline", filter.IsOffline);

            var results = new List<CommunityEvent>();

            using var iterator = container.GetItemQueryIterator<CommunityEvent>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<CommunityEvent> CreateEvent(CommunityEvent entity)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.CreateItemAsync(entity, new PartitionKey(entity.Id.ToString()));
            return newEntity;
        }

        public async Task<CommunityEvent> UpdateEvent(CommunityEvent entity)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.UpsertItemAsync(entity, new PartitionKey(entity.Id.ToString()));
            return newEntity;
        }

        public async Task<IEnumerable<CommunityEvent>> FindEventsForUser(Guid userId)
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("SELECT * from events e WHERE e.UserId = @id ORDER BY e.StartsAt")
                    .WithParameter("@id", userId);

            var results = new List<CommunityEvent>();
            using var iterator = container.GetItemQueryIterator<CommunityEvent>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<CommunityEvent> Update(CommunityEvent entity)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.UpsertItemAsync(entity, new PartitionKey(entity.Id.ToString()));
            return newEntity;
        }

        public async Task<IEnumerable<CommunityEvent>> FindEventsForAdmin(ManageCommunityEventsFilter filter)
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("" +
                "SELECT * from events e " +
                "WHERE " +
                "   e.type = @type" +
                "   AND e.Status = @pending" +
                (filter.IncludeApproved ? " OR e.Status = @approved" : "") +
                (filter.IncludeRejected ? " OR e.Status = @rejected" : "") +
                "   ORDER BY e.StartsAt"
                )
                .WithParameter("@type", EventType.Community)
                .WithParameter("@approved", EventStatus.Approved)
                .WithParameter("@pending", EventStatus.Pending)
                .WithParameter("@rejected", EventStatus.Rejected);

            var results = new List<CommunityEvent>();

            using var iterator = container.GetItemQueryIterator<CommunityEvent>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<IEnumerable<string>> FindAllCountries()
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("SELECT DISTINCT(e.Place.Country) as Name FROM events e ");

            var results = new List<CountryResult>();

            using var iterator = container.GetItemQueryIterator<CountryResult>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(x => x.Name!);
        }
    }

    class CountryResult
    {
        public string? Name { get; set; }
    }
}
