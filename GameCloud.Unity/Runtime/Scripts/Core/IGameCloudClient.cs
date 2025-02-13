using System;
using System.Collections.Generic;
#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

using GameCloud.Models;
using JetBrains.Annotations;

namespace GameCloud
{
    public interface IGameCloudClient
    {
        IGameCloudSession Session { get; }
        string Host { get; }
        int Port { get; }
        bool SSL { get; }

        void SetSession(IGameCloudSession session);
        void AuthenticateCustom(string username, Action<IGameCloudSession> onSuccess, Action<ProblemDetails> onError);

        void AuthenticateWithDevice(string deviceId, Dictionary<string, object> metadata = null,
            Action<IGameCloudSession> onSuccess = null, Action<ProblemDetails> onError = null);

        void AuthenticateWithCustomId(string customId, Dictionary<string, object> metadata = null, bool create = true,
            Action<IGameCloudSession> onSuccess = null, Action<ProblemDetails> onError = null);

        void RefreshSession(string sessionId, Action<IGameCloudSession> onSuccess = null,
            Action<ProblemDetails> onError = null);

        void CreateMatchmakingTicket(string queueName, Dictionary<string, object> properties,
            Action<MatchmakingTicket> onSuccess, Action<ProblemDetails> onError);

        void GetMatch(string matchId, Action<Match> onSuccess, Action<ProblemDetails> onError);

        void SubmitMatchAction(string matchId, string actionType, Dictionary<string, object> actionData,
            Action<MatchAction> onSuccess, Action<ProblemDetails> onError);

        void SetMatchPresence(string matchId, string status, Dictionary<string, object> meta = null,
            Action<Match> onSuccess = null, Action<ProblemDetails> onError = null);

        void GetMatchState(string matchId, Action<MatchState> onSuccess, Action<ProblemDetails> onError);

        void RestoreSession(string authToken, string refreshToken, Action<IGameCloudSession> onSuccess,
            Action<ProblemDetails> onError);

        void GetAttributes(string username, string collection, Action<AttributeCollection> onSuccess,
            Action<ProblemDetails> onError);

        void GetAttribute(string username, string collection, string key, Action<PlayerAttribute> onSuccess,
            Action<ProblemDetails> onError);

        void SetAttribute(string username, string collection, AttributeWriteRequest request, Action onSuccess,
            Action<ProblemDetails> onError);

        void DeleteAttribute(string username, string collection, string key, Action onSuccess,
            Action<ProblemDetails> onError);
        void GetMatchmakingTicket(string ticketId, Action<MatchmakingTicket> onSuccess, Action<ProblemDetails> onError);
        void GetPlayer(string playerId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError);
        void GetPlayerByUsername(string username, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError);
        void GetPlayerByCustomId(string customId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError);
        void GetPlayerByDeviceId(string deviceId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError);

        void UpdatePlayerMetadata(string playerId, Dictionary<string, object> metadata,
            Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError);

        void UpdatePlayerStatus(string playerId, PlayerStatus status, Action<PlayerResponse> onSuccess,
            Action<ProblemDetails> onError);

        void ProcessMatchmaking(string queueId, Action<List<Match>> onSuccess, Action<ProblemDetails> onError);
#if UNITASK_SUPPORT
        UniTask<IGameCloudSession> AuthenticateCustomAsync(string username);
        UniTask<AttributeCollection> GetAttributesAsync(string username, string collection);
        UniTask<PlayerAttribute> GetAttributeAsync(string username, string collection, string key);
        UniTask SetAttributeAsync(string username, string collection, AttributeWriteRequest request);
        UniTask DeleteAttributeAsync(string username, string collection, string key);
        UniTask<MatchmakingTicket> GetMatchmakingTicketAsync(string ticketId);
        UniTask<MatchmakingTicket> CreateMatchmakingTicketAsync(string queueName, Dictionary<string, object> properties);

        UniTask<Match> GetMatchAsync(string matchId);
        UniTask<List<Match>> ProcessMatchmakingAsync([CanBeNull] string queueId = null);

        UniTask<IGameCloudSession> AuthenticateWithDeviceAsync(string deviceId,
            Dictionary<string, object> metadata = null);

        UniTask<IGameCloudSession> AuthenticateWithCustomIdAsync(string customId,
            Dictionary<string, object> metadata = null, bool create = true);

        UniTask<IGameCloudSession> RefreshSessionAsync(string sessionId);
        UniTask<IGameCloudSession> RestoreSessionAsync(string authToken, string refreshToken);

        UniTask<MatchAction> SubmitMatchActionAsync(string matchId, string actionType,
            Dictionary<string, object> actionData);

        UniTask<Match> SetMatchPresenceAsync(string matchId, string status, Dictionary<string, object> meta = null);
        UniTask<MatchState> GetMatchStateAsync(string matchId);

        // Player methods with UniTask
        UniTask<PlayerResponse> GetPlayerAsync(string playerId);
        UniTask<PlayerResponse> GetPlayerByUsernameAsync(string username);
        UniTask<PlayerResponse> GetPlayerByCustomIdAsync(string customId);
        UniTask<PlayerResponse> GetPlayerByDeviceIdAsync(string deviceId);
        UniTask<PlayerResponse> UpdatePlayerMetadataAsync(string playerId, Dictionary<string, object> metadata);
        UniTask<PlayerResponse> UpdatePlayerStatusAsync(string playerId, PlayerStatus status);
#endif
    }
}