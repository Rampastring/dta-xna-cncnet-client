using ClientCore;
using ClientGUI;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace DTAConfig.OptionPanels
{
    enum GameDisplayMode
    {
        BorderlessWindowed = 0,
        NativeFullscreen = 1,
        Windowed = 2,
        Count
    }

    class DisplayOptionsPanel : XNAOptionsPanel
    {
        private const int DRAG_DISTANCE_DEFAULT = 4;
        private const int ORIGINAL_RESOLUTION_WIDTH = 640;
        private const string RENDERERS_INI = "Renderers.ini";

        public DisplayOptionsPanel(WindowManager windowManager, UserINISettings iniSettings)
            : base(windowManager, iniSettings)
        {
        }

        private XNAClientDropDown ddDisplayMode;
        private XNAClientDropDown ddIngameResolution;
        private XNAClientDropDown ddScaleFactor;
        private XNAClientDropDown ddDetailLevel;
        private XNAClientDropDown ddRenderer;
        private XNAClientCheckBox chkBackBufferInVRAM;
        private XNAClientCheckBox chkStretchMovies;
        private XNAClientCheckBox chkClassicMessageListPosition;
        private XNAClientPreferredItemDropDown ddClientResolution;
        private XNAClientCheckBox chkBorderlessClient;
        private XNAClientDropDown ddClientTheme;

        private List<DirectDrawWrapper> renderers;
        private List<ScreenResolution> resolutions;

        private string defaultRenderer;
        private DirectDrawWrapper selectedRenderer = null;

        private XNALabel lblCompatibilityFixes;
        private XNALabel lblGameCompatibilityFix;
        private XNALabel lblMapEditorCompatibilityFix;
        private XNAClientButton btnGameCompatibilityFix;
        private XNAClientButton btnMapEditorCompatibilityFix;

        private bool GameCompatFixInstalled = false;
        private bool FinalSunCompatFixInstalled = false;
        private bool GameCompatFixDeclined = false;


        public override void Initialize()
        {
            base.Initialize();

            Name = "DisplayOptionsPanel";

            var clientConfig = ClientConfiguration.Instance;

            var lblDisplayMode = new XNALabel(WindowManager);
            lblDisplayMode.Name = nameof(lblDisplayMode);
            lblDisplayMode.X = 12;
            lblDisplayMode.Y = 14;
            lblDisplayMode.Text = "Display Mode:";
            AddChild(lblDisplayMode);

            ddDisplayMode = new XNAClientDropDown(WindowManager);
            ddDisplayMode.Name = nameof(ddDisplayMode);
            ddDisplayMode.X = 124;
            ddDisplayMode.Y = lblDisplayMode.Y - 2;
            ddDisplayMode.Width = 120;
            ddDisplayMode.Height = 19;
            AddChild(ddDisplayMode);
            ddDisplayMode.AddItem("Borderless Windowed");
            ddDisplayMode.AddItem("Native Fullscreen");
            ddDisplayMode.AddItem("Windowed");
            ddDisplayMode.ToolTip.Text = "Defines how the in-game window is displayed." + Environment.NewLine + Environment.NewLine +
                "Borderless Windowed is recommended for most systems," + Environment.NewLine + "" +
                "but older systems might have higher performance on Native Fullscreen.";
            ddDisplayMode.SelectedIndexChanged += (s, e) => RefreshScaleFactors(false);

            var lblIngameResolution = new XNALabel(WindowManager);
            lblIngameResolution.Name = "lblIngameResolution";
            lblIngameResolution.ClientRectangle = new Rectangle(lblDisplayMode.X, ddDisplayMode.Bottom + 16, 0, 0);
            lblIngameResolution.Text = "In-Game Resolution:";

            ddIngameResolution = new XNAClientDropDown(WindowManager);
            ddIngameResolution.Name = "ddIngameResolution";
            ddIngameResolution.ClientRectangle = new Rectangle(
                lblIngameResolution.Right + 12,
                lblIngameResolution.Y - 2, ddDisplayMode.Width, ddDisplayMode.Height);
            ddIngameResolution.SelectedIndexChanged += (s, e) => RefreshScaleFactors();

            resolutions = GetResolutions(clientConfig.MinimumIngameWidth, 
                clientConfig.MinimumIngameHeight,
                clientConfig.MaximumIngameWidth, clientConfig.MaximumIngameHeight);

            resolutions.Sort();

            foreach (var res in resolutions)
                ddIngameResolution.AddItem(new XNADropDownItem() { Text = res.ToString(), Tag = res });

            var lblScaleFactor = new XNALabel(WindowManager);
            lblScaleFactor.Name = nameof(lblScaleFactor);
            lblScaleFactor.X = lblDisplayMode.X;
            lblScaleFactor.Y = ddIngameResolution.Bottom + 16;
            lblScaleFactor.Text = "Scale Factor:";
            AddChild(lblScaleFactor);

            ddScaleFactor = new XNAClientDropDown(WindowManager);
            ddScaleFactor.Name = nameof(ddScaleFactor);
            ddScaleFactor.X = ddDisplayMode.X;
            ddScaleFactor.Y = lblScaleFactor.Y - 2;
            ddScaleFactor.Width = ddDisplayMode.Width;
            ddScaleFactor.Height = ddDisplayMode.Height;
            AddChild(ddScaleFactor);

            var lblDetailLevel = new XNALabel(WindowManager);
            lblDetailLevel.Name = "lblDetailLevel";
            lblDetailLevel.ClientRectangle = new Rectangle(lblIngameResolution.X,
                ddScaleFactor.Bottom + 16, 0, 0);
            lblDetailLevel.Text = "Detail Level:";

            ddDetailLevel = new XNAClientDropDown(WindowManager);
            ddDetailLevel.Name = "ddDetailLevel";
            ddDetailLevel.ClientRectangle = new Rectangle(
                ddIngameResolution.X,
                lblDetailLevel.Y - 2,
                ddIngameResolution.Width, 
                ddIngameResolution.Height);
            ddDetailLevel.AddItem("Low");
            ddDetailLevel.AddItem("Medium");
            ddDetailLevel.AddItem("High");

            var lblRenderer = new XNALabel(WindowManager);
            lblRenderer.Name = "lblRenderer";
            lblRenderer.ClientRectangle = new Rectangle(lblDetailLevel.X,
                ddDetailLevel.Bottom + 16, 0, 0);
            lblRenderer.Text = "Renderer:";

            ddRenderer = new XNAClientDropDown(WindowManager);
            ddRenderer.Name = "ddRenderer";
            ddRenderer.ClientRectangle = new Rectangle(
                ddDetailLevel.X,
                lblRenderer.Y - 2,
                ddDetailLevel.Width,
                ddDetailLevel.Height);
            ddRenderer.SelectedIndexChanged += (s, e) => RefreshScaleFactors();

            GetRenderers();

            var localOS = ClientConfiguration.Instance.GetOperatingSystemVersion();

            foreach (var renderer in renderers)
            {
                if (renderer.IsCompatibleWithOS(localOS) && !renderer.Hidden)
                {
                    ddRenderer.AddItem(new XNADropDownItem()
                    {
                        Text = renderer.UIName,
                        Tag = renderer
                    });
                }
            }

            chkBackBufferInVRAM = new XNAClientCheckBox(WindowManager);
            chkBackBufferInVRAM.Name = "chkBackBufferInVRAM";
            chkBackBufferInVRAM.ClientRectangle = new Rectangle(
                lblDetailLevel.X,
                ddRenderer.Bottom + 12, 0, 0);
            chkBackBufferInVRAM.Text = "Back Buffer in Video Memory" + Environment.NewLine +
                "(lower performance, but is" + Environment.NewLine + "necessary on some systems)";

            chkStretchMovies = new XNAClientCheckBox(WindowManager);
            chkStretchMovies.Name = "chkStretchMovies";
            chkStretchMovies.ClientRectangle = new Rectangle(
                lblDetailLevel.X,
                chkBackBufferInVRAM.Bottom + 24, 0, 0);
            chkStretchMovies.Text = "Stretch Movies";

            chkClassicMessageListPosition = new XNAClientCheckBox(WindowManager);
            chkClassicMessageListPosition.Name = "chkClassicMessageListPosition";
            chkClassicMessageListPosition.ClientRectangle = new Rectangle(
                lblDetailLevel.X,
                chkStretchMovies.Bottom + 24, 0, 0);
            chkClassicMessageListPosition.Text = "Classic Message List Position";

            var lblClientResolution = new XNALabel(WindowManager);
            lblClientResolution.Name = "lblClientResolution";
            lblClientResolution.ClientRectangle = new Rectangle(
                285, 14, 0, 0);
            lblClientResolution.Text = "Client Resolution:";

            ddClientResolution = new XNAClientPreferredItemDropDown(WindowManager);
            ddClientResolution.Name = "ddClientResolution";
            ddClientResolution.ClientRectangle = new Rectangle(
                lblClientResolution.Right + 12,
                lblClientResolution.Y - 2,
                Width - (lblClientResolution.Right + 24),
                ddIngameResolution.Height);
            ddClientResolution.AllowDropDown = false;
            ddClientResolution.PreferredItemLabel = "(recommended)";

            var screenBounds = Screen.PrimaryScreen.Bounds;

            resolutions = GetResolutions(800, 600,
                screenBounds.Width, screenBounds.Height);

            // Add "optimal" client resolutions for windowed mode
            // if they're not supported in fullscreen mode

            AddResolutionIfFitting(1024, 600, resolutions);
            AddResolutionIfFitting(1024, 720, resolutions);
            AddResolutionIfFitting(1280, 600, resolutions);
            AddResolutionIfFitting(1280, 720, resolutions);
            AddResolutionIfFitting(1280, 768, resolutions);
            AddResolutionIfFitting(1280, 800, resolutions);

            resolutions.Sort();

            foreach (var res in resolutions)
            {
                var item = new XNADropDownItem();
                item.Text = res.ToString();
                item.Tag = res;
                ddClientResolution.AddItem(item);
            }

            // So we add the optimal resolutions to the list, sort it and then find
            // out the optimal resolution index - it's inefficient, but works

            string[] recommendedResolutions = clientConfig.RecommendedResolutions;

            foreach (string resolution in recommendedResolutions)
            {
                string trimmedresolution = resolution.Trim();
                int index = resolutions.FindIndex(res => res.ToString() == trimmedresolution);
                if (index > -1)
                    ddClientResolution.PreferredItemIndexes.Add(index);
            }

            chkBorderlessClient = new XNAClientCheckBox(WindowManager);
            chkBorderlessClient.Name = "chkBorderlessClient";
            chkBorderlessClient.ClientRectangle = new Rectangle(
                lblClientResolution.X,
                lblScaleFactor.Y, 0, 0);
            chkBorderlessClient.Text = "Fullscreen Client";
            chkBorderlessClient.CheckedChanged += ChkBorderlessMenu_CheckedChanged;
            chkBorderlessClient.Checked = true;

            var lblClientTheme = new XNALabel(WindowManager);
            lblClientTheme.Name = "lblClientTheme";
            lblClientTheme.ClientRectangle = new Rectangle(
                lblClientResolution.X,
                lblIngameResolution.Y, 0, 0);
            lblClientTheme.Text = "Client Theme:";

            ddClientTheme = new XNAClientDropDown(WindowManager);
            ddClientTheme.Name = "ddClientTheme";
            ddClientTheme.ClientRectangle = new Rectangle(
                ddClientResolution.X,
                ddIngameResolution.Y,
                ddClientResolution.Width,
                ddIngameResolution.Height);

            int themeCount = ClientConfiguration.Instance.ThemeCount;

            for (int i = 0; i < themeCount; i++)
                ddClientTheme.AddItem(ClientConfiguration.Instance.GetThemeInfoFromIndex(i)[0]);

            AddCompatibilityFixControls();

            AddChild(chkBackBufferInVRAM);
            AddChild(chkStretchMovies);
            AddChild(chkClassicMessageListPosition);
            AddChild(chkBorderlessClient);
            AddChild(lblClientTheme);
            AddChild(ddClientTheme);
            AddChild(lblClientResolution);
            AddChild(ddClientResolution);
            AddChild(lblRenderer);
            AddChild(ddRenderer);
            AddChild(lblDetailLevel);
            AddChild(ddDetailLevel);
            AddChild(lblIngameResolution);
            AddChild(ddIngameResolution);
        }

        private void RefreshScaleFactors(bool applyRecommendedFactor = true)
        {
            double selectedScaleFactor = 1.0;

            if (ddScaleFactor.SelectedItem != null)
                selectedScaleFactor = (double)ddScaleFactor.SelectedItem.Tag;

            ddScaleFactor.SelectedIndex = -1;
            ddScaleFactor.Items.Clear();

            DirectDrawWrapper renderer = null;
            if (ddRenderer.SelectedItem != null)
                renderer = (DirectDrawWrapper)ddRenderer.SelectedItem.Tag;

            // Check for error conditions. In case one exists, gray out the scaling option.
            if (resolutions.Count <= 0 || ddIngameResolution.SelectedItem == null ||
                ddRenderer.SelectedItem == null || !renderer.SupportsScaling ||
                (ddDisplayMode.SelectedIndex == (int)GameDisplayMode.Windowed && renderer.NoWindowedModeScaling) ||
                (ddDisplayMode.SelectedIndex == (int)GameDisplayMode.BorderlessWindowed && renderer.NoBorderlessModeScaling))
            {
                if (renderer != null && ddDisplayMode.SelectedIndex == (int)GameDisplayMode.Windowed && renderer.NoWindowedModeScaling)
                    ddScaleFactor.ToolTip.Text = "The selected renderer does not support scaling in windowed mode.";
                else
                    ddScaleFactor.ToolTip.Text = "The selected renderer does not support scaling.";

                ddScaleFactor.Items.Add(new XNADropDownItem() { Text = "1.0x", Tag = 1.0 });
                ddScaleFactor.SelectedIndex = 0;
                ddScaleFactor.AllowDropDown = false;
                return;
            }

            ddScaleFactor.ToolTip.Text = "Can be used to zoom the game view." + Environment.NewLine + Environment.NewLine + 
                "If you select an integer-scale factor (such as 2x or 3x)," + Environment.NewLine + 
                "the game is integer-scaled for crisp graphics if supported by the selected renderer.";

            ddScaleFactor.AllowDropDown = true;

            int startResX;
            int startResY;

            var screenResolution = (ScreenResolution)ddIngameResolution.SelectedItem.Tag;
            startResX = screenResolution.Width;
            startResY = screenResolution.Height;

            double recommendedScaleFactor = 0.0;
            defaultScaleFactors.TryGetValue(new Point(startResX, startResY), out recommendedScaleFactor);

            double scaleFactor = 1.0;
            const double scaleFactorIncrease = 0.25;
            int resX = startResX;
            int resY = startResY;

            // If the current scale factor is 1.0, invalidate it so the code below can select a better default.
            if (applyRecommendedFactor && selectedScaleFactor == 1.0)
                selectedScaleFactor = 0.0;

            while (resX >= ClientConfiguration.Instance.MinimumIngameWidth &&
                resY >= ClientConfiguration.Instance.MinimumIngameHeight)
            {
                ddScaleFactor.AddItem(new XNADropDownItem()
                { 
                    Text = scaleFactor.ToString(CultureInfo.InvariantCulture) + "x" + (recommendedScaleFactor == scaleFactor ? " (Recommended)" : ""),
                    Tag = scaleFactor
                });

                if (selectedScaleFactor == scaleFactor)
                    ddScaleFactor.SelectedIndex = ddScaleFactor.Items.Count - 1;

                scaleFactor += scaleFactorIncrease;
                resX = (int)(startResX / scaleFactor);
                resY = (int)(startResY / scaleFactor);
            }

            if (ddScaleFactor.SelectedIndex < 0)
            {
                int recommendedIndex = ddScaleFactor.Items.FindIndex(ddi => (double)ddi.Tag == recommendedScaleFactor);

                if (recommendedIndex > -1)
                    ddScaleFactor.SelectedIndex = recommendedIndex;
                else
                    ddScaleFactor.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Adds a screen resolution to a list of resolutions if it fits on the screen.
        /// Checks if the resolution already exists before adding it.
        /// </summary>
        /// <param name="width">The width of the new resolution.</param>
        /// <param name="height">The height of the new resolution.</param>
        /// <param name="resolutions">A list of screen resolutions.</param>
        private void AddResolutionIfFitting(int width, int height, List<ScreenResolution> resolutions)
        {
            if (resolutions.Find(res => res.Width == width && res.Height == height) != null)
                return;

            var screenBounds = Screen.PrimaryScreen.Bounds;

            if (screenBounds.Width >= width && screenBounds.Height >= height)
            {
                resolutions.Add(new ScreenResolution(width, height));
            }
        }

		private void GetRenderers()
		{
			renderers = new List<DirectDrawWrapper>();

			var renderersIni = new IniFile(ProgramConstants.GetBaseResourcePath() + RENDERERS_INI);

			var keys = renderersIni.GetSectionKeys("Renderers");
			if (keys == null)
				throw new ClientConfigurationException("[Renderers] not found from Renderers.ini!");

			foreach (string key in keys)
			{
				string internalName = renderersIni.GetStringValue("Renderers", key, string.Empty);

				var ddWrapper = new DirectDrawWrapper(internalName, renderersIni);
				renderers.Add(ddWrapper);
			}

			OSVersion osVersion = ClientConfiguration.Instance.GetOperatingSystemVersion();

			defaultRenderer = renderersIni.GetStringValue("DefaultRenderer", osVersion.ToString(), string.Empty);

			if (defaultRenderer == null)
				throw new ClientConfigurationException("Invalid or missing default renderer for operating system: " + osVersion);


			string renderer = UserINISettings.Instance.Renderer;

			selectedRenderer = renderers.Find(r => r.InternalName == renderer);

			if (selectedRenderer == null)
				selectedRenderer = renderers.Find(r => r.InternalName == defaultRenderer);

			if (selectedRenderer == null)
				throw new ClientConfigurationException("Missing renderer: " + renderer);

            GameProcessLogic.UseQres = selectedRenderer.UseQres;
            GameProcessLogic.SingleCoreAffinity = selectedRenderer.SingleCoreAffinity;
		}

        /// <summary>
        /// Loads settings.
        /// </summary>
        public void PostInit()
        {
            Load();

            // if (!GameCompatFixInstalled && !GameCompatFixDeclined)
            // {
            //     string defaultGame = ClientConfiguration.Instance.LocalGame;
            // 
            //     var messageBox = XNAMessageBox.ShowYesNoDialog(WindowManager, "New Compatibility Fix",
            //         "A performance-enhancing compatibility fix for modern Windows versions" + Environment.NewLine +
            //         "has been included in this version of " + defaultGame + ". Enabling it requires" + Environment.NewLine +
            //         "administrative priveleges. Would you like to install the compatibility fix?" + Environment.NewLine + Environment.NewLine + 
            //         "You'll always be able to install or uninstall the compatibility fix later from the options menu.");
            //     messageBox.YesClickedAction = MessageBox_YesClicked;
            //     messageBox.NoClickedAction = MessageBox_NoClicked;
            // }
        }

        private void MessageBox_NoClicked(XNAMessageBox messageBox)
        {
            // Set compatibility fix declined flag in registry
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tiberian Sun Client");

                try
                {
                    regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                    regKey = regKey.CreateSubKey("Tiberian Sun Client");
                    regKey.SetValue("TSCompatFixDeclined", "Yes");
                }
                catch (Exception ex)
                {
                    Logger.Log("Setting TSCompatFixDeclined failed! Returned error: " + ex.Message);
                }
            }
            catch { }
        }

        private void MessageBox_YesClicked(XNAMessageBox messageBox)
        {
            BtnGameCompatibilityFix_LeftClick(messageBox, EventArgs.Empty);
        }

        private void BtnGameCompatibilityFix_LeftClick(object sender, EventArgs e)
        {
            if (GameCompatFixInstalled)
            {
                try
                {
                    Process sdbinst = Process.Start("sdbinst.exe", "-q -n \"TS Compatibility Fix\"");

                    sdbinst.WaitForExit();

                    Logger.Log("DTA/TI/TS Compatibility Fix succesfully uninstalled.");
                    XNAMessageBox.Show(WindowManager, "Compatibility Fix Uninstalled",
                        "The DTA/TI/TS Compatibility Fix has been succesfully uninstalled.");

                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                    regKey = regKey.CreateSubKey("Tiberian Sun Client");
                    regKey.SetValue("TSCompatFixInstalled", "No");

                    btnGameCompatibilityFix.Text = "Enable";

                    GameCompatFixInstalled = false;
                }
                catch (Exception ex)
                {
                    Logger.Log("Uninstalling DTA/TI/TS Compatibility Fix failed. Error message: " + ex.Message);
                    XNAMessageBox.Show(WindowManager, "Uninstalling Compatibility Fix Failed",
                        "Uninstalling DTA/TI/TS Compatibility Fix failed. Returned error: " + ex.Message);
                }

                return;
            }

            try
            {
                Process sdbinst = Process.Start("sdbinst.exe", "-q \"" + ProgramConstants.GamePath + "Resources/compatfix.sdb\"");

                sdbinst.WaitForExit();

                Logger.Log("DTA/TI/TS Compatibility Fix succesfully installed.");
                XNAMessageBox.Show(WindowManager, "Compatibility Fix Installed",
                    "The DTA/TI/TS Compatibility Fix has been succesfully installed.");

                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                regKey = regKey.CreateSubKey("Tiberian Sun Client");
                regKey.SetValue("TSCompatFixInstalled", "Yes");

                btnGameCompatibilityFix.Text = "Disable";

                GameCompatFixInstalled = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Installing DTA/TI/TS Compatibility Fix failed. Error message: " + ex.Message);
                XNAMessageBox.Show(WindowManager, "Installing Compatibility Fix Failed",
                    "Installing DTA/TI/TS Compatibility Fix failed. Error message: " + ex.Message);
            }
        }

        private void BtnMapEditorCompatibilityFix_LeftClick(object sender, EventArgs e)
        {
            if (FinalSunCompatFixInstalled)
            {
                try
                {
                    Process sdbinst = Process.Start("sdbinst.exe", "-q -n \"Final Sun Compatibility Fix\"");

                    sdbinst.WaitForExit();

                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                    regKey = regKey.CreateSubKey("Tiberian Sun Client");
                    regKey.SetValue("FSCompatFixInstalled", "No");

                    btnMapEditorCompatibilityFix.Text = "Enable";

                    Logger.Log("FinalSun Compatibility Fix succesfully uninstalled.");
                    XNAMessageBox.Show(WindowManager, "Compatibility Fix Uninstalled",
                        "The FinalSun Compatibility Fix has been succesfully uninstalled.");

                    FinalSunCompatFixInstalled = false;
                }
                catch (Exception ex)
                {
                    Logger.Log("Uninstalling FinalSun Compatibility Fix failed. Error message: " + ex.Message);
                    XNAMessageBox.Show(WindowManager, "Uninstalling Compatibility Fix Failed",
                        "Uninstalling FinalSun Compatibility Fix failed. Error message: " + ex.Message);
                }

                return;
            }


            try
            {
                Process sdbinst = Process.Start("sdbinst.exe", "-q \"" + ProgramConstants.GamePath + "Resources/FSCompatFix.sdb\"");

                sdbinst.WaitForExit();

                RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                regKey = regKey.CreateSubKey("Tiberian Sun Client");
                regKey.SetValue("FSCompatFixInstalled", "Yes");

                btnMapEditorCompatibilityFix.Text = "Disable";

                Logger.Log("FinalSun Compatibility Fix succesfully installed.");
                XNAMessageBox.Show(WindowManager, "Compatibility Fix Installed",
                    "The FinalSun Compatibility Fix has been succesfully installed.");

                FinalSunCompatFixInstalled = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Installing FinalSun Compatibility Fix failed. Error message: " + ex.Message);
                XNAMessageBox.Show(WindowManager, "Installing Compatibility Fix Failed",
                    "Installing FinalSun Compatibility Fix failed. Error message: " + ex.Message);
            }
        }

        private void ChkBorderlessMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBorderlessClient.Checked)
            {
                ddClientResolution.AllowDropDown = false;
                ScreenResolution nativeRes = new ScreenResolution(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                int nativeResIndex = ddClientResolution.Items.FindIndex(i => (ScreenResolution)i.Tag == nativeRes);
                if (nativeResIndex > -1)
                    ddClientResolution.SelectedIndex = nativeResIndex;
            }
            else
            {
                ddClientResolution.AllowDropDown = true;

                if (ddClientResolution.PreferredItemIndexes.Count > 0)
                {
                    int optimalWindowedResIndex = ddClientResolution.PreferredItemIndexes[0];
                    ddClientResolution.SelectedIndex = optimalWindowedResIndex;
                }
            }
        }

        /// <summary>
        /// Loads the user's preferred renderer.
        /// </summary>
        private void LoadRenderer()
        {
            int index = ddRenderer.Items.FindIndex(
                           r => ((DirectDrawWrapper)r.Tag).InternalName == selectedRenderer.InternalName);

            if (index < 0 && selectedRenderer.Hidden)
            {
                ddRenderer.AddItem(new XNADropDownItem()
                {
                    Text = selectedRenderer.UIName,
                    Tag = selectedRenderer
                });
                index = ddRenderer.Items.Count - 1;
            }

            ddRenderer.SelectedIndex = index;
        }

        private static Dictionary<Point, double> defaultScaleFactors = new Dictionary<Point, double>()
        {
            { new Point(640, 480), 1.0 },
            { new Point(800, 600), 1.0 },
            { new Point(1024, 768), 1.25 },
            { new Point(1280, 720), 1.5 },
            { new Point(1280, 1024), 1.25 },
            { new Point(1366, 768), 1.0 },
            { new Point(1440, 900), 1.25 },
            { new Point(1600, 900), 1.25 },
            { new Point(1650, 1080), 1.5 },
            { new Point(1920, 1080), 1.5 },
            { new Point(2560, 1080), 2.0 },
            { new Point(2560, 1440), 2.0 },
            { new Point(3840, 2160), 3.0 }
        };

        public override void Load()
        {
            base.Load();

            LoadRenderer();
            ddDetailLevel.SelectedIndex = IniSettings.DetailLevel;
            string displayModeString = IniSettings.DisplayMode;
            if (Enum.TryParse(displayModeString, true, out GameDisplayMode gameDisplayMode))
            {
                ddDisplayMode.SelectedIndex = (int)gameDisplayMode;
            }
            else
            {
                ddDisplayMode.SelectedIndex = 0;
            }

            int width = IniSettings.UnscaledScreenWidth.Value;
            int height = IniSettings.UnscaledScreenHeight.Value;

            if (width < 0 || height < 0)
            {
                Point desktopSize = new Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                width = desktopSize.X;
                height = desktopSize.Y;
            }

            ScreenResolution unscaledIngameResolution = new ScreenResolution(width, height);

            int index = ddIngameResolution.Items.FindIndex(ddi => ((ScreenResolution)ddi.Tag).Equals(unscaledIngameResolution));

            ddIngameResolution.SelectedIndex = index;

            // In case the desktop resolution was not available in our resolution list, default to the highest resolution.
            if (ddIngameResolution.SelectedIndex < 0 && !IniSettings.IsCustomResolution)
                ddIngameResolution.SelectedIndex = ddIngameResolution.Items.Count - 1;

            RefreshScaleFactors();
            double scaleFactor = IniSettings.ScaleFactor;
            ddScaleFactor.SelectedIndex = ddScaleFactor.Items.FindIndex(ddi => (double)ddi.Tag == scaleFactor);
            if (ddScaleFactor.SelectedIndex < 0 && IniSettings.ScaleFactor == double.MinValue && !IniSettings.IsCustomResolution)
            {
                // No scale factor currently specified, try to find default
                if (defaultScaleFactors.TryGetValue(new Point(width, height), out scaleFactor))
                {
                    ddScaleFactor.SelectedIndex = ddScaleFactor.Items.FindIndex(ddi => (double)ddi.Tag == scaleFactor);
                }

                // If no scale factor was found for this resolution, just apply 1.0
                if (ddScaleFactor.SelectedIndex == -1 && ddScaleFactor.Items.Count > 0)
                    ddScaleFactor.SelectedIndex = 0;
            }

            // Wonder what this "Win8CompatMode" actually does..
            // Disabling it used to be TS-DDRAW only, but it was never enabled after 
            // you had tried TS-DDRAW once, so most players probably have it always
            // disabled anyway
            IniSettings.Win8CompatMode.Value = "No";

            var currentClientRes = new ScreenResolution(IniSettings.ClientResolutionX.Value, IniSettings.ClientResolutionY.Value);

            int clientResIndex = ddClientResolution.Items.FindIndex(i => ((ScreenResolution)i.Tag).Equals(currentClientRes));

            ddClientResolution.SelectedIndex = clientResIndex > -1 ? clientResIndex : 0;

            chkBorderlessClient.Checked = IniSettings.BorderlessWindowedClient;

            int selectedThemeIndex = ddClientTheme.Items.FindIndex(
                ddi => ddi.Text == IniSettings.ClientTheme);
            ddClientTheme.SelectedIndex = selectedThemeIndex > -1 ? selectedThemeIndex : 0;

            chkBackBufferInVRAM.Checked = !IniSettings.BackBufferInVRAM;
            chkStretchMovies.Checked = IniSettings.StretchMovies;
            chkClassicMessageListPosition.Checked = IniSettings.ClassicMessageListPosition;
        }

        private void AddCompatibilityFixControls()
        {
            lblCompatibilityFixes = new XNALabel(WindowManager);
            lblCompatibilityFixes.Name = "lblCompatibilityFixes";
            lblCompatibilityFixes.FontIndex = 1;
            lblCompatibilityFixes.Text = "Legacy Compatibility Fixes:";
            AddChild(lblCompatibilityFixes);
            lblCompatibilityFixes.CenterOnParent();
            lblCompatibilityFixes.Y = Height - 97;

            lblGameCompatibilityFix = new XNALabel(WindowManager);
            lblGameCompatibilityFix.Name = "lblGameCompatibilityFix";
            lblGameCompatibilityFix.ClientRectangle = new Rectangle(132,
                lblCompatibilityFixes.Bottom + 20, 0, 0);
            lblGameCompatibilityFix.Text = "DTA/TI/TS Compatibility Fix:";

            btnGameCompatibilityFix = new XNAClientButton(WindowManager);
            btnGameCompatibilityFix.Name = "btnGameCompatibilityFix";
            btnGameCompatibilityFix.ClientRectangle = new Rectangle(
                lblGameCompatibilityFix.Right + 20,
                lblGameCompatibilityFix.Y - 4, 133, 23);
            btnGameCompatibilityFix.FontIndex = 1;
            btnGameCompatibilityFix.Text = "Enable";
            btnGameCompatibilityFix.LeftClick += BtnGameCompatibilityFix_LeftClick;

            lblMapEditorCompatibilityFix = new XNALabel(WindowManager);
            lblMapEditorCompatibilityFix.Name = "lblMapEditorCompatibilityFix";
            lblMapEditorCompatibilityFix.ClientRectangle = new Rectangle(
                lblGameCompatibilityFix.X,
                lblGameCompatibilityFix.Bottom + 20, 0, 0);
            lblMapEditorCompatibilityFix.Text = "FinalSun Compatibility Fix:";

            btnMapEditorCompatibilityFix = new XNAClientButton(WindowManager);
            btnMapEditorCompatibilityFix.Name = "btnMapEditorCompatibilityFix";
            btnMapEditorCompatibilityFix.ClientRectangle = new Rectangle(
                btnGameCompatibilityFix.X,
                lblMapEditorCompatibilityFix.Y - 4,
                btnGameCompatibilityFix.Width,
                btnGameCompatibilityFix.Height);
            btnMapEditorCompatibilityFix.FontIndex = 1;
            btnMapEditorCompatibilityFix.Text = "Enable";
            btnMapEditorCompatibilityFix.LeftClick += BtnMapEditorCompatibilityFix_LeftClick;

            AddChild(lblGameCompatibilityFix);
            AddChild(btnGameCompatibilityFix);
            AddChild(lblMapEditorCompatibilityFix);
            AddChild(btnMapEditorCompatibilityFix);

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Tiberian Sun Client");

            if (regKey == null)
                return;

            object tsCompatFixValue = regKey.GetValue("TSCompatFixInstalled", "No");
            string tsCompatFixString = (string)tsCompatFixValue;

            if (tsCompatFixString == "Yes")
            {
                GameCompatFixInstalled = true;
                btnGameCompatibilityFix.Text = "Disable";
            }

            object fsCompatFixValue = regKey.GetValue("FSCompatFixInstalled", "No");
            string fsCompatFixString = (string)fsCompatFixValue;

            if (fsCompatFixString == "Yes")
            {
                FinalSunCompatFixInstalled = true;
                btnMapEditorCompatibilityFix.Text = "Disable";
            }

            // These compatibility fixes from 2015 are no longer necessary on modern systems.
            // They are only offered for uninstallation; if they are not installed, hide them.
            if (!FinalSunCompatFixInstalled)
            {
                lblMapEditorCompatibilityFix.Disable();
                btnMapEditorCompatibilityFix.Disable();

                if (!GameCompatFixInstalled)
                {
                    lblGameCompatibilityFix.Disable();
                    btnGameCompatibilityFix.Disable();
                    lblCompatibilityFixes.Disable();
                }
            }
        }

        public override bool Save()
        {
            base.Save();

            bool restartRequired = false;

            IniSettings.DetailLevel.Value = ddDetailLevel.SelectedIndex;
            IniSettings.DisplayMode.Value = ((GameDisplayMode)ddDisplayMode.SelectedIndex).ToString();

            SaveIngameResolutionAndRenderer();

            ScreenResolution clientResolution = (ScreenResolution)ddClientResolution.SelectedItem.Tag;

            if (clientResolution.Width != IniSettings.ClientResolutionX.Value ||
                clientResolution.Height != IniSettings.ClientResolutionY.Value)
                restartRequired = true;

            IniSettings.ClientResolutionX.Value = clientResolution.Width;
            IniSettings.ClientResolutionY.Value = clientResolution.Height;

            if (IniSettings.BorderlessWindowedClient.Value != chkBorderlessClient.Checked)
                restartRequired = true;

            IniSettings.BorderlessWindowedClient.Value = chkBorderlessClient.Checked;

            if (IniSettings.ClientTheme != ddClientTheme.SelectedItem.Text)
                restartRequired = true;

            IniSettings.ClientTheme.Value = ddClientTheme.SelectedItem.Text;

            IniSettings.BackBufferInVRAM.Value = !chkBackBufferInVRAM.Checked;
            IniSettings.StretchMovies.Value = chkStretchMovies.Checked;
            IniSettings.ClassicMessageListPosition.Value = chkClassicMessageListPosition.Checked;

            return restartRequired;
        }

        private void SaveIngameResolutionAndRenderer()
        {
            DirectDrawWrapper originalRenderer = selectedRenderer;
            selectedRenderer = (DirectDrawWrapper)ddRenderer.SelectedItem.Tag;

            if (selectedRenderer != originalRenderer ||
                !File.Exists(ProgramConstants.GamePath + selectedRenderer.ConfigFileName))
            {
                // User changed the renderer - clean up original renderer configuration files
                foreach (var renderer in renderers)
                {
                    if (renderer != selectedRenderer)
                        renderer.Clean();
                }
            }

            selectedRenderer.Apply();

            IniFile rendererSettingsIni = null;

            if (!string.IsNullOrWhiteSpace(selectedRenderer.ConfigFileName))
                rendererSettingsIni = new IniFile(ProgramConstants.GamePath + selectedRenderer.ConfigFileName);

            bool writeRendererSettings = false;

            // If the user has manually set the custom resolution flag in the settings INI,
            // it tells us that they absolutely don't want the client to do anything regarding
            // resolution. In that case, skip.
            if (!UserINISettings.Instance.IsCustomResolution && ddIngameResolution.SelectedItem != null)
            {
                ScreenResolution unscaledIngameResolution = (ScreenResolution)ddIngameResolution.SelectedItem.Tag;

                IniSettings.UnscaledScreenWidth.Value = unscaledIngameResolution.Width;
                IniSettings.UnscaledScreenHeight.Value = unscaledIngameResolution.Height;

                int scaledWidth = unscaledIngameResolution.Width;
                int scaledHeight = unscaledIngameResolution.Height;

                if (rendererSettingsIni != null && selectedRenderer.SupportsScaling && ddScaleFactor.SelectedItem != null)
                {
                    // This renderer support scaling - write scaling options to the renderer configuration file
                    double scaleFactor = (double)ddScaleFactor.SelectedItem.Tag;
                    scaledWidth = (int)(unscaledIngameResolution.Width / scaleFactor);
                    scaledHeight = (int)(unscaledIngameResolution.Height / scaleFactor);
                    IniSettings.ScaleFactor.Value = scaleFactor;

                    writeRendererSettings = true;

                    if (!string.IsNullOrWhiteSpace(selectedRenderer.ScaledWidthKey) && !string.IsNullOrWhiteSpace(selectedRenderer.ScaledHeightKey))
                    {
                        rendererSettingsIni.SetIntValue(selectedRenderer.ScalingSection, selectedRenderer.ScaledWidthKey, unscaledIngameResolution.Width);
                        rendererSettingsIni.SetIntValue(selectedRenderer.ScalingSection, selectedRenderer.ScaledHeightKey, unscaledIngameResolution.Height);
                    }

                    if (!string.IsNullOrWhiteSpace(selectedRenderer.ScaledResolutionKey))
                    {
                        rendererSettingsIni.SetStringValue(selectedRenderer.ScalingSection, selectedRenderer.ScaledResolutionKey, $"{unscaledIngameResolution.Width}x{unscaledIngameResolution.Height}");
                    }

                    if (selectedRenderer.SupportsSharpScaling)
                    {
                        if (Math.Truncate(scaleFactor) == scaleFactor)
                        {
                            // Enable sharp-scaling
                            rendererSettingsIni.SetStringValue(selectedRenderer.SharpScalingConfigValue.Section,
                                selectedRenderer.SharpScalingConfigValue.Key,
                                selectedRenderer.SharpScalingConfigValue.Value);
                        }
                        else
                        {
                            // Disable sharp-scaling
                            rendererSettingsIni.SetStringValue(selectedRenderer.NonSharpScalingConfigValue.Section,
                                selectedRenderer.NonSharpScalingConfigValue.Key,
                                selectedRenderer.NonSharpScalingConfigValue.Value);
                        }
                    }
                }
                else
                {
                    IniSettings.ScaleFactor.ClearValue();
                }

                IniSettings.ScaledScreenWidth.Value = scaledWidth;
                IniSettings.ScaledScreenHeight.Value = scaledHeight;

                if (rendererSettingsIni != null)
                {
                    if (!string.IsNullOrWhiteSpace(selectedRenderer.GameResolutionKey))
                    {
                        rendererSettingsIni.SetStringValue(selectedRenderer.ScalingSection, selectedRenderer.GameResolutionKey, $"{scaledWidth}x{scaledHeight}");
                    }
                }

                // Calculate drag selection distance, scale it with resolution width
                int dragDistance = scaledWidth / ORIGINAL_RESOLUTION_WIDTH * DRAG_DISTANCE_DEFAULT;
                IniSettings.DragDistance.Value = dragDistance;

                if (scaledWidth >= 1024 && scaledHeight >= 720)
                    File.Copy(ProgramConstants.GamePath + "Resources/language_1024x720.dll", ProgramConstants.GamePath + "Language.dll", true);
                else if (scaledWidth >= 800 && scaledHeight >= 600)
                    File.Copy(ProgramConstants.GamePath + "Resources/language_800x600.dll", ProgramConstants.GamePath + "Language.dll", true);
                else
                    File.Copy(ProgramConstants.GamePath + "Resources/language_640x480.dll", ProgramConstants.GamePath + "Language.dll", true);
            }

            // Save display mode settings
            bool windowed = ddDisplayMode.SelectedIndex == (int)(GameDisplayMode.BorderlessWindowed) || ddDisplayMode.SelectedIndex == (int)(GameDisplayMode.Windowed);
            bool borderless = ddDisplayMode.SelectedIndex == (int)(GameDisplayMode.BorderlessWindowed);

            if (rendererSettingsIni != null)
            {
                // Unfortunately, due to differences between renderers (mainly DDrawCompat being weird),
                // we have to handle all 3 display mode cases separately.
                if (windowed && !borderless)
                {
                    // Windowed mode (with borders)

                    if (selectedRenderer.UsesCustomWindowedOption())
                    {
                        rendererSettingsIni.SetBooleanValue(selectedRenderer.WindowedModeSection,
                            selectedRenderer.WindowedModeKey, windowed);
                    }
                    else
                    {
                        IniSettings.WindowedMode.Value = windowed;
                        IniSettings.BorderlessWindowedMode.Value = false;
                    }

                    IniSettings.BorderlessWindowedMode.Value = false;
                    if (!string.IsNullOrWhiteSpace(selectedRenderer.BorderlessWindowedModeKey))
                    {
                        bool borderlessModeIniValue = borderless;
                        if (selectedRenderer.IsBorderlessWindowedModeKeyReversed)
                            borderlessModeIniValue = !borderlessModeIniValue;

                        rendererSettingsIni.SetBooleanValue(selectedRenderer.WindowedModeSection,
                            selectedRenderer.BorderlessWindowedModeKey, borderlessModeIniValue);
                    }

                    if (selectedRenderer.BorderlessModeKeyValue != null)
                    {
                        var section = rendererSettingsIni.GetSection(selectedRenderer.BorderlessModeKeyValue.Value.Section);
                        if (section != null)
                            section.RemoveKey(selectedRenderer.BorderlessModeKeyValue.Value.Key);
                    }

                    if (selectedRenderer.FullscreenModeKeyValue != null)
                    {
                        var section = rendererSettingsIni.GetSection(selectedRenderer.FullscreenModeKeyValue.Value.Section);
                        if (section != null)
                            section.RemoveKey(selectedRenderer.FullscreenModeKeyValue.Value.Key);
                    }
                }
                else if (borderless)
                {
                    // Borderless mode

                    if (selectedRenderer.UsesCustomWindowedOption())
                    {
                        rendererSettingsIni.SetBooleanValue(selectedRenderer.WindowedModeSection,
                            selectedRenderer.WindowedModeKey, windowed);

                        if (!string.IsNullOrEmpty(selectedRenderer.BorderlessWindowedModeKey))
                        {
                            bool borderlessModeIniValue = borderless;
                            if (selectedRenderer.IsBorderlessWindowedModeKeyReversed)
                                borderlessModeIniValue = !borderlessModeIniValue;

                            rendererSettingsIni.SetBooleanValue(selectedRenderer.WindowedModeSection,
                                selectedRenderer.BorderlessWindowedModeKey, borderlessModeIniValue);

                            IniSettings.BorderlessWindowedMode.Value = false;
                        }
                        else
                        {
                            IniSettings.BorderlessWindowedMode.Value = true;
                        }

                        // Disable game windowed mode option so it does not interfere with the renderer
                        IniSettings.WindowedMode.Value = false;
                    }
                    else if (selectedRenderer.HandlesWindowedOnlyInBorderlessMode)
                    {
                        IniSettings.WindowedMode.Value = false;
                        IniSettings.BorderlessWindowedMode.Value = false;
                    }
                    else
                    {
                        IniSettings.WindowedMode.Value = true;
                        IniSettings.BorderlessWindowedMode.Value = true;
                    }

                    if (selectedRenderer.FullscreenModeKeyValue != null)
                    {
                        var section = rendererSettingsIni.GetSection(selectedRenderer.FullscreenModeKeyValue.Value.Section);
                        if (section != null)
                            section.RemoveKey(selectedRenderer.FullscreenModeKeyValue.Value.Key);
                    }

                    if (selectedRenderer.BorderlessModeKeyValue != null)
                    {
                        rendererSettingsIni.SetStringValue(selectedRenderer.BorderlessModeKeyValue.Value.Section,
                            selectedRenderer.BorderlessModeKeyValue.Value.Key,
                            selectedRenderer.BorderlessModeKeyValue.Value.Value);
                    }
                }
                else
                {
                    // Fullscreen mode

                    if (selectedRenderer.BorderlessModeKeyValue != null)
                    {
                        var section = rendererSettingsIni.GetSection(selectedRenderer.BorderlessModeKeyValue.Value.Section);
                        if (section != null)
                            section.RemoveKey(selectedRenderer.BorderlessModeKeyValue.Value.Key);
                    }

                    if (selectedRenderer.FullscreenModeKeyValue != null)
                    {
                        rendererSettingsIni.SetStringValue(selectedRenderer.FullscreenModeKeyValue.Value.Section,
                            selectedRenderer.FullscreenModeKeyValue.Value.Key,
                            selectedRenderer.FullscreenModeKeyValue.Value.Value);
                    }

                    IniSettings.WindowedMode.Value = false;
                    IniSettings.BorderlessWindowedMode.Value = false;

                    if (selectedRenderer.UsesCustomWindowedOption())
                    {
                        rendererSettingsIni.SetBooleanValue(selectedRenderer.WindowedModeSection,
                            selectedRenderer.WindowedModeKey, false);

                        if (!string.IsNullOrWhiteSpace(selectedRenderer.BorderlessWindowedModeKey))
                        {
                            rendererSettingsIni.SetBooleanValue(selectedRenderer.WindowedModeSection, selectedRenderer.BorderlessWindowedModeKey, false);
                        }
                    }
                }

                writeRendererSettings = true;
            }

            if (selectedRenderer.ForcedConfigValues.Count > 0)
            {
                foreach (var configValue in selectedRenderer.ForcedConfigValues)
                {
                    rendererSettingsIni.SetStringValue(configValue.Section, configValue.Key, configValue.Value);
                }

                writeRendererSettings = true;
            }

            if (writeRendererSettings)
                rendererSettingsIni.WriteIniFile();

            GameProcessLogic.UseQres = selectedRenderer.UseQres;
            GameProcessLogic.SingleCoreAffinity = selectedRenderer.SingleCoreAffinity;

            IniSettings.Renderer.Value = selectedRenderer.InternalName;
        }

        private List<ScreenResolution> GetResolutions(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            var screenResolutions = new List<ScreenResolution>();

            foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (dm.Width < minWidth || dm.Height < minHeight || dm.Width > maxWidth || dm.Height > maxHeight)
                    continue;

                var resolution = new ScreenResolution(dm.Width, dm.Height);

                // SupportedDisplayModes can include the same resolution multiple times
                // because it takes the refresh rate into consideration.
                // Which means that we have to check if the resolution is already listed
                if (screenResolutions.Find(res => res.Equals(resolution)) != null)
                    continue;

                screenResolutions.Add(resolution);
            }

            return screenResolutions;
        }

        /// <summary>
        /// A single screen resolution.
        /// </summary>
        sealed class ScreenResolution : IComparable<ScreenResolution>
        {
            public ScreenResolution(int width, int height)
            {
                Width = width;
                Height = height;
            }

            /// <summary>
            /// The width of the resolution in pixels.
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// The height of the resolution in pixels.
            /// </summary>
            public int Height { get; set; }

            public override string ToString()
            {
                return Width + "x" + Height;
            }

            public int CompareTo(ScreenResolution res2)
            {
                if (this.Width < res2.Width)
                    return -1;
                else if (this.Width > res2.Width)
                    return 1;
                else // equal
                {
                    if (this.Height < res2.Height)
                        return -1;
                    else if (this.Height > res2.Height)
                        return 1;
                    else return 0;
                }
            }

            public override bool Equals(object obj)
            {
                var resolution = obj as ScreenResolution;

                if (resolution == null)
                    return false;

                return CompareTo(resolution) == 0;
            }

            public override int GetHashCode()
            {
                return Width - Height;
            }
        }
    }
}
