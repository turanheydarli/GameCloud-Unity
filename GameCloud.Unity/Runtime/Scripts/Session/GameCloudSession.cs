using System;
using System.Collections.Generic;
using GameCloud.Models;

namespace GameCloud
{
    public class GameCloudSession : IGameCloudSession
    {
        private AuthResponse auth;
        private readonly long createdAt;
        private readonly long expiresAt;

        public string AuthToken => auth.token;
        public string RefreshToken => auth.refreshToken;
        public string UserId => auth.player.id;
        public string Username => auth.player.username;
        public Dictionary<string, object> Vars => auth.vars;
        public string SessionId => auth.sessionId;
        
        public bool IsValid => !HasExpired;
        public PlayerResponse Player => auth.player;
        public bool HasExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiresAt;

        public GameCloudSession(AuthResponse auth)
        {
            this.auth = auth;
            this.createdAt = DateTimeOffset.Parse(auth.issuedAt).ToUnixTimeSeconds();
            this.expiresAt = DateTimeOffset.Parse(auth.expiresAt).ToUnixTimeSeconds();
        }

        public void Refresh(GameCloudClient client, Action onSuccess, Action<ProblemDetails> onError)
        {
            if (!HasExpired)
            {
                onSuccess?.Invoke();
                return;
            }

            client.RefreshSession(SessionId, newSession => {
                this.auth = ((GameCloudSession)newSession).auth;
                onSuccess?.Invoke();
            }, onError);
        }

        public void Update(AuthResponse newAuth)
        {
            this.auth = newAuth;
        }
    }
}