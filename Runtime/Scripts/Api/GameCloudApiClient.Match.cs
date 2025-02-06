namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator SetMatchPresence(string matchId, System.Action onSuccess, System.Action<string> onError)
        {
            var request = new PresenceRequest
            {
                status = "connected",
                sessionId = System.Guid.NewGuid().ToString(),
                meta = new object()
            };

            return Post($"/matchmaking/matches/{matchId}/presence", request, (_) => onSuccess?.Invoke(), onError);
        }

        public IEnumerator MarkReady(string matchId, System.Action onSuccess, System.Action<string> onError)
        {
            return Post($"/matchmaking/matches/{matchId}/ready", null, (_) => onSuccess?.Invoke(), onError);
        }

        public IEnumerator SubmitMove(string matchId, GameAction action, System.Action onSuccess, System.Action<string> onError)
        {
            return Post($"/matchmaking/matches/{matchId}/actions", action, (_) => onSuccess?.Invoke(), onError);
        }

        public IEnumerator GetMatchState(string matchId, System.Action<MatchState> onSuccess, System.Action<string> onError)
        {
            return Get<MatchState>($"/matchmaking/matches/{matchId}/state", onSuccess, onError);
        }
    }
} 