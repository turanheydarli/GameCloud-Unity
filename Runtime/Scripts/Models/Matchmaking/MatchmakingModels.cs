using System;
using System.Collections.Generic;

namespace GameCloud.Models
{
    [Serializable]
    public class MatchmakingTicket
    {
        public string id;
        public string gameId;
        public string playerId;
        public string queueName;
        public MatchmakingTicketStatus status;
        public MatchmakingProperties properties;
        public string createdAt;
        public string expiresAt;
        public string matchId;
    }

    [Serializable]
    public class MatchmakingTicketRequest
    {
        public string queueName;
        public MatchmakingProperties properties;
    }

    [Serializable]
    public class MatchmakingProperties
    {
        public int skill;
        public int level;
        public string[] preferredColors;
        public Dictionary<string, object> customProperties;
    }

    [Serializable]
    public class MatchmakingQueue
    {
        public string id;
        public string name;
        public string description;
        public int minPlayers;
        public int maxPlayers;
        public string ticketTTL;
        public string matchmakerFunctionName;
        public MatchmakingRules rules;
        public string createdAt;
        public string updatedAt;
    }

    [Serializable]
    public class MatchmakingRules
    {
        public string gameMode;
        public int startLevel;
        public int maxSkillDifference;
        public int maxWaitTime;
        public Dictionary<string, object> customRules;
    }

    public enum MatchmakingTicketStatus
    {
        Created,
        Searching,
        MatchFound,
        Cancelled,
        Error,
        Expired
    }

    [Serializable]
    public class MatchmakingResponse
    {
        public string ticketId;
        public MatchmakingTicketStatus status;
        public string matchId;
        public string estimatedWaitTime;
        public MatchmakingError error;
    }

    [Serializable]
    public class MatchmakingError
    {
        public string code;
        public string message;
        public Dictionary<string, object> details;
    }

    [Serializable]
    public class MatchmakingStats
    {
        public int activeTickets;
        public int matchesInProgress;
        public int averageWaitTime;
        public int playersOnline;
        public Dictionary<string, QueueStats> queueStats;
    }

    [Serializable]
    public class QueueStats
    {
        public string queueName;
        public int activeTickets;
        public int averageWaitTime;
        public int matchesCreated;
    }

    [Serializable]
    public class MatchmakingFilter
    {
        public string queueName;
        public MatchmakingTicketStatus? status;
        public DateTime? createdAfter;
        public DateTime? createdBefore;
        public int? limit;
        public string cursor;
    }

    [Serializable]
    public class MatchmakingTicketList
    {
        public List<MatchmakingTicket> tickets;
        public string cursor;
        public bool hasMore;
    }
} 