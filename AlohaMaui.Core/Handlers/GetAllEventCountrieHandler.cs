using AlohaMaui.Core.Queries;
using AlohaMaui.Core.Repositories;
using MediatR;

namespace AlohaMaui.Core.Handlers
{
    public class GetAllEventCountrieHandler : IRequestHandler<GetAllEventCountriesQuery, IEnumerable<string>>
    {
        private readonly ICommunityEventRepository _repository;

        public GetAllEventCountrieHandler(ICommunityEventRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<string>> Handle(GetAllEventCountriesQuery request, CancellationToken cancellationToken)
        {
            return _repository.FindAllCountries();
        }
    }
}
