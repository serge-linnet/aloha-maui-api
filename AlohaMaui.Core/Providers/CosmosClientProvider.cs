using Microsoft.Azure.Cosmos;

namespace AlohaMaui.Core.Providers
{
    public interface ICosmosClientProvider
    {
        CosmosClient GetClient();
    }

    public class CosmosClientProvider : ICosmosClientProvider
    {
        private readonly string _account;
        private readonly string _key;

        public CosmosClientProvider(string account, string key)
        {
            _account = account;
            _key = key;
        }

        public CosmosClient GetClient()
        {
            var client = new CosmosClient(_account, _key);
            return client;
        }
    }
}
