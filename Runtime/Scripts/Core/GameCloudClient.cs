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

        // Authentication
        public void AuthenticateCustom(string username, System.Action<IGameCloudSession> onSuccess, System.Action<GameCloudError> onError)
        {
            apiClient.AuthenticateCustom(username, (response) => {
                var session = new GameCloudSession(response);
                SetSession(session);
                onSuccess?.Invoke(session);
            }, onError);
        }

        // Matchmaking
        public void CreateMatchmakingTicket(MatchmakingProperties properties, System.Action<MatchmakingTicket> onSuccess, System.Action<GameCloudError> onError)
        {
            RequireSession();
            apiClient.CreateMatchmakingTicket(properties, onSuccess, onError);
        }

        private void RequireSession()
        {
            if (session == null || !session.IsValid)
            {
                throw new GameCloudException("Active session required");
            }
        }
    }
} 