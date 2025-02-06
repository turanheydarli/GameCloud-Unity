using System.Collections;
using GameCloud.Models;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator CreateMatchmakingTicket(System.Action<MatchmakingTicket> onSuccess, System.Action<string> onError)
        {
            var request = new MatchmakingTicketRequest
            {
                queueName = "tetris_vs",
                properties = new MatchmakingProperties
                {
                    skill = 1500,
                    level = 25,
                    preferredColors = new string[] { "blue", "red" }
                }
            };

            return Post("/matchmaking/tickets", request, onSuccess, onError);
        }

        public IEnumerator GetTicketStatus(string ticketId, System.Action<MatchmakingTicket> onSuccess, System.Action<string> onError)
        {
            return Get<MatchmakingTicket>($"/matchmaking/tickets/{ticketId}", onSuccess, onError);
        }

        public IEnumerator ProcessMatchmaking(System.Action onSuccess, System.Action<string> onError)
        {
            return Post<object>("/matchmaking/process", null, (_) => onSuccess?.Invoke(), onError);
        }
    }
} 