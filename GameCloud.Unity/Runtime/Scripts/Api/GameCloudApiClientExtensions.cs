#if UNITASK_SUPPORT
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCloud.Core.Exceptions;
using GameCloud.Models;
using UnityEngine.Networking;

namespace GameCloud.Api
{
    public static class GameCloudApiClientExtensions
    {
        public static void SetRequestHeaders(this UnityWebRequest www, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }
        }

        public static UniTask<T> GetAsync<T>(this GameCloudApiClient client, string endpoint)
        {
            return UniTask.Create<T>(async () =>
            {
                T result = default;
                ProblemDetails error = null;

                await client.Get<T>(endpoint,
                    r => result = r,
                    e => error = e).ToUniTask();

                if (error != null)
                    throw new GameCloudException(error);

                return result;
            });
        }

        public static UniTask<T> PostAsync<T>(this GameCloudApiClient client, string endpoint, object data)
        {
            return UniTask.Create<T>(async () =>
            {
                T result = default;
                ProblemDetails error = null;

                await client.Post<T>(endpoint, data,
                    r => result = r,
                    e => error = e).ToUniTask();

                if (error != null)
                    throw new GameCloudException(error);

                return result;
            });
        }

        public static UniTask<T> PutAsync<T>(this GameCloudApiClient client, string endpoint, object data)
        {
            return UniTask.Create<T>(async () =>
            {
                T result = default;
                ProblemDetails error = null;

                await client.Put<T>(endpoint, data,
                    r => result = r,
                    e => error = e).ToUniTask();

                if (error != null)
                    throw new GameCloudException(error);

                return result;
            });
        }

        public static UniTask DeleteAsync(this GameCloudApiClient client, string endpoint)
        {
            return UniTask.Create(async () =>
            {
                ProblemDetails error = null;

                await client.Delete(endpoint,
                    () => { },
                    e => error = e).ToUniTask();

                if (error != null)
                    throw new GameCloudException(error);
            });
        }
    }
}
#endif