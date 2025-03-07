using System;
using System.Collections.Generic;
using GameCloud.Proto;
using GameCloud.Api;
using NativeWebSocket;

namespace GameCloud.Core
{
    public class GameCloudClient
    {
        public static string DefaultHost = "localhost";
        public static int DefaultPort = 5005;

        private readonly GameCloudApiClient _apiClient;
        private string _session;

        // public bool IsConnected => _apiClient.IsConnected;
        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool SSL { get; private set; }

        // Events from the API client
        public event Action OnConnected
        {
            add { _apiClient.OnConnected += value; }
            remove { _apiClient.OnConnected -= value; }
        }

        public event Action<WebSocketCloseCode> OnDisconnected
        {
            add { _apiClient.OnDisconnected += value; }
            remove { _apiClient.OnDisconnected -= value; }
        }

        public event Action<Error> OnError
        {
            add { _apiClient.OnError += value; }
            remove { _apiClient.OnError -= value; }
        }

        public event Action<RoomPresence> OnRoomPresence
        {
            add { _apiClient.OnRoomPresence += value; }
            remove { _apiClient.OnRoomPresence -= value; }
        }

        public event Action<RoomData> OnRoomData
        {
            add { _apiClient.OnRoomData += value; }
            remove { _apiClient.OnRoomData -= value; }
        }

        public event Action<GameState> OnGameState
        {
            add { _apiClient.OnGameState += value; }
            remove { _apiClient.OnGameState -= value; }
        }

        public event Action<GameAction> OnGameAction
        {
            add { _apiClient.OnGameAction += value; }
            remove { _apiClient.OnGameAction -= value; }
        }

        public event Action<TurnChange> OnTurnChange
        {
            add { _apiClient.OnTurnChange += value; }
            remove { _apiClient.OnTurnChange -= value; }
        }

        public event Action<GameEnd> OnGameEnd
        {
            add { _apiClient.OnGameEnd += value; }
            remove { _apiClient.OnGameEnd -= value; }
        }

        public event Action<MatchmakerMatched> OnMatchmakerMatched
        {
            add { _apiClient.OnMatchmakerMatched += value; }
            remove { _apiClient.OnMatchmakerMatched -= value; }
        }

        public event Action<StatusPresence> OnStatusPresence
        {
            add { _apiClient.OnStatusPresence += value; }
            remove { _apiClient.OnStatusPresence -= value; }
        }

        private GameCloudClient(string host, int port, bool ssl)
        {
            Host = host;
            Port = port;
            SSL = ssl;
            _apiClient = new GameCloudApiClient(host, port, ssl);
        }

        public static GameCloudClient Create(string host, int port, bool ssl = true)
        {
            return new GameCloudClient(host, port, ssl);
        }

        public static GameCloudClient Default()
        {
            return new GameCloudClient(DefaultHost, DefaultPort, false);
        }

        public void Connect(Action onSuccess = null, Action<Error> onError = null)
        {
            _apiClient.Connect("13c07822",onSuccess, onError);
        }

        public void Disconnect()
        {
            _apiClient.Disconnect();
        }

        public void Authenticate(string token, string deviceId, Action<SessionConnect> onSuccess = null,
            Action<Error> onError = null)
        {
            _apiClient.Authenticate(token, deviceId, onSuccess, onError);
        }

        public void CreateRoom(string gameId, int maxPlayers, Dictionary<string, string> metadata = null,
            Action<Room> onSuccess = null, Action<Error> onError = null)
        {
        }

        public void JoinRoom(string roomId, bool asSpectator = false, Action<JoinRoomResponse> onSuccess = null,
            Action<Error> onError = null)
        {
        }

        public void LeaveRoom(string roomId, Action onSuccess = null, Action<Error> onError = null)
        {
        }

        public void SendGameAction(string roomId, string actionType, byte[] payload, int sequenceNumber,
            Action<GameActionAck> onSuccess = null, Action<Error> onError = null)
        {
        }

        public void AddMatchmaker(string gameId, int minCount, int maxCount, string query = null,
            Dictionary<string, double> numericProperties = null, Dictionary<string, string> stringProperties = null,
            Action<string> onSuccess = null, Action<Error> onError = null)
        {
        }

        public void RemoveMatchmaker(string ticket, Action onSuccess = null, Action<Error> onError = null)
        {
        }

        public void UpdateStatus(string status, Dictionary<string, string> metadata = null, Action onSuccess = null,
            Action<Error> onError = null)
        {
            UpdateStatus(status, metadata, onSuccess, onError);
        }
    }
}