using Microsoft.Xna.Framework;
using Rampastring.XNAUI.XNAControls;
using Rampastring.Tools;
using System;
using ClientCore;
using Rampastring.XNAUI;
using ClientGUI;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DTAClient.DXGUI
{
    /// <summary>
    /// Displays a dialog in the client when a game is in progress.
    /// Also enables power-saving (lowers FPS) while a game is in progress,
    /// and performs various operations on game start and exit.
    /// </summary>
    public class GameInProgressWindow : XNAPanel
    {
        private const double FPS = 60.0;
        private const double POWER_SAVING_FPS = 5.0;

        public GameInProgressWindow(WindowManager windowManager) : base(windowManager)
        {
        }

        private bool initialized = false;
        private bool nativeCursorUsed = false;
        private bool deletingLogFilesFailed = false;

        public override void Initialize()
        {
            if (initialized)
                throw new InvalidOperationException("GameInProgressWindow cannot be initialized twice!");

            initialized = true;

            BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 1, 1);
            PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;
            DrawBorders = false;
            ClientRectangle = new Rectangle(0, 0, WindowManager.RenderResolutionX, WindowManager.RenderResolutionY);

            XNAWindow window = new XNAWindow(WindowManager);

            window.Name = "GameInProgressWindow";
            window.BackgroundTexture = AssetLoader.LoadTexture("gameinprogresswindowbg.png");
            window.ClientRectangle = new Rectangle(0, 0, 200, 100);

            XNALabel explanation = new XNALabel(WindowManager);
            explanation.Text = "A game is in progress.";

            AddChild(window);

            window.AddChild(explanation);

            base.Initialize();

            GameProcessLogic.GameProcessStarted += SharedUILogic_GameProcessStarted;
            GameProcessLogic.GameProcessExited += SharedUILogic_GameProcessExited;

            explanation.CenterOnParent();

            window.CenterOnParent();

            Game.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / UserINISettings.Instance.ClientFPS);

            Disable();
        }

        private void SharedUILogic_GameProcessStarted()
        {
            try
            {
                File.Delete(ProgramConstants.GamePath + "EXCEPT.TXT");

                for (int i = 0; i < 8; i++)
                    File.Delete(ProgramConstants.GamePath + "SYNC" + i + ".TXT");

                deletingLogFilesFailed = false;
            }
            catch (IOException ex)
            {
                Logger.Log("Exception when deleting error log files! Message: " + ex.Message);
                deletingLogFilesFailed = true;
            }

            Enable();
            WindowManager.Cursor.Visible = false;
            nativeCursorUsed = Game.IsMouseVisible;
            Game.IsMouseVisible = false;
            ProgramConstants.IsInGame = true;
            Game.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / POWER_SAVING_FPS);
            if (UserINISettings.Instance.MinimizeWindowsOnGameStart)
                WindowManager.MinimizeWindow();
        }

        private void SharedUILogic_GameProcessExited()
        {
            AddCallback(new Action(HandleGameProcessExited), null);
        }

        private void HandleGameProcessExited()
        {
            Logger.Log("GameInProgressWindow.HandleGameProcessExited");

            Disable();
            if (nativeCursorUsed)
                Game.IsMouseVisible = true;
            else
                WindowManager.Cursor.Visible = true;
            ProgramConstants.IsInGame = false;
            Game.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / UserINISettings.Instance.ClientFPS);
            if (UserINISettings.Instance.MinimizeWindowsOnGameStart)
                WindowManager.MaximizeWindow();

            UserINISettings.Instance.ReloadSettings();

            if (UserINISettings.Instance.BorderlessWindowedClient)
            {
                // Hack: Re-set graphics mode
                // Windows resizes our window if we're in fullscreen mode and
                // the in-game resolution is lower than the user's desktop resolution.
                // After the game exits, Windows doesn't properly re-size our window
                // back to cover the entire screen, which causes graphics to get
                // stretched and also messes up input handling since the window manager
                // still thinks it's using the original resolution.
                // Re-setting the graphics mode fixes it.
                GameClass.SetGraphicsMode(WindowManager);
            }

            DateTime dtn = DateTime.Now;

            if (deletingLogFilesFailed)
                return;

            CopyErrorLog(ProgramConstants.ClientUserFilesPath + "GameCrashLogs", "EXCEPT.TXT", dtn);
            CopySyncErrorLogs(ProgramConstants.ClientUserFilesPath + "SyncErrorLogs", dtn);
        }

        /// <summary>
        /// Attempts to copy a general error log from game directory to another directory.
        /// </summary>
        /// <param name="directory">Directory to copy error log to.</param>
        /// <param name="filename">Filename of the error log.</param>
        /// <param name="dateTime">Time to to apply as a timestamp to filename. Set to null to not apply a timestamp.</param>
        /// <returns>True if error log was copied, false otherwise.</returns>
        private bool CopyErrorLog(string directory, string filename, DateTime? dateTime)
        {
            bool copied = false;

            try
            {
                if (File.Exists(ProgramConstants.GamePath + filename))
                {
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    Logger.Log("The game crashed! Copying " + filename + " file.");

                    string timeStamp = dateTime.HasValue ? dateTime.Value.ToString("_yyyy_MM_dd_HH_mm") : "";

                    string filenameCopy = Path.GetFileNameWithoutExtension(filename) +
                        timeStamp + Path.GetExtension(filename);

                    File.Copy(ProgramConstants.GamePath + filename, directory + "/" + filenameCopy);
                    copied = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while checking for " + filename + " file. Message: " + ex.Message);
            }
            return copied;
        }

        /// <summary>
        /// Attempts to copy sync error logs from game directory to another directory.
        /// </summary>
        /// <param name="directory">Directory to copy sync error logs to.</param>
        /// <param name="dateTime">Time to to apply as a timestamp to filename. Set to null to not apply a timestamp.</param>
        /// <returns>True if any sync logs were copied, false otherwise.</returns>
        private bool CopySyncErrorLogs(string directory, DateTime? dateTime)
        {
            bool copied = false;

            try
            {
                for (int i = 0; i < 8; i++)
                {
                    string filename = "SYNC" + i + ".TXT";

                    if (File.Exists(ProgramConstants.GamePath + filename))
                    {
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        Logger.Log("There was a sync error! Copying file " + filename);

                        string timeStamp = dateTime.HasValue ? dateTime.Value.ToString("_yyyy_MM_dd_HH_mm") : "";

                        string filenameCopy = Path.GetFileNameWithoutExtension(filename) +
                            timeStamp + Path.GetExtension(filename);

                        File.Copy(ProgramConstants.GamePath + filename, directory + "/" + filenameCopy);
                        copied = true;
                        File.Delete(ProgramConstants.GamePath + filename);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("An error occured while checking for SYNCX.TXT files. Message: " + ex.Message);
            }
            return copied;
        }
    }
}
