using System;
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

        private Dictionary<int, bool> headerFoldouts = new Dictionary<int, bool>();

        private struct LogEntry
        {
            public string Message;
            public LogType Type;
            public string Timestamp;
            public bool IsRequest;
            public string Headers;
            public string Body;
        }

        private void DrawLogEntry(LogEntry log, int index)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Header row with timestamp and type
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"[{log.Timestamp}]", GUILayout.Width(70));
            EditorGUILayout.LabelField(log.IsRequest ? "REQUEST" : "RESPONSE",
                EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            // Headers foldout
            if (!string.IsNullOrEmpty(log.Headers))
            {
                if (!headerFoldouts.ContainsKey(index))
                    headerFoldouts[index] = false;

                var style = new GUIStyle(EditorStyles.foldout);
                style.normal.textColor = new Color(0.5f, 0.7f, 1f);
                headerFoldouts[index] = EditorGUILayout.Foldout(headerFoldouts[index], "Headers", true, style);
                
                if (headerFoldouts[index])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(log.Headers, logStyle);
                    EditorGUI.indentLevel--;
                }
            }

            // Main content
            if (!string.IsNullOrEmpty(log.Body))
            {
                EditorGUILayout.LabelField(log.Body, logStyle);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
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

        public static void AddLog(string message, string headers, string body, bool isRequest, LogType type = LogType.Log)
        {
            var window = GetWindow<GameCloudLogWindow>();
            window.logs.Add(new LogEntry
            {
                Message = message,
                Headers = headers,
                Body = body,
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

                DrawLogEntry(log, logs.IndexOf(log));

                // EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //
                // EditorGUILayout.BeginHorizontal();
                // EditorGUILayout.LabelField($"[{log.Timestamp}]", GUILayout.Width(70));
                // EditorGUILayout.LabelField(log.IsRequest ? "REQUEST" : "RESPONSE",
                //     EditorStyles.boldLabel, GUILayout.Width(80));
                // EditorGUILayout.EndHorizontal();
                //
                // EditorGUILayout.LabelField(log.Message, logStyle);
                // EditorGUILayout.EndVertical();
                // EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndScrollView();
        }

        private string ExtractHeaders(string message)
        {
            var start = message.IndexOf("Headers:");
            if (start == -1) return string.Empty;

            var end = message.IndexOf("Body:");
            if (end == -1) end = message.Length;

            return message.Substring(start, end - start);
        }

        private string RemoveHeaders(string message)
        {
            var parts = message.Split(new[] { "Headers:" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1) return message;

            var mainContent = parts[1];
            var headerEnd = mainContent.IndexOf("Body:");
            if (headerEnd != -1)
            {
                return $"{parts[0]}Body:{mainContent.Substring(headerEnd + 5)}";
            }

            return parts[0];
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