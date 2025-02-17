#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using GameCloud.Api;
using GameCloud.Core.Exceptions;
using GameCloud.Models;

namespace GameCloud
{
    public class GameCloudClient : IGameCloudClient
    {
        public static string DefaultHost = "localhost";
        public static int DefaultPort = 7257;

        private readonly GameCloudApiClient apiClient;
        private IGameCloudSession session;

        public IGameCloudSession Session => session;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool SSL { get; private set; }

        private GameCloudClient(string host, int port, string gameKey, bool ssl, bool useLogger)
        {
            Host = host;
            Port = port;
            SSL = ssl;
            apiClient = new GameCloudApiClient(host, port, gameKey, ssl, useLogger);
        }

        public static GameCloudClient FromSettings(GameCloudSettings settings)
        {
            return new GameCloudClient(settings.Host, settings.Port, settings.GameKey, settings.UseSSL,
                settings.EnableDebugLogs);
        }

        public static GameCloudClient Default(string gameKey)
        {
            return new GameCloudClient(DefaultHost, DefaultPort, gameKey, true, false);
        }

        public static GameCloudClient Create(string host, int port, string gameKey, bool ssl = true,
            bool useLogger = false)
        {
            return new GameCloudClient(host, port, gameKey, ssl, useLogger);
        }

        public void SetSession(IGameCloudSession session)
        {
            this.session = session;
            if (session != null)
            {
                apiClient.SetAuthToken(session.AuthToken);
            }
        }

