using System;
using System.IO;

namespace ClientCore.Statistics
{
    /// <summary>
    /// Simple static helper class with a function that finds the game log file.
    /// Debug log files don't have a static name with the Vinifera engine extension,
    /// while without Vinifera the name of the log file is constant.
    /// </summary>
    public static class LogFileFinder
    {
        public static string GetLogFilePath()
        {
            string logFileName = ClientConfiguration.Instance.StatisticsLogFileName;
            DateTime latestDate = DateTime.MinValue;

            if (File.Exists(ProgramConstants.GamePath + "LaunchVinifera.exe") || File.Exists(ProgramConstants.GamePath + "LaunchVinifera.dat"))
            {
                if (Directory.Exists(ProgramConstants.GamePath + "Debug"))
                {
                    string[] files = Directory.GetFiles(ProgramConstants.GamePath + "Debug", "DEBUG_*");

                    // Find the latest debug file
                    foreach (string viniferaDebugLogPath in files)
                    {
                        DateTime fileWriteTime = File.GetLastWriteTime(viniferaDebugLogPath);
                        if (fileWriteTime > latestDate)
                        {
                            latestDate = fileWriteTime;
                            logFileName = "Debug/" + Path.GetFileName(viniferaDebugLogPath);
                        }
                    }
                }
            }

            // Check that the date of the latest file was reasonable (like, written to within the past 5 minutes).
            // This is to make sure that we don't parse an outdated log file in error conditions where
            // the game has not generated a log at all.
            if (latestDate < DateTime.Now.AddMinutes(-5.0))
            {
                return null;
            }

            return logFileName;
        }
    }
}
