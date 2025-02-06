using UnityEngine;
using UnityEditor;
using GameCloud;

namespace GameCloud.Editor
{
    public class GameCloudEditorWindow : EditorWindow
    {
        private GameCloudSettings settings;
        private Vector2 scrollPosition;
        private bool showAuthentication = true;
        private bool showMatchmaking = true;
        private bool showAdvanced = false;

        [MenuItem("Window/Game Cloud/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameCloudEditorWindow>("Game Cloud Settings");
            window.minSize = new Vector2(400, 550);
            window.Show();
        }

        private void OnEnable()
        {
            if (settings == null)
            {
                settings = GameCloudSettings.Load();
            }
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                settings = GameCloudSettings.Load();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            EditorGUILayout.Space(10);
            DrawMainSettings();
            EditorGUILayout.Space(10);
            DrawAuthenticationSettings();
            EditorGUILayout.Space(10);
            DrawMatchmakingSettings();
            EditorGUILayout.Space(10);
            DrawAdvancedSettings();
            EditorGUILayout.Space(20);
            DrawButtons();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Game Cloud Settings", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMainSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Connection Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            settings.GameKey = EditorGUILayout.TextField("Game Key", settings.GameKey);
            settings.Host = EditorGUILayout.TextField("Host", settings.Host);
            settings.Port = EditorGUILayout.IntField("Port", settings.Port);
            settings.UseSSL = EditorGUILayout.Toggle("Use SSL", settings.UseSSL);

            EditorGUILayout.EndVertical();
        }

        private void DrawAuthenticationSettings()
        {
            showAuthentication = EditorGUILayout.Foldout(showAuthentication, "Authentication Settings", true);
            if (showAuthentication)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                settings.AllowGuestAuth = EditorGUILayout.Toggle("Allow Guest Auth", settings.AllowGuestAuth);
                settings.AllowCustomAuth = EditorGUILayout.Toggle("Allow Custom Auth", settings.AllowCustomAuth);
                settings.SessionTokenExpiry = EditorGUILayout.IntField("Session Token Expiry (seconds)", settings.SessionTokenExpiry);
                
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMatchmakingSettings()
        {
            showMatchmaking = EditorGUILayout.Foldout(showMatchmaking, "Matchmaking Settings", true);
            if (showMatchmaking)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                settings.DefaultQueueName = EditorGUILayout.TextField("Default Queue Name", settings.DefaultQueueName);
                settings.MatchmakingTimeout = EditorGUILayout.IntField("Matchmaking Timeout (seconds)", settings.MatchmakingTimeout);
                settings.MaxPlayersPerMatch = EditorGUILayout.IntField("Max Players Per Match", settings.MaxPlayersPerMatch);
                
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawAdvancedSettings()
        {
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Settings", true);
            if (showAdvanced)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                settings.EnableDebugLogs = EditorGUILayout.Toggle("Enable Debug Logs", settings.EnableDebugLogs);
                settings.RequestTimeout = EditorGUILayout.IntField("Request Timeout (seconds)", settings.RequestTimeout);
                
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Save Settings"))
            {
                settings.Save();
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                Debug.Log("Game Cloud settings saved successfully!");
            }

            if (GUILayout.Button("Reset to Defaults"))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", 
                    "Are you sure you want to reset all settings to default values?", 
                    "Yes", "No"))
                {
                    settings.ResetToDefaults();
                    EditorUtility.SetDirty(settings);
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
} 