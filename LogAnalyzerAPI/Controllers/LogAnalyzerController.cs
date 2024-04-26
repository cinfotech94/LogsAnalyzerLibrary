using Microsoft.AspNetCore.Mvc;
using LogAnalyzerAPI.Models;
using LogAnalyzerLibrary.Model;
using LogAnalyzerLibrary.Interface;

namespace LogAnalyzerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogAnalyzerController : ControllerBase
    {
        private readonly ILogAnalyzer _logAnalyzer;
        public LogAnalyzerController(ILogAnalyzer logAnalyzer)
        {
            _logAnalyzer = logAnalyzer;
        }

        [HttpPost("search")]
        public IActionResult SearchLogsInDirectories([FromBody] List<string> directories)
        {
            try
            {
                List<string> foundLogs = _logAnalyzer.SearchLogsInDirectories(directories);
                return Ok(foundLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("count-unique-errors")]
        public IActionResult CountUniqueErrorsPerLogFile(IFormFile logFile)
        {
            try
            {
                Dictionary<string, int> uniqueErrorsCount = _logAnalyzer.CountUniqueErrorsPerLogFile(logFile);
                return Ok(uniqueErrorsCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("count-duplicated-errors")]
        public IActionResult CountDuplicatedErrorsPerLogFile( IFormFile logFile)
        {
            try
            {
                Dictionary<string, int> duplicatedErrorsCount = _logAnalyzer.CountDuplicatedErrorsPerLogFile(logFile);
                return Ok(duplicatedErrorsCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("delete")]
        public IActionResult DeleteArchiveFromPeriod( ArchiveDeleteRequest request)
        {
            try
            {
            _logAnalyzer.DeleteArchiveFromPeriod(request.StartDate, request.EndDate, request.DirectoryPath);
                return Ok("Archives deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("archive")]
        public IActionResult ArchiveLogsFromPeriod([FromBody] ArchiveLogsRequest request)
        {
            try
            {
                _logAnalyzer.ArchiveLogsFromPeriod(request.StartDate, request.EndDate, request.LogDirectory);
                return Ok("Logs archived successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("delete-logs")]
        public IActionResult DeleteLogsFromPeriod([FromBody] DeleteLogsRequest request)
        {
            try
            {
                _logAnalyzer.DeleteLogsFromPeriod(request.StartDate, request.EndDate, request.LogDirectory);
                return Ok("Logs deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("count-logs")]
        public IActionResult CountTotalAvailableLogsInPeriod([FromBody] CountLogsRequest request)
        {
            try
            {
                var totalLogs = _logAnalyzer.CountTotalAvailableLogsInPeriod(request.StartDate, request.EndDate, request.LogDirectory);
                return Ok($"Total logs in the period: {totalLogs}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("search-by-size")]
        public IActionResult SearchLogsPerSize([FromBody] SearchLogsRequest request)
        {
            try
            {
                var matchedLogs = _logAnalyzer.SearchLogsPerSize(request.DirectoryPath, request.MinSizeKB, request.MaxSizeKB);
                return Ok(matchedLogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("search-by-directory")]
        public IActionResult SearchLogsPerDirectory([FromBody] SearchLogRequest request)
        {
            try
            {
                var matchedLogs = _logAnalyzer.SearchLogsPerDirectory(request.RootDirectory, request.SearchDirectory);
                return Ok(matchedLogs);
            }
            catch (DirectoryNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("upload-logs")]
        public async Task<IActionResult> UploadLogsOnRemoteServer([FromForm] string remoteServerUrl, [FromForm] List<IFormFile> logFiles)
            {
                try
                {
                    await _logAnalyzer.UploadLogsOnRemoteServer(remoteServerUrl, logFiles);
                    return Ok("Logs uploaded successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }
        
    }
}
