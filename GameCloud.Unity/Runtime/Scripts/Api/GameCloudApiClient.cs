using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using GameCloud.Models;
using Newtonsoft.Json;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        private readonly string baseUrl;
        private readonly string gameKey;
        private string authToken;
        private readonly bool useLogger;
        private readonly ILogger logger;
    
        public GameCloudApiClient(string host, int port, string gameKey, bool ssl, bool useLogger = false, ILogger logger = null)
        {
            this.baseUrl = $"{(ssl ? "https" : "http")}://{host}:{port}";
            this.gameKey = gameKey;
            this.useLogger = useLogger;
            this.logger = logger;
        }

        public void SetAuthToken(string token)
        {
            this.authToken = token;
        }

        protected internal IEnumerator Get<T>(string endpoint, Action<T> onSuccess, Action<ProblemDetails> onError)
        {
            LogRequest("GET", endpoint);
            
            using UnityWebRequest www = UnityWebRequest.Get($"{baseUrl}/api/v1{endpoint}");
            SetupHeaders(www);
            yield return www.SendWebRequest();
            
            LogResponse("GET", endpoint, www.downloadHandler.text, www.result != UnityWebRequest.Result.Success);
            HandleResponse(www, onSuccess, onError);
        }

        protected internal IEnumerator Post<T>(string endpoint, object data, Action<T> onSuccess, Action<ProblemDetails> onError)
        {
            LogRequest("POST", endpoint, data);
            
            using UnityWebRequest www = new UnityWebRequest($"{baseUrl}/api/v1{endpoint}", "POST");
            www.downloadHandler = new DownloadHandlerBuffer();
            
            if (data != null)
            {
                string jsonData = JsonConvert.SerializeObject(data);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            SetupHeaders(www);
            yield return www.SendWebRequest();
            
            LogResponse("POST", endpoint, www.downloadHandler.text, www.result != UnityWebRequest.Result.Success);
            HandleResponse(www, onSuccess, onError);
        }

        protected internal IEnumerator Put<T>(string endpoint, object data, Action<T> onSuccess,
            Action<ProblemDetails> onError)
        {
            using (UnityWebRequest www = new UnityWebRequest($"{baseUrl}/api/v1{endpoint}", "PUT"))
            {
                if (data != null)
                {
                    string jsonData = JsonConvert.SerializeObject(data);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    www.downloadHandler = new DownloadHandlerBuffer();
                }

                SetupHeaders(www);
                yield return www.SendWebRequest();
                HandleResponse(www, onSuccess, onError);
            }
        }

        private void HandleResponse<T>(UnityWebRequest www, Action<T> onSuccess, Action<ProblemDetails> onError)
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                try
                {
                    var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(www.downloadHandler.text);
                    onError?.Invoke(problemDetails);
                }
                catch
                {
                    onError?.Invoke(ProblemDetails.FromError(www.error));
                }
                
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
                    T response = JsonConvert.DeserializeObject<T>(www.downloadHandler.text);
                    onSuccess?.Invoke(response);
                }
            }
            catch (Exception e)
            {
                onError?.Invoke(ProblemDetails.FromError($"Failed to parse response: {e.Message}"));
            }
        }

        protected internal IEnumerator Delete(string endpoint, Action onSuccess, Action<ProblemDetails> onError)
        {
            using UnityWebRequest www = UnityWebRequest.Delete($"{baseUrl}/api/v1{endpoint}");
            www.downloadHandler = new DownloadHandlerBuffer();
            SetupHeaders(www);
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                try
                {
                    var problemDetails = JsonUtility.FromJson<ProblemDetails>(www.downloadHandler.text);
                    onError?.Invoke(problemDetails);
                }
                catch
                {
                    onError?.Invoke(ProblemDetails.FromError(www.error));
                }
                yield break;
            }
            
            onSuccess?.Invoke();
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
    }
}