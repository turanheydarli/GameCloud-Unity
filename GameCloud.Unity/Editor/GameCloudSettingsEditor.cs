using UnityEngine;
using UnityEditor;

namespace GameCloud.Editor
{
    public class GameCloudSettingsEditor : EditorWindow
    {
        private const float MIN_WINDOW_WIDTH = 350f;
        private const float MIN_WINDOW_HEIGHT = 500f;
        private const float SECTION_SPACING = 10f;
        private const float WIDE_MODE_WIDTH = 600f;

        private GameCloudSettings settings;
        private Vector2 scrollPosition;
        private GUIStyle headerStyle;
        private GUIStyle sectionStyle;
        private Texture2D headerIcon;

        [MenuItem("Window/Game Cloud/Settings1", false, 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<GameCloudSettingsEditor>("Game Cloud");
            window.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
        }

        private void OnEnable()
        {
            settings = GameCloudSettings.Load();
            headerIcon = EditorGUIUtility.FindTexture("d_SettingsIcon");
            
            headerStyle = new GUIStyle(EditorStyles.label)  // Changed from largeLabel to label
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, 10, 10),
                normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
            };

            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(10, 10, 5, 5)
            };
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                settings = GameCloudSettings.Load();
            }

            var isWideMode = position.width >= WIDE_MODE_WIDTH;
            DrawHeader();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (isWideMode)
            {
                DrawWideLayout();
            }
            else
            {
                DrawNarrowLayout();
            }

            DrawActionButtons();
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }
        }

        private void DrawWideLayout()
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2 - SECTION_SPACING));
            DrawConnectionSettings();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 2 - SECTION_SPACING));
            DrawAdvancedSettings();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNarrowLayout()
        {
            DrawConnectionSettings();
            EditorGUILayout.Space(SECTION_SPACING);
            DrawAdvancedSettings();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                
                if (headerIcon != null)
                {
                    GUILayout.Label(new GUIContent(headerIcon), GUILayout.Width(32), GUILayout.Height(32));
                    GUILayout.Space(10);
                }
                
                GUILayout.Label("Game Cloud Settings", headerStyle, GUILayout.ExpandWidth(true));
                
                GUILayout.Space(42); // Balance the left side spacing (icon width + spaces)
            }
            
            EditorGUILayout.Space(10);
            
            // Draw separator line
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.5f, 0.5f, 0.5f, 0.5f));
            EditorGUILayout.Space(5);
        }

        private void DrawConnectionSettings()
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            DrawSectionHeader("Connection Settings");
            
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            settings.GameKey = EditorGUILayout.TextField(new GUIContent("Game Key", "Your Game Cloud API key"), settings.GameKey);
            settings.Host = EditorGUILayout.TextField(new GUIContent("Host", "Game Cloud server host"), settings.Host);
            settings.Port = EditorGUILayout.IntField(new GUIContent("Port", "Game Cloud server port"), settings.Port);
            settings.UseSSL = EditorGUILayout.Toggle(new GUIContent("Use SSL", "Enable SSL for secure connections"), settings.UseSSL);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
        }

        private void DrawSectionHeader(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
        }

        private void DrawAdvancedSettings()
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUI.indentLevel++;
            settings.EnableDebugLogs = EditorGUILayout.Toggle(new GUIContent("Enable Debug Logs", "Enable debug logging"), settings.EnableDebugLogs);
            settings.ShowDetailedLogs = EditorGUILayout.Toggle(new GUIContent("Show Detailed Logs", "Show detailed request/response logs in console"), settings.ShowDetailedLogs);
            settings.RequestTimeout = EditorGUILayout.IntField(new GUIContent("Request Timeout", "API request timeout in seconds"), settings.RequestTimeout);
            EditorGUI.indentLevel--;
        
            if (settings.EnableDebugLogs)
            {
                EditorGUILayout.Space(5);
                if (GUILayout.Button("Open Log Window"))
                {
                    GameCloudLogWindow.ShowWindow();
                }
            }
        
            EditorGUILayout.EndVertical();
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.Space(SECTION_SPACING);
            var buttonWidth = Mathf.Min(200f, (position.width - 3 * SECTION_SPACING) / 2);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save", GUILayout.Width(buttonWidth)))
            {
                settings.Save();
                Debug.Log("GameCloud settings saved successfully!");
            }

            GUILayout.Space(SECTION_SPACING);

            if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
            {
                if (EditorUtility.DisplayDialog("Reset Settings",
                        "Are you sure you want to reset all settings to their default values?", "Yes", "No"))
                {
                    settings.ResetToDefaults();
                    settings.Save();
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(SECTION_SPACING);
        }
    }
}