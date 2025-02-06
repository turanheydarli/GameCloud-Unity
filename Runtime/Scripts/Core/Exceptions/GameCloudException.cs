using System;

namespace GameCloud
{
    public class GameCloudException : Exception
    {
        public GameCloudException(string message) : base(message)
        {
        }

        public GameCloudException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}