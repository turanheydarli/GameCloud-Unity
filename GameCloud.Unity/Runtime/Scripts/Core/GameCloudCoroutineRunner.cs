using UnityEngine;
using System.Collections;

namespace GameCloud
{
    internal class GameCloudCoroutineRunner : MonoBehaviour
    {
        private static GameCloudCoroutineRunner instance;
        
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
    }
}