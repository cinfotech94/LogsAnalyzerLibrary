namespace LogAnalyzerAPI.Models
{
    public class LogRequest
    {
        public List<string> Directories { get; set; }
        public string LogFile { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MinSizeKB { get; set; }
        public int MaxSizeKB { get; set; }
        public string RootDirectory { get; set; }
        public string SearchDirectory { get; set; }
    }
    public class SearchLogsRequest
    {
        public string DirectoryPath { get; set; }
        public int MinSizeKB { get; set; }
        public int MaxSizeKB { get; set; }
    }
    public class DeleteLogsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LogDirectory { get; set; }
    }
    public class SearchLogRequest
    {
        public string RootDirectory { get; set; }
        public string SearchDirectory { get; set; }
    }
    public class ArchiveLogsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LogDirectory { get; set; }
    }
    public class CountLogsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LogDirectory { get; set; }
    }
    public class ArchiveDeleteRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DirectoryPath { get; set; }
    }
}
