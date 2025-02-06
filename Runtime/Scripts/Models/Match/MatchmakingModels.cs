using System;
using System.Collections.Generic;

namespace GameCloud.Models
{
    [Serializable]
    public class MatchmakingTicketRequest
    {
        public string queueName;
        public Dictionary<string, object> properties;
    }

    [Serializable]
    public class MatchmakingTicket
    {
        public string id;
        public string gameId;
        public string playerId;
        public string queueName;
        public string status;
        public Dictionary<string, object> properties;
        public string createdAt;
        public string expiresAt;
        public string matchId;
    }

    [Serializable]
    public class Match
    {
        public string id;
        public string gameId;
        public string queueName;
        public string state;
        public string[] playerIds;
        public string currentPlayerId;
        public Dictionary<string, object> playerStates;
        public Dictionary<string, object> matchState;
        public Dictionary<string, object> turnHistory;
        public string createdAt;
        public string startedAt;
        public string lastActionAt;
        public string nextActionDeadline;
        public string completedAt;
    }

    [Serializable]
    public class MatchAction
    {
        public string id;
        public string matchId;
        public string playerId;
        public string actionType;
        public Dictionary<string, object> actionData;
        public string timestamp;
    }

    [Serializable]
    public class MatchActionRequest
    {
        public string actionType;
        public Dictionary<string, object> actionData;
    }

    [Serializable]
    public class MatchPresence
    {
        public string playerId;
        public string sessionId;
        public string status;
        public Dictionary<string, object> meta;
        public string lastSeen;
        public string joinedAt;
        public string mode;
    }

    [Serializable]
    public class PresenceRequest
    {
        public string sessionId;
        public string status;
        public Dictionary<string, object> meta;
    }

    [Serializable]
    public class MatchState
    {
        public string status;
        public string phase;
        public string startedAt;
        public Dictionary<string, object> gameState;
        public Dictionary<string, object> metadata;
        public MatchPresence[] presences;
        public int size;
        public string label;
        public int tickRate;
    }
}