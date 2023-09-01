using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Providers;
using Microsoft.Azure.Cosmos;
using System.Text.RegularExpressions;

namespace AlohaMaui.Core.Repositories
{

    public interface IOrgEventRepository
    {
        Task<IEnumerable<OrganizationEvent>> FindAll();
        Task<OrganizationEvent> Create(OrganizationEvent pledge);
        Task<IEnumerable<OrgEventTotals>> FindTotals();
    }

    public class OrgEventRepository : IOrgEventRepository
    {
        private readonly ICosmosContainerProvider _containerProvider;

        public OrgEventRepository(ICosmosContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public async Task<OrganizationEvent> Create(OrganizationEvent pledge)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.CreateItemAsync(pledge, new PartitionKey(pledge.Id.ToString()));
            return newEntity;
        }

        public async Task<IEnumerable<OrganizationEvent>> FindAll()
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("" +
                "SELECT * from events e " +
                "WHERE " +
                "   e.type = @type" +
                "   AND e.Status = @status"
                )
                .WithParameter("@type", EventType.OrgEvent)
                .WithParameter("@status", EventStatus.Approved);

            var results = new List<OrganizationEvent>();

            using var iterator = container.GetItemQueryIterator<OrganizationEvent>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<IEnumerable<OrgEventTotals>> FindTotals()
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("" +
                "SELECT " +
                "    e.Country, " +
                "    count(1) AS Pledges, " +
                "    sum(e.NumberOfPeople) AS People " +
                "FROM events e " +
                "WHERE e.type = @type " +
                 "   AND e.Status = @status " +
                "GROUP BY e.Country "
            )
                .WithParameter("@type", EventType.OrgEvent)
                .WithParameter("@status", EventStatus.Approved);


            var results = new List<OrgEventTotals>();

            using var iterator = container.GetItemQueryIterator<OrgEventTotals>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }
    }
}
