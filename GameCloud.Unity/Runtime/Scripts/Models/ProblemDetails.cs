using System.Collections.Generic;

namespace GameCloud.Models
{
    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public Dictionary<string, object> Extensions { get; set; }

        public override string ToString()
        {
            return $"{Title}: {Detail} (Status: {Status})";
        }

        public static ProblemDetails FromError(string error)
        {
            return new ProblemDetails
            {
                Title = "Error",
                Detail = error,
                Status = 500
            };
        }
    }
}