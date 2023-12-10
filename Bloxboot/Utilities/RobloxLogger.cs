using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bloxboot.Utilities
{
    public class RobloxLogger
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static string Logspath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Logs", "Roblox");

        private static string GetLatestLog(string search)
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

        private static string GetGameId()
        {
            const string search = "[FLog::Output] ! Joining game";
            string latestLogEntry = GetLatestLog(search);
            return latestLogEntry?.Split(" ").LastOrDefault();
        }

        private static async Task<Dictionary<string, string>> GetServerLocationInfo(string serverLocation)
        {
            const string cityUrl = "https://ipinfo.io/{0}/city";
            const string regionUrl = "https://ipinfo.io/{0}/region";
            const string countryUrl = "https://ipinfo.io/{0}/country";

            var cityResponse = await httpClient.GetStringAsync(string.Format(cityUrl, serverLocation));
            var regionResponse = await httpClient.GetStringAsync(string.Format(regionUrl, serverLocation));
            var countryResponse = await httpClient.GetStringAsync(string.Format(countryUrl, serverLocation));

            return new Dictionary<string, string>
            {
                ["city"] = cityResponse?.Trim(),
                ["region"] = regionResponse?.Trim(),
                ["country"] = countryResponse?.Trim(),
            };
        }

        private static string CheckActivity()
        {
            const string join = "[FLog::Output] ! Joining game";
            const string leave = "[FLog::Network] Time to disconnect replication data";

            string joinLogs = GetLatestLog(join);
            string leaveLogs = GetLatestLog(leave);

            if (leaveLogs != null && joinLogs != null)
            {
                return DateTime.Parse(leaveLogs) > DateTime.Parse(joinLogs)
                    ? "You are not in a game."
                    : $"You are in a game. Game ID: {GetGameId()}\nServer Location: {GetServerLocationInfo(leaveLogs).Result["city"]}, {GetServerLocationInfo(leaveLogs).Result["region"]}, {GetServerLocationInfo(leaveLogs).Result["country"]}";
            }

            if (joinLogs != null)
            {
                return $"You are in a game. Game ID: {GetGameId()}\nServer Location: {GetServerLocationInfo(joinLogs).Result["city"]}, {GetServerLocationInfo(joinLogs).Result["region"]}, {GetServerLocationInfo(joinLogs).Result["country"]}";
            }

            return "No relevant logs found.";
        }

        private static void SendNotification(string mode, Dictionary<string, string> data = null)
        {
            string title = "Roblox Notification";
            string message = "";

            if (mode == "ServerInfo")
            {
                if (data != null)
                {
                    message = $"Server Location: {data["city"]}, {data["region"]}, {data["country"]}";
                }
                else
                {
                    message = "Failed to retrieve server location information.";
                }
            }
            else if (mode == "GameInfo")
            {
                if (data != null)
                {
                    message = $"You are in a game. Game ID: {data["game_id"]}";
                    var locationInfo = GetServerLocationInfo(data["leave_logs"]).Result;
                    if (locationInfo != null)
                    {
                        message += $"\nServer Location: {locationInfo["city"]}, {locationInfo["region"]}, {locationInfo["country"]}";
                    }
                    else
                    {
                        message += "\nFailed to retrieve server location information.";
                    }
                }
                else
                {
                    message = "You are not in a game.";
                }
            }

            Console.WriteLine($"{title}: {message}");
        }
    }
}
