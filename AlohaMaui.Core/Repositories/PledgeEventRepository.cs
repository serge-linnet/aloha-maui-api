using AlohaMaui.Core.Entities;
using AlohaMaui.Core.Providers;
using Microsoft.Azure.Cosmos;
using System.Text.RegularExpressions;

namespace AlohaMaui.Core.Repositories
{

    public interface IPledgeEventRepository
    {
        Task<IEnumerable<PledgeEvent>> FindAll();
        Task<PledgeEvent> Create(PledgeEvent pledge);
        Task<IEnumerable<PledgeTotals>> FindTotals();
    }

    public class PledgeEventRepository : IPledgeEventRepository
    {
        private readonly ICosmosContainerProvider _containerProvider;

        public PledgeEventRepository(ICosmosContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public async Task<PledgeEvent> Create(PledgeEvent pledge)
        {
            var container = _containerProvider.GetContainer();
            var newEntity = await container.CreateItemAsync(pledge, new PartitionKey(pledge.Id.ToString()));
            return newEntity;
        }

        public async Task<IEnumerable<PledgeEvent>> FindAll()
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("" +
                "SELECT * from events e " +
                "WHERE " +
                "   e.type = @type" +
                "   AND e.Status = @status"
                )
                .WithParameter("@type", EventType.Pledge)
                .WithParameter("@status", EventStatus.Approved);

            var results = new List<PledgeEvent>();

            using var iterator = container.GetItemQueryIterator<PledgeEvent>(query);
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<IEnumerable<PledgeTotals>> FindTotals()
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("" +
                "SELECT " +
                "    e.Country, " +
                "    count(1) AS Pledges, " +
                "    sum(e.NumberOfPeople) AS People " +
                "FROM events e " +
                "WHERE e.type = 1 " +
                "GROUP BY e.Country "
            );

            var results = new List<PledgeTotals>();

            using var iterator = container.GetItemQueryIterator<PledgeTotals>(query);
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
