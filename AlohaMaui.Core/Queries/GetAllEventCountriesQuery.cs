using MediatR;

namespace AlohaMaui.Core.Queries
{
    public class GetAllEventCountriesQuery : IRequest<IEnumerable<string>>
    {
        public string GetCacheKey()
        {
            return $"{GetType().Name}";
        }
    }
}
