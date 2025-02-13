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
            
            var settings = GameCloudSettings.Load();
            if (!settings.EnableDebugLogs) return;

            if (logString.StartsWith("<click>") && logString.EndsWith("</click>"))
            {
                var cleanLog = Regex.Replace(logString, "<click>|</click>", "");
                var isRequest = cleanLog.Contains("[GameCloud] ") && cleanLog.Contains("Request");
                
                if (settings.ShowDetailedLogs)
                {
                    var headers = ExtractHeaders(cleanLog);
                    var body = ExtractBody(cleanLog);
                    var message = ExtractFirstLine(cleanLog);
                    
                    GameCloudLogWindow.AddLog(message, headers, body, isRequest, type);
                }
            }
        }

        private static string ExtractFirstLine(string log)
        {
            var index = log.IndexOf('\n');
            return index > 0 ? log.Substring(0, index) : log;
        }

        private static string ExtractHeaders(string log)
        {
            var startIndex = log.IndexOf("Headers:");
            if (startIndex == -1) return string.Empty;

            var endIndex = log.IndexOf("Body:", startIndex);
            if (endIndex == -1) endIndex = log.Length;

            var headers = log.Substring(startIndex, endIndex - startIndex).Trim();
            return headers;
        }

        private static string ExtractBody(string log)
        {
            var index = log.IndexOf("Body:");
            if (index == -1) return string.Empty;

            return log.Substring(index).Trim();
        }

        private static string FormatLogWithHeaders(string log, string headers)
        {
            if (string.IsNullOrEmpty(headers))
                return log;

            var parts = log.Split(new[] { "\nBody:" }, System.StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length > 1)
            {
                return $"{parts[0]}\n{headers}Body:{parts[1]}";
            }
            
            return $"{log}\n{headers}";
        }
    }
}