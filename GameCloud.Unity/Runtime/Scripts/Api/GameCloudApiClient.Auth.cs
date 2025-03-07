using System;
using GameCloud.Proto;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public void Authenticate(string token, string deviceId, Action<SessionConnect> onSuccess = null, Action<Error> onError = null)
        {
            var connectRequest = new SessionConnect
            {
                Token = token,
                DeviceId = deviceId
            };
            
            Send(connectRequest, response => {
                if (response.Error != null)
                {
                    onError?.Invoke(response.Error);
                    return;
                }
                
                // Start heartbeat after successful authentication
                StartHeartbeat();
                
                onSuccess?.Invoke(connectRequest);
            });
        }
    }
}