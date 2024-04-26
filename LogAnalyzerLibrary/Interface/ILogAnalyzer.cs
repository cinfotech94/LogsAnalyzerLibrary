using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLibrary.Interface
{
    public interface ILogAnalyzer
    {
        List<string> SearchLogsInDirectories(List<string> directoryPaths);
        Dictionary<string, int> CountUniqueErrorsPerLogFile(IFormFile logFile);
        Dictionary<string, int> CountDuplicatedErrorsPerLogFile(IFormFile logFile);
        void DeleteArchiveFromPeriod(DateTime startDate, DateTime endDate, string directoryPath);
        void ArchiveLogsFromPeriod(DateTime startDate, DateTime endDate, string logDirectory);
        Task UploadLogsOnRemoteServer(string remoteServerUrl, List<IFormFile> logFiles);
        void DeleteLogsFromPeriod(DateTime startDate, DateTime endDate, string logDirectory);
        int CountTotalAvailableLogsInPeriod(DateTime startDate, DateTime endDate, string logDirectory);
        List<string> SearchLogsPerSize(string directoryPath, int minSizeKB, int maxSizeKB);
        List<string> SearchLogsPerDirectory(string rootDirectory, string searchDirectory);
    }
}
