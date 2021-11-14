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
            if (File.Exists(ProgramConstants.GamePath + "LaunchVinifera.exe"))
            {
                if (Directory.Exists(ProgramConstants.GamePath + "Debug"))
                {
                    string[] files = Directory.GetFiles(ProgramConstants.GamePath + "Debug", "DEBUG_*");

                    // Find the latest debug file
                    DateTime latestDate = DateTime.MinValue;
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

            return logFileName;
        }
    }
}
