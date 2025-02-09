using System;
using GameCloud.Models;

namespace GameCloud.Core.Exceptions
{
    public class GameCloudException : Exception
    {
        public ProblemDetails ProblemDetails { get; }

        public GameCloudException(string message) : base(message)
        {
        }

        public GameCloudException(ProblemDetails problemDetails) : base(problemDetails?.Detail 
                                                                        ?? problemDetails?.Title 
                                                                        ?? "Unknown error")
        {
            ProblemDetails = problemDetails;
        }
    }
}