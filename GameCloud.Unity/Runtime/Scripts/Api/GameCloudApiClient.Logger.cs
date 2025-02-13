using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using GameCloud.Models;
using Newtonsoft.Json;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        private void LogRequest(string method, string endpoint, object data = null)
        {
            if (!useLogger) return;
            
            var logMessage = new StringBuilder();
            logMessage.AppendLine($"[GameCloud] {method} Request: {baseUrl}/api/v1{endpoint}");
            
            logMessage.AppendLine("Headers:");
            logMessage.AppendLine("  Content-Type: application/json");
            logMessage.AppendLine($"  X-Game-Key: {gameKey}");
            if (!string.IsNullOrEmpty(authToken))
            {
                logMessage.AppendLine($"  Authorization: Bearer {authToken}");
            }
            
            if (data != null)
            {
                logMessage.AppendLine("Body:");
                logMessage.AppendLine(JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            
            Debug.Log($"<click>{logMessage}</click>");
        }

        private void LogResponse(string method, string endpoint, string response, bool isError = false)
        {
            if (!useLogger) return;
    
            var logMessage = FormatResponseLog(method, endpoint, response);
            
            if (logger != null)
                logger.Log(isError ? LogType.Error : LogType.Log, logMessage);
            else
            {
                #if UNITY_EDITOR
                if (isError)
                    Debug.LogError($"<click>{logMessage}</click>");
                else
                    Debug.Log($"<click>{logMessage}</click>");
                #else
                if (isError)
                    Debug.LogError(logMessage);
                else
                    Debug.Log(logMessage);
                #endif
            }
        }

        private string FormatRequestLog(string method, string endpoint, object data)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<b>[GameCloud] {method} Request</b>");
            sb.AppendLine($"<color=#80FF80>URL:</color> {baseUrl}/api/v1{endpoint}");
            
            if (data != null)
            {
                sb.AppendLine("<color=#80FF80>Body:</color>");
                sb.AppendLine(JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            
            return sb.ToString();
        }

        private string FormatResponseLog(string method, string endpoint, string response)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<b>[GameCloud] {method} Response</b>");
            sb.AppendLine($"<color=#80FF80>URL:</color> {baseUrl}/api/v1{endpoint}");
            
            try
            {
                // Try to parse and format JSON response
                var obj = JsonConvert.DeserializeObject(response);
                sb.AppendLine("<color=#80FF80>Response:</color>");
                sb.AppendLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
            catch
            {
                // If not JSON, just append the raw response
                sb.AppendLine("<color=#80FF80>Response:</color>");
                sb.AppendLine(response);
            }
            
            return sb.ToString();
        }
    }
}