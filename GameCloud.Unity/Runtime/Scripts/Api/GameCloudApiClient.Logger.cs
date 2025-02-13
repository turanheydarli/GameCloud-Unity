using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameCloud.Models;
using Newtonsoft.Json;

namespace GameCloud.Api
{
    public partial class GameCloudApiClient
    {
        private void LogRequest(string method, string endpoint, object data = null,
            Dictionary<string, string> headers = null)
        {
            if (!useLogger) return;
        
            var headerStr = new StringBuilder();
            if (headers != null)
            {
                headerStr.AppendLine("Headers:");
                foreach (var header in headers)
                {
                    headerStr.AppendLine($"  {header.Key}: {header.Value}");
                }
            }
        
            var bodyStr = data != null ? JsonConvert.SerializeObject(data, Formatting.Indented) : "";
        
            var logEvent = new GameCloudLogEvent
            {
                Method = method,
                Endpoint = endpoint,
                Message = $"[GameCloud] {method} Request: {baseUrl}/api/v1{endpoint}",
                Headers = headerStr.ToString(),
                Body = bodyStr,
                IsRequest = true
            };
        
            GameCloudLogEvent.Emit(logEvent);
            Debug.Log(logEvent.Message);
        }
        
        private void LogResponse(string method, string endpoint, string response, Dictionary<string, string> headers,
            bool isError = false)
        {
            if (!useLogger) return;
        
            var headerStr = new StringBuilder();
            if (headers != null)
            {
                headerStr.AppendLine("Headers:");
                foreach (var header in headers)
                {
                    headerStr.AppendLine($"  {header.Key}: {header.Value}");
                }
            }
        
            string formattedBody;
            try
            {
                var obj = JsonConvert.DeserializeObject(response);
                formattedBody = JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch
            {
                formattedBody = response;
            }
        
            var logEvent = new GameCloudLogEvent
            {
                Method = method,
                Endpoint = endpoint,
                Message = $"[GameCloud] {method} Response: {baseUrl}/api/v1{endpoint}",
                Headers = headerStr.ToString(),
                Body = formattedBody,
                IsRequest = false,
                IsError = isError
            };
        
            GameCloudLogEvent.Emit(logEvent);
        
            if (logger != null)
                logger.Log(isError ? LogType.Error : LogType.Log, logEvent.Message);
            else
            {
        #if UNITY_EDITOR
                if (isError)
                    Debug.LogError(logEvent.Message);
                else
                    Debug.Log(logEvent.Message);
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