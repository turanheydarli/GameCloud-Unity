namespace GameCloud
{
    public class GameCloudSession : IGameCloudSession
    {
        private AuthResponse auth;
        private readonly long createdAt;
        private readonly long expiresAt;

        public string AuthToken => auth.token;
        public string RefreshToken => auth.refreshToken;
        public string UserId => auth.userId;
        public string Username => auth.username;
        
        public bool IsValid => !HasExpired;
        public bool HasExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiresAt;

        public GameCloudSession(AuthResponse auth)
        {
            this.auth = auth;
            this.createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expiresAt = createdAt + 3600; 
        }

        public void Refresh(GameCloudClient client, System.Action onSuccess, System.Action<GameCloudError> onError)
        {
        }
    }
} 