using UnityEngine;

namespace GameCloud
{
    public class GameCloudSettings : ScriptableObject
    {
        private const string SettingsPath = "Assets/GameCloud/Resources";
        private const string SettingsAssetName = "GameCloudSettings";

        [Header("Connection Settings")]
        public string GameKey = "";
        public string Host = "api.gamecloud.example.com";
        public int Port = 7350;
        public bool UseSSL = true;

        [Header("Authentication Settings")]
        public bool AllowGuestAuth = true;
        public bool AllowCustomAuth = true;
        public int SessionTokenExpiry = 3600;

        [Header("Matchmaking Settings")]
        public string DefaultQueueName = "default";
        public int MatchmakingTimeout = 60;
        public int MaxPlayersPerMatch = 2;

        [Header("Advanced Settings")]
        public bool EnableDebugLogs = true;
        public int RequestTimeout = 30;

        public static GameCloudSettings Load()
        {
            var settings = Resources.Load<GameCloudSettings>(SettingsAssetName);
            
            if (settings == null)
            {
                settings = CreateInstance<GameCloudSettings>();
                
                if (!System.IO.Directory.Exists(SettingsPath))
                {
                    System.IO.Directory.CreateDirectory(SettingsPath);
                }
                
                UnityEditor.AssetDatabase.CreateAsset(settings, $"{SettingsPath}/{SettingsAssetName}.asset");
                UnityEditor.AssetDatabase.SaveAssets();
            }
            
            return settings;
        }

        public void Save()
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        public void ResetToDefaults()
        {
            GameKey = "";
            Host = "api.gamecloud.example.com";
            Port = 7350;
            UseSSL = true;
            AllowGuestAuth = true;
            AllowCustomAuth = true;
            SessionTokenExpiry = 3600;
            DefaultQueueName = "default";
            MatchmakingTimeout = 60;
            MaxPlayersPerMatch = 2;
            EnableDebugLogs = true;
            RequestTimeout = 30;
        }
    }
} 