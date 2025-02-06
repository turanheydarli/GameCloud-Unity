using UnityEngine;
using GameCloud.Api;
using System.Collections;

namespace GameCloud.Core
{
    public class GameCloudClient : MonoBehaviour
    {
        private static GameCloudClient instance;
        public static GameCloudClient Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameCloudClient");
                    instance = go.AddComponent<GameCloudClient>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }


        private GameCloudApiClient apiClient;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }


        public void Initialize(string baseUrl, string gameKey)
        {
            apiClient = new GameCloudApiClient(baseUrl, gameKey);
        }

        
    }
} 