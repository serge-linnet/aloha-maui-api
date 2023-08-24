using Google.Apis.Auth;

namespace AlohaMaui.Api
{
    public interface IGoogleAuthValidator
    {
        Task<GoogleJsonWebSignature.Payload> Validate(string credentials);
    }

    public class GoogleAuthValidator : IGoogleAuthValidator
    {
        private readonly string _clientId;

        public GoogleAuthValidator(string clientId)
        {
            _clientId = clientId;
        }

        public async Task<GoogleJsonWebSignature.Payload> Validate(string credentials)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);

            return payload;
        }
    }
}
