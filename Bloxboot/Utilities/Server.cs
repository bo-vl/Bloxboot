using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bloxboot.Utilities
{
    public class Server
    {
        public virtual string Logspath => "/Library/Logs/Roblox";

        public string GetLatestLog(string searchString)
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
                        if (entry.Contains(searchString))
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
    }
}
