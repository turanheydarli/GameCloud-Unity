using System;
using System.Collections;
using System.Collections.Generic;
using GameCloud.Models;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator GetPlayer(string playerId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<PlayerResponse>($"/players/{playerId}", onSuccess, onError);
        }

        public IEnumerator GetPlayerByUsername(string username, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<PlayerResponse>($"/players/username/{username}", onSuccess, onError);
        }

        public IEnumerator GetPlayerByCustomId(string customId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<PlayerResponse>($"/players/custom/{customId}", onSuccess, onError);
        }

        public IEnumerator GetPlayerByDeviceId(string deviceId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<PlayerResponse>($"/players/device/{deviceId}", onSuccess, onError);
        }

        public IEnumerator UpdatePlayerMetadata(string playerId, Dictionary<string, object> metadata, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            return Put($"/players/{playerId}/metadata", metadata, onSuccess, onError);
        }

        public IEnumerator UpdatePlayerStatus(string playerId, PlayerStatus status, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            return Put($"/players/{playerId}/status", new { status = status.ToString().ToLower() }, onSuccess, onError);
        }
    }
}