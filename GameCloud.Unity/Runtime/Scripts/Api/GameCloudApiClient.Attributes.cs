using System;
using System.Collections;
using GameCloud.Models;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        public IEnumerator GetAttributes(string username, string collection, Action<AttributeCollection> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<AttributeCollection>($"/players/{username}/attributes/{collection}", onSuccess, onError);
        }

        public IEnumerator GetAttribute(string username, string collection, string key,
            Action<PlayerAttribute> onSuccess, Action<ProblemDetails> onError)
        {
            return Get<PlayerAttribute>($"/players/{username}/attributes/{collection}/{key}", onSuccess, onError);
        }

        public IEnumerator SetAttribute(string username, string collection, AttributeWriteRequest request,
            Action<object> onSuccess, Action<ProblemDetails> onError)
        {
            return Put($"/players/{username}/attributes/{collection}", request, onSuccess, onError);
        }

        public IEnumerator DeleteAttribute(string username, string collection, string key, Action onSuccess,
            Action<ProblemDetails> onError)
        {
            return Delete($"/players/{username}/attributes/{collection}/{key}", onSuccess, onError);
        }
    }
}