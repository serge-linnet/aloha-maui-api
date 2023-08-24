using AlohaMaui.Core.Providers;
using Microsoft.Azure.Cosmos;
using User = AlohaMaui.Core.Entities.User;

namespace AlohaMaui.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> FindByEmail(string email);
        Task<User> CreateUser(User user);
        Task UpdateUser(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ICosmosContainerProvider _containerProvider;

        public UserRepository(ICosmosContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public async Task<User?> FindByEmail(string email)
        {
            var container = _containerProvider.GetContainer();
            var query = new QueryDefinition("SELECT TOP 1 * from users u WHERE StringEquals(u.email, @email)")
                    .WithParameter("@email", email);

            using var iterator = container.GetItemQueryIterator<User>(query);
            if (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync();
                return result.FirstOrDefault();
            }
            return null;
        }

        public async Task<User> CreateUser(User user)
        {
            var container = _containerProvider.GetContainer();
            var newUser = await container.CreateItemAsync(user, new PartitionKey(user.Email));
            return newUser;
        }

        public Task UpdateUser(User user)
        {
            var container = _containerProvider.GetContainer();
            return container.UpsertItemAsync(user, new PartitionKey(user.Email));
        }
    }
}
