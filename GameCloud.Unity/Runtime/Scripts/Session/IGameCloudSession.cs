using System.Collections.Generic;
using GameCloud.Models;

namespace GameCloud
{
    public interface IGameCloudSession
    {
        string AuthToken { get; }
        string RefreshToken { get; }
        string SessionId { get; }
        bool IsValid { get; }
        PlayerResponse Player { get; }
        bool HasExpired { get; }
        Dictionary<string, object> Vars { get; }
        
        void Refresh(GameCloudClient client, System.Action onSuccess, System.Action<ProblemDetails> onError);
    }
}