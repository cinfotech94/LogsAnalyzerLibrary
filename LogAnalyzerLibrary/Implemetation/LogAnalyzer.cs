using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LogAnalyzerLibrary.Interface;
using Microsoft.AspNetCore.Http;

namespace LogAnalyzerLibrary.Model
{
    public class LogAnalyzer: ILogAnalyzer
    {
        public List<string> SearchLogsInDirectories(List<string> directoryPaths)
        {
            var foundLogs = new List<string>();

            foreach (var directoryPath in directoryPaths)
            {
                if (Directory.Exists(directoryPath))
                {
                    var logFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
                        .Where(filePath => filePath.ToLower().EndsWith(".log"));

                    foundLogs.AddRange(logFiles);
                }
                else
                {
                    Console.WriteLine($"Directory {directoryPath} does not exist.");
                }
            }

            return foundLogs;
        }

        public Dictionary<string, int> CountUniqueErrorsPerLogFile(IFormFile logFile)
        {
            var uniqueErrorsCount = new Dictionary<string, int>();

            using (var reader = new StreamReader(logFile.OpenReadStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!uniqueErrorsCount.ContainsKey(line))
                    {
                        uniqueErrorsCount[line] = 1;
                    }
                    else
                    {
                        uniqueErrorsCount[line]++;
                    }
                }
            }

            return uniqueErrorsCount;
        }

        public Dictionary<string, int> CountDuplicatedErrorsPerLogFile(IFormFile logFile)
        {
            var duplicatedErrorsCount = new Dictionary<string, int>();
            var uniqueErrors = new HashSet<string>();

            using (var reader = new StreamReader(logFile.OpenReadStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!uniqueErrors.Contains(line))
                    {
                        uniqueErrors.Add(line);
                    }
                    else
                    {
                        if (!duplicatedErrorsCount.ContainsKey(line))
                        {
                            duplicatedErrorsCount[line] = 2;
                        }
                        else
                        {
                            duplicatedErrorsCount[line]++;
                        }
                    }
                }
            }

            return duplicatedErrorsCount;
        }

        public void DeleteArchiveFromPeriod(DateTime startDate, DateTime endDate, string directoryPath)
        {
            var archiveFiles = Directory.GetFiles(directoryPath, "*.zip");

            foreach (var archiveFile in archiveFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(archiveFile);
                if (DateTime.TryParseExact(fileName, "dd_MM_yyyy", null, System.Globalization.DateTimeStyles.None, out var archiveDate))
                {
                    if (archiveDate >= startDate && archiveDate <= endDate)
                    {
                        File.Delete(archiveFile);
                    }
                }
            }
        }

        public void ArchiveLogsFromPeriod(DateTime startDate, DateTime endDate, string logDirectory)
        {
            var zipFileName = $"{startDate:dd_MM_yyyy}-{endDate:dd_MM_yyyy}.zip";
            var zipFilePath = Path.Combine(logDirectory, zipFileName);

            using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                var logFiles = Directory.GetFiles(logDirectory, "*.log");

                foreach (var logFile in logFiles)
                {
                    var fileCreationDate = File.GetCreationTime(logFile);

                    if (fileCreationDate >= startDate && fileCreationDate <= endDate)
                    {
                        archive.CreateEntryFromFile(logFile, Path.GetFileName(logFile));

                        File.Delete(logFile);
                    }
                }
            }
        }

        public async Task UploadLogsOnRemoteServer(string remoteServerUrl, List<IFormFile> logFiles)
        {
            using (var client = new HttpClient())
            {
                foreach (var file in logFiles)
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                        var response = await client.PostAsync(remoteServerUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to upload {file.FileName} to remote server.");
                        }
                    }
                }
            }
        }

        public void DeleteLogsFromPeriod(DateTime startDate, DateTime endDate, string logDirectory)
        {

            var logFiles = Directory.GetFiles(logDirectory, "*.log");

            foreach (var logFile in logFiles)
            {
                var fileCreationDate = File.GetCreationTime(logFile);

                if (fileCreationDate >= startDate && fileCreationDate <= endDate)
                {
                    File.Delete(logFile);
                }
            }
        }

        public int CountTotalAvailableLogsInPeriod(DateTime startDate, DateTime endDate, string logDirectory)
        {
            var logFiles = Directory.GetFiles(logDirectory, "*.log");

            var totalLogs = 0;

            foreach (var logFile in logFiles)
            {
                var fileCreationDate = File.GetCreationTime(logFile);

                if (fileCreationDate >= startDate && fileCreationDate <= endDate)
                {
                    totalLogs++;
                }
            }

            return totalLogs;
        }

        public List<string> SearchLogsPerSize(string directoryPath, int minSizeKB, int maxSizeKB)
        {
            var logFiles = Directory.GetFiles(directoryPath, "*.log");

            var matchedLogs = new List<string>();

            foreach (var logFile in logFiles)
            {
                var fileSizeKB = new FileInfo(logFile).Length / 1024;

                if (fileSizeKB >= minSizeKB && fileSizeKB <= maxSizeKB)
                {
                    matchedLogs.Add(logFile);
                }
            }

            return matchedLogs;
        }

        public List<string> SearchLogsPerDirectory(string rootDirectory, string searchDirectory)
        {
            var fullSearchPath = Path.Combine(rootDirectory, searchDirectory);

            if (!Directory.Exists(fullSearchPath))
            {
                throw new DirectoryNotFoundException($"Directory '{fullSearchPath}' not found.");
            }

            var matchedLogs = new List<string>();

            SearchLogFiles(fullSearchPath, matchedLogs);

            return matchedLogs;
        }

        private void SearchLogFiles(string directoryPath, List<string> matchedLogs)
        {
            var logFiles = Directory.GetFiles(directoryPath, "*.log");

            matchedLogs.AddRange(logFiles);

            foreach (var subDirectory in Directory.GetDirectories(directoryPath))
            {
                SearchLogFiles(subDirectory, matchedLogs);
            }
        }
    }
}
