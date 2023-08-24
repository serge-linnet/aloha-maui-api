using Microsoft.Azure.Cosmos;

namespace AlohaMaui.Core.Providers
{
    public interface ICosmosContainerProvider
    {
        Container GetContainer();
    }

    public class CosmosContainerProvider : ICosmosContainerProvider
    {
        private readonly string _name;
        private readonly ICosmosDatabaseProvider _databaseProvider;

        public CosmosContainerProvider(string name, ICosmosDatabaseProvider databaseProvider)
        {
            _name = name;
            _databaseProvider = databaseProvider;
        }

        public Container GetContainer()
        {
            var database = _databaseProvider.GetDatabase();
            return database.GetContainer(_name);
        }
    }
}
