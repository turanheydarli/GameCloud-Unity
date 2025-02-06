namespace GameCloud
{
    public interface IGameCloudSession
    {
        string AuthToken { get; }
        string RefreshToken { get; }
        bool IsValid { get; }
        string UserId { get; }
        string Username { get; }
        bool HasExpired { get; }
        
        void Refresh(GameCloudClient client, System.Action onSuccess, System.Action<GameCloudError> onError);
    }
} 