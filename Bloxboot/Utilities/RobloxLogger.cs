using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bloxboot.Utilities
{
    public class RobloxLogger
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static string Logspath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Logs", "Roblox");

        public static string GetLatestLog(string search)
        {
            string logspathExpanded = Environment.ExpandEnvironmentVariables(Logspath);

            string[] allFiles = Directory.GetFiles(logspathExpanded);
            List<string> logFiles = allFiles.Where(file => file.EndsWith(".log")).ToList();

            List<string> sortedFiles = logFiles.OrderByDescending(file => File.GetLastWriteTime(file)).ToList();

            foreach (string filename in sortedFiles)
            {
                try
                {
                    string[] logData = File.ReadAllLines(filename);
                    Array.Reverse(logData);

                    foreach (string entry in logData)
                    {
                        if (entry.Contains(search))
                        {
                            Console.WriteLine($"Found: {entry}");
                            return entry.Trim();
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine($"Error reading file: {filename}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Access denied for file: {filename}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {filename}: {ex.Message}");
                }
            }

            return null;
        }


        private static async Task<Dictionary<string, string>> GetServerLocationInfo(string serverLocation)
        {
            const string cityUrl = "https://ipinfo.io/{0}/city";
            const string regionUrl = "https://ipinfo.io/{0}/region";
            const string countryUrl = "https://ipinfo.io/{0}/country";

            var combinedUrl = string.Format(cityUrl, serverLocation);

            var responses = await Task.WhenAll(
                httpClient.GetStringAsync(string.Format(cityUrl, serverLocation)),
                httpClient.GetStringAsync(string.Format(regionUrl, serverLocation)),
                httpClient.GetStringAsync(string.Format(countryUrl, serverLocation))
            );

            return new Dictionary<string, string>
            {
                ["city"] = responses[0]?.Trim(),
                ["region"] = responses[1]?.Trim(),
                ["country"] = responses[2]?.Trim(),
            };
        }

        private static string GetGameId(string logEntry)
        {
            const string search = "[FLog::Output] ! Joining game";

            if (!string.IsNullOrEmpty(logEntry) && logEntry.Contains(search))
            {
                var match = Regex.Match(logEntry, @"game '(.*?)'");

                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }


        public static string CheckActivity()
        {
            const string join = "[FLog::Output] ! Joining game";
            const string leave = "[FLog::Network] Time to disconnect replication data";

            string joinLogs = GetLatestLog(join);
            string leaveLogs = GetLatestLog(leave);

            if (!string.IsNullOrEmpty(joinLogs) && !string.IsNullOrEmpty(leaveLogs))
            {
                DateTime? joinTime = ParseLogDateTime(joinLogs);
                DateTime? leaveTime = ParseLogDateTime(leaveLogs);

                Console.WriteLine($"Join time: {joinTime}");
                Console.WriteLine($"Leave time: {leaveTime}");

                if (joinTime != null && leaveTime != null)
                {
                    if (leaveTime < joinTime)
                    {
                        return "You are in a game";
                    }
                    else
                    {
                        return "you are not in a game";
                    }
                }
            }

            return "Failed to determine activity."; 
        }


        private static DateTime? ParseLogDateTime(string logEntry)
        {
            var match = Regex.Match(logEntry, @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{3})?Z");

            if (match.Success)
            {
                if (DateTime.TryParseExact(match.Value, "yyyy-MM-ddTHH:mm:ss.fffZ", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var dateTime))
                {
                    return dateTime;
                }
                else if (DateTime.TryParseExact(match.Value, "yyyy-MM-ddTHH:mm:ssZ", null, System.Globalization.DateTimeStyles.AssumeUniversal, out dateTime))
                {
                    return dateTime;
                }
            }

            return null;
        }
    }
}
