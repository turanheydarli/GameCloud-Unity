using System.Collections.Generic;
using UnityEngine;
using GameCloud.Api;
using GameCloud.Models;

namespace GameCloud
{
    public class GameCloudClient
    {
        public static string DefaultHost = "cloud.playables.studio";
        public static int DefaultPort = 80;


        private readonly GameCloudApiClient apiClient;
        private IGameCloudSession session;

        public IGameCloudSession Session => session;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool SSL { get; private set; }

        private GameCloudClient(string host, int port, string gameKey, bool ssl)
        {
            Host = host;
            Port = port;
            SSL = ssl;
            apiClient = new GameCloudApiClient(host, port, gameKey, ssl);
        }

        public static GameCloudClient Default(string gameKey)
        {
            return new GameCloudClient(DefaultHost, DefaultPort, gameKey, true);
        }

        public static GameCloudClient Create(string host, int port, string gameKey, bool ssl = true)
        {
            return new GameCloudClient(host, port, gameKey, ssl);
        }

        public void SetSession(IGameCloudSession session)
        {
            this.session = session;
            if (session != null)
            {
                apiClient.SetAuthToken(session.AuthToken);
            }
        }

        public void AuthenticateCustom(string username, System.Action<IGameCloudSession> onSuccess,
            System.Action<GameCloudError> onError)
        {
            apiClient.AuthenticateCustom(username, null, true, (response) =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void AuthenticateWithDevice(string deviceId, Dictionary<string, object> metadata = null,
            System.Action<IGameCloudSession> onSuccess = null, System.Action<GameCloudError> onError = null)
        {
            apiClient.AuthenticateDevice(deviceId, metadata, (response) =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void AuthenticateWithCustomId(string customId, Dictionary<string, object> metadata = null,
            bool create = true, System.Action<IGameCloudSession> onSuccess = null,
            System.Action<GameCloudError> onError = null)
        {
            apiClient.AuthenticateCustom(customId, metadata, create, (response) =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void RefreshSession(string sessionId, System.Action<IGameCloudSession> onSuccess = null,
            System.Action<GameCloudError> onError = null)
        {
            apiClient.RefreshSession(sessionId, (response) =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void CreateMatchmakingTicket(string queueName, Dictionary<string, object> properties,
            System.Action<MatchmakingTicket> onSuccess, System.Action<GameCloudError> onError)
        {
            RequireSession();
            var request = new MatchmakingTicketRequest
            {
                queueName = queueName,
                properties = properties
            };
            apiClient.CreateMatchmakingTicket(request, onSuccess,
                (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void GetMatch(string matchId, System.Action<Match> onSuccess, System.Action<GameCloudError> onError)
        {
            RequireSession();
            apiClient.GetMatch(matchId, onSuccess, (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void SubmitMatchAction(string matchId, string actionType, Dictionary<string, object> actionData,
            System.Action<MatchAction> onSuccess, System.Action<GameCloudError> onError)
        {
            RequireSession();
            var request = new MatchActionRequest
            {
                actionType = actionType,
                actionData = actionData
            };
            apiClient.SubmitMatchAction(matchId, request, onSuccess,
                (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void SetMatchPresence(string matchId, string status, Dictionary<string, object> meta = null,
            System.Action<Match> onSuccess = null, System.Action<GameCloudError> onError = null)
        {
            RequireSession();
            apiClient.SetMatchPresence(matchId, status, System.Guid.NewGuid().ToString(), meta, onSuccess,
                (error) => onError?.Invoke(new GameCloudError { message = error }));
        }

        public void GetMatchState(string matchId, System.Action<MatchState> onSuccess,
            System.Action<GameCloudError> onError)
        {
            RequireSession();
            apiClient.GetMatchState(matchId, onSuccess,
                (error) => onError?.Invoke(new GameCloudError { message = error }));
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
                    error => throw new GameCloudException($"Session refresh failed: {error.message}"));
            }
        }

        public void RestoreSession(string authToken, string refreshToken, System.Action<IGameCloudSession> onSuccess,
            System.Action<GameCloudError> onError)
        {
            apiClient.SetAuthToken(authToken);
            apiClient.RefreshSession(refreshToken, response =>
            {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, error => onError?.Invoke(new GameCloudError { message = error }));
        }
    }
}