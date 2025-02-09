using System;
using System.Collections;
using System.Collections.Generic;
using GameCloud.Models;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator CreateMatchmakingTicket(MatchmakingTicketRequest request, Action<MatchmakingTicket> onSuccess, Action<ProblemDetails> onError)
        {
            return Post("/matchmaking/tickets", request, onSuccess, onError);
        }

        public IEnumerator GetMatch(string matchId, Action<Match> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<Match>($"/matchmaking/matches/{matchId}", onSuccess, onError);
        }

        public IEnumerator SubmitMatchAction(string matchId, MatchActionRequest action, Action<MatchAction> onSuccess,
            Action<ProblemDetails> onError)
        {
            return Post($"/matchmaking/matches/{matchId}/actions", action, onSuccess, onError);
        }

        public IEnumerator SetMatchPresence(string matchId, string status, string sessionId, Dictionary<string, object> meta, Action<Match> onSuccess, Action<ProblemDetails> onError)
        {
            var request = new PresenceRequest
            {
                status = status,
                sessionId = sessionId ?? Guid.NewGuid().ToString(),
                meta = meta
            };

            return Post($"/matchmaking/matches/{matchId}/presence", request, onSuccess, onError);
        }

        public IEnumerator GetMatchState(string matchId, Action<MatchState> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<MatchState>($"/matchmaking/matches/{matchId}/state", onSuccess, onError);
        }
    }
}