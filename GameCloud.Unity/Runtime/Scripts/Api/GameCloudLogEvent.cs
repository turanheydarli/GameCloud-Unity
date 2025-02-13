namespace GameCloud.Api
{
    public class GameCloudLogEvent
    {
        public string Method { get; set; }
        public string Endpoint { get; set; }
        public string Message { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
        public bool IsRequest { get; set; }
        public bool IsError { get; set; }

        public static event System.Action<GameCloudLogEvent> OnLog;
        
        public static void Emit(GameCloudLogEvent logEvent)
        {
            OnLog?.Invoke(logEvent);
        }
    }
}