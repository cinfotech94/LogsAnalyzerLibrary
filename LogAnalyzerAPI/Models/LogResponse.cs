namespace LogAnalyzerAPI.Models
{
    public class LogResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> FoundLogs { get; set; }
        public Dictionary<string, int> ErrorCounts { get; set; }
    }
}
