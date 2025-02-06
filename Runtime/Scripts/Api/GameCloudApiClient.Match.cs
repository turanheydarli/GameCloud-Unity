using System.Collections;
using System.Collections.Generic;
using GameCloud.Models;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator CreateMatchmakingTicket(MatchmakingTicketRequest request, System.Action<MatchmakingTicket> onSuccess, System.Action<string> onError)
        {
            return Post("/matchmaking/tickets", request, onSuccess, onError);
        }

        public IEnumerator GetMatch(string matchId, System.Action<Match> onSuccess, System.Action<string> onError)
        {
            return Get<Match>($"/matchmaking/matches/{matchId}", onSuccess, onError);
        }

        public IEnumerator SubmitMatchAction(string matchId, MatchActionRequest action, System.Action<MatchAction> onSuccess, System.Action<string> onError)
        {
            return Post($"/matchmaking/matches/{matchId}/actions", action, onSuccess, onError);
        }

        public IEnumerator SetMatchPresence(string matchId, string status, string sessionId, Dictionary<string, object> meta, System.Action<Match> onSuccess, System.Action<string> onError)
        {
            var request = new PresenceRequest
            {
                status = status,
                sessionId = sessionId ?? System.Guid.NewGuid().ToString(),
                meta = meta
            };

            return Post($"/matchmaking/matches/{matchId}/presence", request, onSuccess, onError);
        }

        public IEnumerator GetMatchState(string matchId, System.Action<MatchState> onSuccess, System.Action<string> onError)
        {
            return Get<MatchState>($"/matchmaking/matches/{matchId}/state", onSuccess, onError);
        }
    }
}