using System.Collections;
using System.Collections.Generic;
using GameCloud.Models;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator AuthenticateDevice(string deviceId, Dictionary<string, object> metadata, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            var request = new DeviceAuthRequest 
            { 
                deviceId = deviceId,
                metadata = metadata
            };

            return Post("/players/authenticate/device", request, (AuthResponse response) => {
                SetAuthToken(response.token);
                onSuccess?.Invoke(response);
            }, onError);
        }

        public IEnumerator AuthenticateCustom(string customId, Dictionary<string, object> metadata, bool create, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            var request = new CustomAuthRequest 
            { 
                customId = customId,
                metadata = metadata,
                create = create
            };

            return Post("/players/authenticate/custom", request, (AuthResponse response) => {
                SetAuthToken(response.token);
                onSuccess?.Invoke(response);
            }, onError);
        }

        public IEnumerator RefreshSession(string sessionId, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            var request = new SessionRefreshRequest 
            { 
                sessionId = sessionId 
            };

            return Post("/players/authenticate/refresh", request, onSuccess, onError);
        }
    }
}