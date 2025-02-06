using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        private readonly string baseUrl;
        private readonly string gameKey;
        private string authToken;

        public GameCloudApiClient(string baseUrl, string gameKey)
        {
            this.baseUrl = baseUrl;
            this.gameKey = gameKey;
        }

        public void SetAuthToken(string token)
        {
            this.authToken = token;
        }

        protected IEnumerator Get<T>(string endpoint, System.Action<T> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}/api/v1{endpoint}"))
            {
                SetupHeaders(www);
                yield return www.SendWebRequest();
                HandleResponse(www, onSuccess, onError);
            }
        }

        protected IEnumerator Post<T>(string endpoint, object data, System.Action<T> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest www = new UnityWebRequest($"{baseUrl}/api/v1{endpoint}", "POST"))
            {
                if (data != null)
                {
                    string jsonData = JsonUtility.ToJson(data);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    www.downloadHandler = new DownloadHandlerBuffer();
                }

                SetupHeaders(www);
                yield return www.SendWebRequest();
                HandleResponse(www, onSuccess, onError);
            }
        }

        protected IEnumerator Put<T>(string endpoint, object data, System.Action<T> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest www = new UnityWebRequest($"{baseUrl}/api/v1{endpoint}", "PUT"))
            {
                if (data != null)
                {
                    string jsonData = JsonUtility.ToJson(data);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    www.downloadHandler = new DownloadHandlerBuffer();
                }

                SetupHeaders(www);
                yield return www.SendWebRequest();
                HandleResponse(www, onSuccess, onError);
            }
        }

        protected IEnumerator Delete(string endpoint, System.Action onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest www = UnityWebRequest.Delete($"{baseUrl}/api/v1{endpoint}"))
            {
                SetupHeaders(www);
                yield return www.SendWebRequest();
                //HandleResponse(www, onSuccess, onError);
            }
        }

        private void SetupHeaders(UnityWebRequest www)
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("X-Game-Key", gameKey);
            if (!string.IsNullOrEmpty(authToken))
            {
                www.SetRequestHeader("Authorization", $"Bearer {authToken}");
            }
        }

        private void HandleResponse<T>(UnityWebRequest www, System.Action<T> onSuccess, System.Action<string> onError)
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(www.error);
                return;
            }

            try
            {
                if (typeof(T) == typeof(string))
                {
                    onSuccess?.Invoke((T)(object)www.downloadHandler.text);
                }
                else
                {
                    T response = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    onSuccess?.Invoke(response);
                }
            }
            catch (System.Exception e)
            {
                onError?.Invoke($"Failed to parse response: {e.Message}");
            }
        }
    }
} 