using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameCloud.Editor
{
    public class GameCloudLogWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<LogEntry> logs = new List<LogEntry>();
        private bool showRequests = true;
        private bool showResponses = true;
        private string searchFilter = "";
        private GUIStyle logStyle;

        private struct LogEntry
        {
            public string Message;
            public LogType Type;
            public string Timestamp;
            public bool IsRequest;
        }

        public static GameCloudLogWindow ShowWindow()
        {
            var window = GetWindow<GameCloudLogWindow>("GameCloud Logs");
            window.minSize = new Vector2(400, 300);
            return window;
        }

        private void OnEnable()
        {
            logStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                wordWrap = true
            };
        }

        public static void AddLog(string message, bool isRequest, LogType type = LogType.Log)
        {
            var window = GetWindow<GameCloudLogWindow>();
            window.logs.Add(new LogEntry
            {
                Message = message,
                Type = type,
                Timestamp = System.DateTime.Now.ToString("HH:mm:ss"),
                IsRequest = isRequest
            });
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawSearchBar();
            DrawLogs();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                logs.Clear();
            }
            
            showRequests = GUILayout.Toggle(showRequests, "Requests", EditorStyles.toolbarButton);
            showResponses = GUILayout.Toggle(showResponses, "Responses", EditorStyles.toolbarButton);
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLogs()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var log in logs)
            {
                if (!ShouldShowLog(log)) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"[{log.Timestamp}]", GUILayout.Width(70));
                EditorGUILayout.LabelField(log.IsRequest ? "REQUEST" : "RESPONSE", 
                    EditorStyles.boldLabel, GUILayout.Width(80));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField(log.Message, logStyle);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();
        }

        private bool ShouldShowLog(LogEntry log)
        {
            if (!string.IsNullOrEmpty(searchFilter) && 
                !log.Message.ToLower().Contains(searchFilter.ToLower()))
                return false;

            if (log.IsRequest && !showRequests) return false;
            if (!log.IsRequest && !showResponses) return false;

            return true;
        }
    }
}