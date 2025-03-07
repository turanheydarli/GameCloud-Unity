using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCloud.Proto;
using UnityEngine;
using Google.Protobuf;
using NativeWebSocket;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        private WebSocket _socket;
        
        private string _baseUrl;
        private bool _connected;
        private Dictionary<string, Action<Envelope>> _callbacks = new Dictionary<string, Action<Envelope>>();
        private Coroutine _heartbeatCoroutine;
        
        public event Action OnConnected;
        public event Action<WebSocketCloseCode> OnDisconnected;
        
        public event Action<Error> OnError;
        
        public event Action<RoomPresence> OnRoomPresence;
        public event Action<RoomData> OnRoomData;
        public event Action<GameState> OnGameState;
        public event Action<GameAction> OnGameAction;
        public event Action<TurnChange> OnTurnChange;
        public event Action<GameEnd> OnGameEnd;
        
        public event Action<MatchmakerMatched> OnMatchmakerMatched;
        
        public event Action<StatusPresence> OnStatusPresence;
        
        public GameCloudApiClient(string host, int port, bool useSSL)
        {
            string protocol = useSSL ? "wss" : "ws";
            _baseUrl = $"{protocol}://{host}:{port}/ws";
            
            GameCloudCoroutineRunner.Instance.RegisterClient(this);
        }
        
        public bool IsConnected => _connected && _socket != null && _socket.State == WebSocketState.Open;
        
        public async void Connect(string gameKey, Action onSuccess = null, Action<Error> onError = null)
        {
            if (IsConnected)
            {
                onSuccess?.Invoke();
                return;
            }
            
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "X-Game-Key", gameKey }
                };
                
                _socket = new WebSocket(_baseUrl, headers);
                
                // Set up event handlers
                _socket.OnOpen += () => 
                {
                    _connected = true;
                    OnConnected?.Invoke();
                    onSuccess?.Invoke();
                    StartHeartbeat();
                };
                
                _socket.OnClose += (closeCode) => 
                {
                    _connected = false;
                    StopHeartbeat();
                    OnDisconnected?.Invoke(closeCode);
                };
                
                _socket.OnError += (errorMsg) => 
                {
                    Debug.LogError($"WebSocket error: {errorMsg}");
                    var error = new Error { Code = "ConnectionError", Message = errorMsg };
                    OnError?.Invoke(error);
                    onError?.Invoke(error);
                };
                
                _socket.OnMessage += (bytes) => 
                {
                    try
                    {
                        var envelope = Envelope.Parser.ParseFrom(bytes);
                        
                        // Check if this is a response to a pending request
                        if (!string.IsNullOrEmpty(envelope.Id) && _callbacks.TryGetValue(envelope.Id, out var callback))
                        {
                            callback?.Invoke(envelope);
                            _callbacks.Remove(envelope.Id);
                            return;
                        }
                        
                        // Process incoming message based on type
                        ProcessMessage(envelope);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error processing message: {ex.Message}");
                    }
                };
                
                // Connect to the server
                await _socket.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect: {ex.Message}");
                var error = new Error { Code = "ConnectionError", Message = ex.Message };
                onError?.Invoke(error);
            }
        }
        
        public async void Disconnect()
        {
            if (_socket != null)
            {
                try
                {
                    StopHeartbeat();
                    await _socket.Close();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error during disconnect: {ex.Message}");
                }
                finally
                {
                    _socket = null;
                    _connected = false;
                }
            }
        }
        
        private void ProcessMessage(Envelope envelope)
        {
            switch (envelope.MessageCase)
            {
                case Envelope.MessageOneofCase.Error:
                    OnError?.Invoke(envelope.Error);
                    break;
                    
                case Envelope.MessageOneofCase.RoomPresence:
                    OnRoomPresence?.Invoke(envelope.RoomPresence);
                    break;
                    
                case Envelope.MessageOneofCase.RoomData:
                    OnRoomData?.Invoke(envelope.RoomData);
                    break;
                    
                case Envelope.MessageOneofCase.GameState:
                    OnGameState?.Invoke(envelope.GameState);
                    break;
                    
                case Envelope.MessageOneofCase.GameAction:
                    OnGameAction?.Invoke(envelope.GameAction);
                    break;
                    
                case Envelope.MessageOneofCase.TurnChange:
                    OnTurnChange?.Invoke(envelope.TurnChange);
                    break;
                    
                case Envelope.MessageOneofCase.GameEnd:
                    OnGameEnd?.Invoke(envelope.GameEnd);
                    break;
                    
                case Envelope.MessageOneofCase.MatchmakerMatched:
                    OnMatchmakerMatched?.Invoke(envelope.MatchmakerMatched);
                    break;
                    
                case Envelope.MessageOneofCase.StatusPresence:
                    OnStatusPresence?.Invoke(envelope.StatusPresence);
                    break;
            }
        }
        
        protected async void Send<T>(T message, Action<Envelope> callback = null) where T : IMessage
        {
            if (!IsConnected)
            {
                var error = new Error { Code = "NotConnected", Message = "WebSocket is not connected" };
                OnError?.Invoke(error);
                return;
            }
            
            try
            {
                var id = Guid.NewGuid().ToString();
                var envelope = new Envelope { Id = id };
                
                if (message is SessionConnect connectMsg)
                    envelope.Connect = connectMsg;
                else if (message is SessionDisconnect disconnectMsg)
                    envelope.Disconnect = disconnectMsg;
                else if (message is SessionHeartbeat heartbeatMsg)
                    envelope.Heartbeat = heartbeatMsg;
                else if (message is RoomCreate roomCreateMsg)
                    envelope.RoomCreate = roomCreateMsg;
                else if (message is RoomJoin roomJoinMsg)
                    envelope.RoomJoin = roomJoinMsg;
                else if (message is RoomLeave roomLeaveMsg)
                    envelope.RoomLeave = roomLeaveMsg;
                else if (message is RoomData roomDataMsg)
                    envelope.RoomData = roomDataMsg;
                else if (message is MatchmakerAdd matchmakerAddMsg)
                    envelope.MatchmakerAdd = matchmakerAddMsg;
                else if (message is MatchmakerRemove matchmakerRemoveMsg)
                    envelope.MatchmakerRemove = matchmakerRemoveMsg;
                else if (message is StatusUpdate statusUpdateMsg)
                    envelope.StatusUpdate = statusUpdateMsg;
                else if (message is GameAction gameActionMsg)
                    envelope.GameAction = gameActionMsg;
                else
                    throw new ArgumentException($"Unsupported message type: {message.GetType().Name}");
                
                if (callback != null)
                {
                    _callbacks[id] = callback;
                }
                
                byte[] data = envelope.ToByteArray();
                await _socket.Send(data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending message: {ex.Message}");
                var error = new Error { Code = "SendError", Message = ex.Message };
                OnError?.Invoke(error);
            }
        }
        
        private void StartHeartbeat()
        {
            StopHeartbeat();
            _heartbeatCoroutine = GameCloudCoroutineRunner.Instance.StartCoroutine(HeartbeatCoroutine());
        }
        
        private void StopHeartbeat()
        {
            if (_heartbeatCoroutine != null)
            {
                GameCloudCoroutineRunner.Instance.StopCoroutine(_heartbeatCoroutine);
                _heartbeatCoroutine = null;
            }
        }
        
        private IEnumerator HeartbeatCoroutine()
        {
            while (IsConnected)
            {
                yield return new WaitForSeconds(15f);
                
                try
                {
                    var heartbeat = new SessionHeartbeat
                    {
                        Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
                    };
                    
                    Send(heartbeat);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to send heartbeat: {ex.Message}");
                }
            }
        }
        
        public void Update()
        {
            if (_socket != null)
            {
                #if !UNITY_WEBGL || UNITY_EDITOR
                _socket.DispatchMessageQueue();
                #endif
            }
        }
    }
}