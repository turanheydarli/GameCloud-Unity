using System;

namespace GameCloud.Models
{
    [Serializable]
    public class MatchState
    {
        public string matchId;
        public string status;
        public MatchPlayer[] players;
        public object gameState; // You can create a specific type for your game state
        public string createdAt;
        public string updatedAt;
    }

    [Serializable]
    public class MatchPlayer
    {
        public string playerId;
        public string username;
        public string status;
        public string sessionId;
        public bool isReady;
        public object meta;
    }

    [Serializable]
    public class PresenceRequest
    {
        public string status;
        public string sessionId;
        public object meta;
    }

    [Serializable]
    public class GameAction
    {
        public string type;
        public string playerId;
        public object data; // You can create a specific type for your action data
        public string timestamp;
    }
} 