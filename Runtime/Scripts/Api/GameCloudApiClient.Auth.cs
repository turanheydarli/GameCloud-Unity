using System.Collections;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator Authenticate(string username, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            var request = new AuthRequest 
            { 
                provider = 3,
                username = username 
            };

            return Post("/players/authenticate", request, (AuthResponse response) => {
                SetAuthToken(response.token);
                onSuccess?.Invoke(response);
            }, onError);
        }
    }
} 