        public void AuthenticateCustom(string username, Action<IGameCloudSession> onSuccess,
            Action<ProblemDetails> onError)
        {
            apiClient.AuthenticateCustom(username, null, true, response =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void AuthenticateWithDevice(string deviceId, Dictionary<string, object> metadata = null,
            Action<IGameCloudSession> onSuccess = null, Action<ProblemDetails> onError = null)
        {
            apiClient.AuthenticateDevice(deviceId, metadata, response =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void AuthenticateWithCustomId(string customId, Dictionary<string, object> metadata = null,
            bool create = true, Action<IGameCloudSession> onSuccess = null,
            Action<ProblemDetails> onError = null)
        {
            apiClient.AuthenticateCustom(customId, metadata, create, response =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void RefreshSession(string sessionId, Action<IGameCloudSession> onSuccess = null,
            Action<ProblemDetails> onError = null)
        {
            apiClient.RefreshSession(sessionId, response =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void CreateMatchmakingTicket(string queueName, Dictionary<string, object> properties,
            Action<MatchmakingTicket> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            var request = new MatchmakingTicketRequest
            {
                queueName = queueName,
                properties = properties
            };
            apiClient.CreateMatchmakingTicket(request, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void GetMatch(string matchId, Action<Match> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetMatch(matchId, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void SubmitMatchAction(string matchId, string actionType, Dictionary<string, object> actionData,
            Action<MatchAction> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            var request = new MatchActionRequest
            {
                actionType = actionType,
                actionData = actionData
            };
            apiClient.SubmitMatchAction(matchId, request, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void SetMatchPresence(string matchId, string status, Dictionary<string, object> meta = null,
            Action<Match> onSuccess = null, Action<ProblemDetails> onError = null)
        {
            RequireSession();
            apiClient.SetMatchPresence(matchId, status, Guid.NewGuid().ToString(), meta, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void GetMatchState(string matchId, Action<MatchState> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetMatchState(matchId, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        private void RequireSession()
        {
            if (session == null)
            {
                throw new GameCloudException("Active session required");
            }

            if (session.HasExpired)
            {
                session.Refresh(this,
                    () => { },
                    error => throw new GameCloudException($"Session refresh failed: {error.Detail}"));
            }
        }

        public void RestoreSession(string authToken, string refreshToken, Action<IGameCloudSession> onSuccess,
            Action<ProblemDetails> onError)
        {
            apiClient.SetAuthToken(authToken);
            apiClient.RefreshSession(refreshToken, response =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, error => onError?.Invoke(error)).ToCoroutine();
        }

        // Attributes
        public void GetAttributes(string username, string collection, Action<AttributeCollection> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetAttributes(username, collection, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void GetAttribute(string username, string collection, string key,
            Action<PlayerAttribute> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetAttribute(username, collection, key, onSuccess,
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void SetAttribute(string username, string collection, AttributeWriteRequest request,
            Action onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.SetAttribute(username, collection, request, _ => onSuccess?.Invoke(),
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void DeleteAttribute(string username, string collection, string key, Action onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.DeleteAttribute(username, collection, key, () => onSuccess?.Invoke(),
                error => onError?.Invoke(error)).ToCoroutine();
        }

        public void GetPlayer(string playerId, Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetPlayer(playerId, onSuccess, error => onError?.Invoke(error));
        }

        public void GetPlayerByUsername(string username, Action<PlayerResponse> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetPlayerByUsername(username, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void GetPlayerByCustomId(string customId, Action<PlayerResponse> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetPlayerByCustomId(customId, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void GetPlayerByDeviceId(string deviceId, Action<PlayerResponse> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetPlayerByDeviceId(deviceId, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void UpdatePlayerMetadata(string playerId, Dictionary<string, object> metadata,
            Action<PlayerResponse> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.UpdatePlayerMetadata(playerId, metadata, onSuccess, error => onError?.Invoke(error))
                .ToCoroutine();
        }

        public void UpdatePlayerStatus(string playerId, PlayerStatus status, Action<PlayerResponse> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.UpdatePlayerStatus(playerId, status, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void ProcessMatchmaking(string queueId, Action<List<Match>> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.ProcessMatchmaking(queueId, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void EndMatch(string matchId, Dictionary<string, object> finalState, Action<Match> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.EndMatch(matchId, finalState, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

        public void LeaveMatch(string matchId, Action<Match> onSuccess, Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.LeaveMatch(matchId, onSuccess, error => onError?.Invoke(error)).ToCoroutine();
        }

#if UNITASK_SUPPORT
        public async UniTask<IGameCloudSession> AuthenticateCustomAsync(string username)
        {
            var response = await apiClient.PostAsync<AuthResponse>("/players/authenticate/custom",
                new { username, create = true });
            var session = new GameCloudSession(response);
            SetSession(session);
            return session;
        }

        public async UniTask<AttributeCollection> GetAttributesAsync(string username, string collection)
        {
            RequireSession();
            return await apiClient.GetAsync<AttributeCollection>($"/players/{username}/attributes/{collection}");
        }

        public async UniTask<PlayerAttribute> GetAttributeAsync(string username, string collection, string key)
        {
            RequireSession();
            return await apiClient.GetAsync<PlayerAttribute>($"/players/{username}/attributes/{collection}/{key}");
        }

        public async UniTask SetAttributeAsync(string username, string collection, AttributeWriteRequest request)
        {
            RequireSession();
            await apiClient.PutAsync<object>($"/players/{username}/attributes/{collection}", request);
        }

        public async UniTask DeleteAttributeAsync(string username, string collection, string key)
        {
            RequireSession();
            await apiClient.DeleteAsync($"/players/{username}/attributes/{collection}/{key}");
        }

        public async UniTask<MatchmakingTicket> CreateMatchmakingTicketAsync(string queueName,
            Dictionary<string, object> properties)
        {
            RequireSession();
            var request = new MatchmakingTicketRequest
            {
                queueName = queueName,
                properties = properties
            };
            return await apiClient.PostAsync<MatchmakingTicket>("/matchmaking/tickets", request);
        }

        public async UniTask<Match> GetMatchAsync(string matchId)
        {
            RequireSession();
            return await apiClient.GetAsync<Match>($"/matchmaking/matches/{matchId}");
        }

        public void GetMatchmakingTicket(string ticketId, Action<MatchmakingTicket> onSuccess,
            Action<ProblemDetails> onError)
        {
            RequireSession();
            apiClient.GetMatchmakingTicket(ticketId, onSuccess, error => onError?.Invoke(error));
        }

        public async UniTask<IGameCloudSession> AuthenticateWithDeviceAsync(string deviceId,
            Dictionary<string, object> metadata = null)
        {
            var response = await apiClient.PostAsync<AuthResponse>("/players/authenticate/device",
                new { deviceId, metadata });
            var session = new GameCloudSession(response);
            SetSession(session);
            return session;
        }

        public async UniTask<IGameCloudSession> AuthenticateWithCustomIdAsync(string customId,
            Dictionary<string, object> metadata = null, bool create = true)
        {
            var response = await apiClient.PostAsync<AuthResponse>("/players/authenticate/custom",
                new { username = customId, metadata, create });
            var session = new GameCloudSession(response);
            SetSession(session);
            return session;
        }

        public async UniTask<IGameCloudSession> RefreshSessionAsync(string sessionId)
        {
            var response = await apiClient.PostAsync<AuthResponse>("/players/session/refresh",
                new { sessionId });
            var session = new GameCloudSession(response);
            SetSession(session);
            return session;
        }

        public async UniTask<IGameCloudSession> RestoreSessionAsync(string authToken, string refreshToken)
        {
            apiClient.SetAuthToken(authToken);
            var response = await apiClient.PostAsync<AuthResponse>("/players/session/refresh",
                new { sessionId = refreshToken });
            var session = new GameCloudSession(response);
            SetSession(session);
            return session;
        }

        public async UniTask<MatchAction> SubmitMatchActionAsync(string matchId, string actionType,
            Dictionary<string, object> actionData)
        {
            RequireSession();
            var request = new MatchActionRequest
            {
                actionType = actionType,
                actionData = actionData
            };
            return await apiClient.PostAsync<MatchAction>($"/matchmaking/matches/{matchId}/actions", request);
        }

        public async UniTask<Match> SetMatchPresenceAsync(string matchId, string status,
            Dictionary<string, object> meta = null)
        {
            RequireSession();
            return await apiClient.PostAsync<Match>($"/matchmaking/matches/{matchId}/presence",
                new { status, meta, presenceId = Guid.NewGuid().ToString() });
        }

        public async UniTask<MatchState> GetMatchStateAsync(string matchId)
        {
            RequireSession();
            return await apiClient.GetAsync<MatchState>($"/matchmaking/matches/{matchId}/state");
        }

        public async UniTask<PlayerResponse> GetPlayerAsync(string playerId)
        {
            RequireSession();
            return await apiClient.GetAsync<PlayerResponse>($"/players/{playerId}");
        }

        public async UniTask<PlayerResponse> GetPlayerByUsernameAsync(string username)
        {
            RequireSession();
            return await apiClient.GetAsync<PlayerResponse>($"/players/username/{username}");
        }

        public async UniTask<PlayerResponse> GetPlayerByCustomIdAsync(string customId)
        {
            RequireSession();
            return await apiClient.GetAsync<PlayerResponse>($"/players/custom/{customId}");
        }

        public async UniTask<PlayerResponse> GetPlayerByDeviceIdAsync(string deviceId)
        {
            RequireSession();
            return await apiClient.GetAsync<PlayerResponse>($"/players/device/{deviceId}");
        }

        public async UniTask<PlayerResponse> UpdatePlayerMetadataAsync(string playerId,
            Dictionary<string, object> metadata)
        {
            RequireSession();
            return await apiClient.PutAsync<PlayerResponse>($"/players/{playerId}/metadata", metadata);
        }

        public async UniTask<PlayerResponse> UpdatePlayerStatusAsync(string playerId, PlayerStatus status)
        {
            RequireSession();
            return await apiClient.PutAsync<PlayerResponse>($"/players/{playerId}/status",
                new { status = status.ToString().ToLower() });
        }

        public async UniTask<MatchmakingTicket> GetMatchmakingTicketAsync(string ticketId)
        {
            RequireSession();
            return await apiClient.GetAsync<MatchmakingTicket>($"/matchmaking/tickets/{ticketId}");
        }

        public async UniTask<List<Match>> ProcessMatchmakingAsync(string queueId = null)
        {
            RequireSession();
            var endpoint = queueId != null ? $"/matchmaking/process?queueId={queueId}" : "/matchmaking/process";
            return await apiClient.PostAsync<List<Match>>(endpoint, null);
        }

        public async UniTask<Match> EndMatchAsync(string matchId, Dictionary<string, object> finalState)
        {
            RequireSession();
            return await apiClient.PostAsync<Match>($"/matchmaking/matches/{matchId}/end", new { finalState });
        }

        public async UniTask<Match> LeaveMatchAsync(string matchId)
        {
            RequireSession();
            return await apiClient.PostAsync<Match>($"/matchmaking/matches/{matchId}/leave", null);
        }
#endif

        private void RunCoroutine(IEnumerator coroutine)
        {
            GameCloudCoroutineRunner.Instance.StartCoroutine(coroutine);
        }
    }
}