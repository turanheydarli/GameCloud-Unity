using System;
using System.Collections.Generic;

namespace GameCloud.Models
{
    public enum PlayerStatus
    {
        Online,
        Offline,
        InGame
    }

    [Serializable]
    public class PlayerResponse
    {
        public string id;
        public string username;
        public string displayName;
        public string? deviceId;
        public string? customId;
        public PlayerStatus status;
        public Guid gameId;
        public Dictionary<string, object> metadata;
        public DateTime createdAt;
        public DateTime? lastLoginAt;
    }

    [Serializable]
    public class AuthResponse
    {
        public string token;
        public string refreshToken;
        public PlayerResponse player;
        public Dictionary<string, object> vars;
        public string sessionId;
        public string issuedAt;
        public string expiresAt;
    }

    [Serializable]
    public class DeviceAuthRequest
    {
        public string deviceId;
        public Dictionary<string, object> metadata;
    }

    [Serializable]
    public class CustomAuthRequest
    {
        public string customId;
        public Dictionary<string, object> metadata;
        public bool create;
    }

    [Serializable]
    public class SessionRefreshRequest
    {
        public string sessionId;
    }
}