using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameCloud
{
    internal class GameCloudCoroutineRunner : MonoBehaviour
    {
        private static GameCloudCoroutineRunner instance;
        
        private List<Api.GameCloudApiClient> _clients = new List<Api.GameCloudApiClient>();
        
        public static GameCloudCoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("GameCloudCoroutineRunner");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<GameCloudCoroutineRunner>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void RegisterClient(Api.GameCloudApiClient client)
        {
            if (!_clients.Contains(client))
            {
                _clients.Add(client);
            }
        }
        
        public void UnregisterClient(Api.GameCloudApiClient client)
        {
            if (_clients.Contains(client))
            {
                _clients.Remove(client);
            }
        }
        
        private void Update()
        {
            var clientsCopy = new List<Api.GameCloudApiClient>(_clients);
            
            foreach (var client in clientsCopy)
            {
                if (client != null)
                {
                    client.Update();
                }
            }
        }
    }
}