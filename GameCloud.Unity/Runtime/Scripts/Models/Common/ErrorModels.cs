using System;

namespace GameCloud.Models
{
    [Serializable]
    public class GameCloudError
    {
        public string code;
        public string message;
        public object details;
    }

    [Serializable]
    public class ApiResponse<T>
    {
        public bool success;
        public T data;
        public GameCloudError error;
    }
} 