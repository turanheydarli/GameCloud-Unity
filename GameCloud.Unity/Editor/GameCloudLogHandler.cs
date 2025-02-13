using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameCloud.Editor
{
    [InitializeOnLoad]
    public class GameCloudLogHandler
    {
        static GameCloudLogHandler()
        {
            Application.logMessageReceived += HandleLog;
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!logString.Contains("[GameCloud]")) return;
            
            var window = EditorWindow.GetWindow<GameCloudLogWindow>();
            if (window == null) return;
        
            var isRequest = logString.Contains("Request");
            var method = ExtractMethod(logString);
            var endpoint = ExtractEndpoint(logString);
            var headers = ExtractHeaders(logString);
            var body = ExtractBody(logString);
            
            window.AddLogFromConsole(logString, isRequest, endpoint, method, headers, body, type);
        }

        private static string ExtractHeaders(string logString)
        {
            var start = logString.IndexOf("Headers:");
            if (start == -1) return string.Empty;

            var end = logString.IndexOf("Body:", start);
            if (end == -1) end = logString.Length;

            return logString.Substring(start, end - start).Trim();
        }

        private static string ExtractBody(string logString)
        {
            var start = logString.IndexOf("Body:");
            if (start == -1) return string.Empty;

            return logString.Substring(start).Trim();
        }

        private static string ExtractMethod(string logString)
        {
            var parts = logString.Split(']');
            if (parts.Length < 2) return "";
            
            var methodParts = parts[1].Trim().Split(' ');
            return methodParts.Length > 0 ? methodParts[0] : "";
        }

        private static string ExtractEndpoint(string logString)
        {
            var apiIndex = logString.IndexOf("/api/v1");
            if (apiIndex == -1) return "";
            
            return logString.Substring(apiIndex + 7).Split('\n')[0].Trim();
        }
    }
}