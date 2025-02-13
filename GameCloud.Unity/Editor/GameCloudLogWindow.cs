using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GameCloud.Api;

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
        private GUIStyle headerBoxStyle;
        private GUIStyle typeStyle;
        private GUIStyle methodStyle;
        private GUIStyle foldoutStyle;

        private Dictionary<int, bool> headerFoldouts = new Dictionary<int, bool>();

        private struct LogEntry
        {
            public string Message;
            public LogType Type;
            public string Timestamp;
            public bool IsRequest;
            public string Headers;
            public string Body;
            public string Endpoint;
            public string Method;
        }
        
        private void DrawLogEntry(LogEntry log, int index)
        {
            EditorGUILayout.BeginVertical(headerBoxStyle);

            // Header row
            using (new EditorGUILayout.HorizontalScope())
            {
                // Timestamp
                EditorGUILayout.LabelField($"[{log.Timestamp}]", GUILayout.Width(70));
                
                // Request/Response label
                var color = GUI.color;
                GUI.color = log.Type == LogType.Error ? Color.red : Color.white;
                EditorGUILayout.LabelField(log.IsRequest ? "REQUEST" : "RESPONSE", typeStyle, GUILayout.Width(80));
                GUI.color = color;

                // Method and endpoint
                EditorGUILayout.LabelField($"{log.Method} {log.Endpoint}", methodStyle);
            }

            EditorGUILayout.Space(5);

            // Headers section
            if (!string.IsNullOrEmpty(log.Headers))
            {
                if (!headerFoldouts.ContainsKey(index))
                    headerFoldouts[index] = false;

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    headerFoldouts[index] = EditorGUILayout.Foldout(headerFoldouts[index], "Headers", true, foldoutStyle);
                    if (headerFoldouts[index])
                    {
                        EditorGUI.indentLevel++;
                        var content = new GUIContent(log.Headers);
                        var height = logStyle.CalcHeight(content, EditorGUIUtility.currentViewWidth - 60);
                        EditorGUILayout.LabelField(content, logStyle, GUILayout.Height(height));
                        EditorGUI.indentLevel--;
                    }
                }
                
                EditorGUILayout.Space(5);
            }

            // Body section
            if (!string.IsNullOrEmpty(log.Body))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    var content = new GUIContent(log.Body);
                    var height = logStyle.CalcHeight(content, EditorGUIUtility.currentViewWidth - 40);
                    EditorGUILayout.LabelField(content, logStyle, GUILayout.Height(height));
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        public static GameCloudLogWindow ShowWindow()
        {
            var window = GetWindow<GameCloudLogWindow>("GameCloud Logs");
            window.minSize = new Vector2(400, 300);
            return window;
        }

        private void OnEnable()
        {
            GameCloudLogEvent.OnLog += OnGameCloudLog;
            InitializeStyles();
        }

        private void InitializeStyles()
        {
            logStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                wordWrap = true,
                fontSize = 12,
                padding = new RectOffset(10, 10, 5, 5),
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
            };

            headerBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(15, 15, 10, 10),
                margin = new RectOffset(0, 0, 5, 5),
                fontSize = 12
            };

            typeStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 20
            };

            methodStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                normal = { textColor = new Color(0.4f, 0.8f, 1f) },
                fontStyle = FontStyle.Bold
            };

            foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                normal = { textColor = new Color(0.7f, 0.9f, 1f) },
                fontStyle = FontStyle.Bold,
                fontSize = 11,
                padding = new RectOffset(15, 0, 0, 0)
            };
        }

        private void OnDisable()
        {
            GameCloudLogEvent.OnLog -= OnGameCloudLog;
        }

        private void OnGameCloudLog(GameCloudLogEvent logEvent)
        {
            AddLogEntry(logEvent.Message, logEvent.Headers, logEvent.Body, logEvent.IsRequest, logEvent.Endpoint,
                logEvent.Method, logEvent.IsError ? LogType.Error : LogType.Log);
        }

        private void AddLogEntry(string message, string headers, string body, bool isRequest, string endpoint,
            string method, LogType type = LogType.Log)
        {
            logs.Add(new LogEntry
            {
                Message = message,
                Headers = headers,
                Body = body,
                Type = type,
                Timestamp = System.DateTime.Now.ToString("HH:mm:ss"),
                IsRequest = isRequest,
                Endpoint = endpoint,
                Method = method
            });
            
            Repaint();
        }

        // Remove AddLog and AddLogFromConsole methods

        // public void AddLogFromConsole(string message, bool isRequest, string endpoint, string method,
        //     string headers, string body, LogType type)
        // {
        //     AddLogEntry(message, headers, body, isRequest, endpoint, method, type);
        // }

        // Remove ExtractHeaders and RemoveHeaders methods as they're no longer needed

        private void OnGUI()
        {
                  
            if (logStyle == null)
            {
                InitializeStyles();
            }
            
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