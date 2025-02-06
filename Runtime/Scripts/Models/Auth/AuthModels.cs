using System;
using System.Collections.Generic;

namespace GameCloud.Models
{
    [Serializable]
    public class AuthResponse
    {
        public string token;
        public string refreshToken;
        public string userId;
        public string username;
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