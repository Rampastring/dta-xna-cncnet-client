using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using ClientCore;
using Rampastring.Tools;
using ClientCore.INIProcessing;
using System.Threading;

namespace ClientGUI
{
    /// <summary>
    /// A static class used for controlling the launching and exiting of the game executable.
    /// </summary>
    public static class GameProcessLogic
    {
        public static event Action GameProcessStarted;

        public static event Action GameProcessStarting;

        public static event Action GameProcessExited;

        public static GameSessionManager GameSessionManager { get; private set; }

        public static bool INIPreprocessingFailed = false;

        public static bool IsGameRunning = false;

        /// <summary>
        /// Starts the main game process.
        /// </summary>
        public static void StartGameProcess(GameSessionManager sessionManager)
        {
            GameSessionManager = sessionManager;

            Logger.Log("About to launch main game executable.");

            INIPreprocessingFailed = false;

            // Re-process INI files
            PreprocessorBackgroundTask.Instance.Run();

            // Wait for INI preprocessing to complete. Time-out if it seems to have stalled.
            // TODO ideally this should be handled in the UI so the client doesn't appear just frozen for the user.
            int waitTimes = 0;
            while (PreprocessorBackgroundTask.Instance.IsRunning)
            {
                Thread.Sleep(1000);
                waitTimes++;
                if (waitTimes > 10)
                {
                    Logger.Log("INI preprocessing timed out (10 seconds) when launching game!");

                    INIPreprocessingFailed = true;
                    PreprocessorBackgroundTask.Instance.LogException();
                    PreprocessorBackgroundTask.Instance.LogState();
                    break;
                }
            }

            PreprocessorBackgroundTask.Instance.LogException();

            OSVersion osVersion = ClientConfiguration.Instance.GetOperatingSystemVersion();

            string gameExecutableName;
            string additionalExecutableName = string.Empty;

            if (osVersion == OSVersion.UNIX)
                gameExecutableName = ClientConfiguration.Instance.UnixGameExecutableName;
            else
            {
                string launcherExecutableName = ClientConfiguration.Instance.GameLauncherExecutableName;
                if (string.IsNullOrEmpty(launcherExecutableName))
                {
                    gameExecutableName = ClientConfiguration.Instance.GetGameExecutableName();
                }
                else
                {
                    gameExecutableName = launcherExecutableName;
                    additionalExecutableName = ClientConfiguration.Instance.GetGameExecutableName();
                }
            }

            string extraCommandLine = ClientConfiguration.Instance.ExtraExeCommandLineParameters;

            GameProcessStarting?.Invoke();

            Process dtaProcess = new Process();
            dtaProcess.StartInfo.FileName = gameExecutableName;
            dtaProcess.StartInfo.UseShellExecute = false;
            if (!string.IsNullOrEmpty(extraCommandLine))
                dtaProcess.StartInfo.Arguments = additionalExecutableName + " " + extraCommandLine;
            else
                dtaProcess.StartInfo.Arguments = additionalExecutableName;
            dtaProcess.EnableRaisingEvents = true;
            dtaProcess.Exited += new EventHandler(Process_Exited);
            Logger.Log("Launch executable: " + dtaProcess.StartInfo.FileName);
            Logger.Log("Launch arguments: " + dtaProcess.StartInfo.Arguments);
            try
            {
                dtaProcess.Start();
                Logger.Log("GameProcessLogic: Process started.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error launching " + gameExecutableName + ": " + ex.Message);
                MessageBox.Show("Error launching " + gameExecutableName + ". Please check that your anti-virus isn't blocking the CnCNet Client. " +
                    "You can also try running the client as an administrator." + Environment.NewLine + Environment.NewLine + "You are unable to participate in this match." +
                    Environment.NewLine + Environment.NewLine + "Returned error: " + ex.Message,
                    "Error launching game", MessageBoxButtons.OK);
                Process_Exited(dtaProcess, EventArgs.Empty);
                return;
            }

            IsGameRunning = true;
            GameProcessStarted?.Invoke();

            Logger.Log("Waiting for qres.dat or " + gameExecutableName + " to exit.");
        }

        static void Process_Exited(object sender, EventArgs e)
        {
            Logger.Log("GameProcessLogic: Process exited.");
            IsGameRunning = false;
            Process proc = (Process)sender;
            proc.Exited -= Process_Exited;
            proc.Dispose();
            GameSessionManager.EndSession();
            GameProcessExited?.Invoke();
        }
    }
}
