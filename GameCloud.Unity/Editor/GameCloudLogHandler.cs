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
                var isRequest = logString.Contains("Request:");
                
                if (settings.ShowDetailedLogs)
                {
                    GameCloudLogWindow.AddLog(cleanLog, isRequest, type);
                }
            }
        }
    }
}