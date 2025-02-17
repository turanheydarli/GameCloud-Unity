using System;
using System.Collections;
using System.Collections.Generic;
using GameCloud.Models;
using JetBrains.Annotations;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator CreateMatchmakingTicket(MatchmakingTicketRequest request,
            Action<MatchmakingTicket> onSuccess, Action<ProblemDetails> onError)
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

        public IEnumerator SetMatchPresence(string matchId, string status, string sessionId,
            Dictionary<string, object> meta, Action<Match> onSuccess, Action<ProblemDetails> onError)
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

        public IEnumerator ProcessMatchmaking([CanBeNull] string queueId, Action<List<Match>> onSuccess,
            Action<ProblemDetails> onError)
        {
            var endpoint = !string.IsNullOrEmpty(queueId)
                ? $"/matchmaking/process?queueId={queueId}"
                : "/matchmaking/process";

            return Post<List<Match>>($"/matchmaking/process?queueId={queueId}", null, onSuccess, onError);
        }

        public IEnumerator GetMatchmakingTicket(string ticketId, Action<MatchmakingTicket> onSuccess,
            Action<ProblemDetails> onError)
        {
            return Get<MatchmakingTicket>($"/matchmaking/tickets/{ticketId}", onSuccess, onError);
        }

        public IEnumerator EndMatch(string matchId, Dictionary<string, object> finalState, Action<Match> onSuccess,
            Action<ProblemDetails> onError)
        {
            return Post($"/matchmaking/matches/{matchId}/end", new { finalState }, onSuccess, onError);
        }

        public IEnumerator LeaveMatch(string matchId, Action<Match> onSuccess, Action<ProblemDetails> onError)
        {
            return Post($"/matchmaking/matches/{matchId}/leave", null, onSuccess, onError);
        }
    }
}