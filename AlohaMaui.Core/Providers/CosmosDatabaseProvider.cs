using Microsoft.Azure.Cosmos;

namespace AlohaMaui.Core.Providers
{
    public interface ICosmosDatabaseProvider
    {
        Database GetDatabase();
    }

    public class CosmosDatabaseProvider : ICosmosDatabaseProvider
    {
        private readonly string _database;
        private readonly ICosmosClientProvider _clientProvider;

        public CosmosDatabaseProvider(string database, ICosmosClientProvider clientProvider)
        {
            _database = database;
            _clientProvider = clientProvider;
        }

        public Database GetDatabase()
        {
            var client = _clientProvider.GetClient();
            var database = client.GetDatabase(_database);
            return database;
        }
    }
}
