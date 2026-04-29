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
        Windowed = 1,
        NativeFullscreen = 2,
        Count
    }

    struct ViniferaRendererInfo
    {
        public readonly ViniferaRenderer InternalType;
        public readonly string UIName;

        public ViniferaRendererInfo(ViniferaRenderer internalType, string uIName)
        {
            InternalType = internalType;
            UIName = uIName;
        }
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
        private XNAClientDropDown ddRenderer;
        private XNAClientDropDown ddDetailLevel;
        private XNAClientCheckBox chkBackBufferInVRAM;
        private XNAClientCheckBox chkStretchMovies;
        private XNAClientCheckBox chkClassicMessageListPosition;
        private XNAClientPreferredItemDropDown ddClientResolution;
        private XNAClientCheckBox chkBorderlessClient;
        private XNAClientDropDown ddClientTheme;

        private List<ScreenResolution> resolutions;

        private XNALabel lblCompatibilityFixes;
        private XNALabel lblGameCompatibilityFix;
        private XNALabel lblMapEditorCompatibilityFix;
        private XNAClientButton btnGameCompatibilityFix;
        private XNAClientButton btnMapEditorCompatibilityFix;

        private bool GameCompatFixInstalled = false;
        private bool FinalSunCompatFixInstalled = false;

        private List<ViniferaRendererInfo> viniferaRenderers = new List<ViniferaRendererInfo>()
        {
            new ViniferaRendererInfo(ViniferaRenderer.Auto, "Auto"),
            new ViniferaRendererInfo(ViniferaRenderer.Direct3D, "DirectX 9"),
            new ViniferaRendererInfo(ViniferaRenderer.Direct3D11, "DirectX 11"),
            new ViniferaRendererInfo(ViniferaRenderer.Direct3D12, "DirectX 12"),
            new ViniferaRendererInfo(ViniferaRenderer.OpenGL, "OpenGL"),
            new ViniferaRendererInfo(ViniferaRenderer.Vulkan, "Vulkan"),
        };

        public override void Initialize()
        {
            base.Initialize();

            Name = "DisplayOptionsPanel";

            var clientConfig = ClientConfiguration.Instance;

            var lblDisplayMode = new XNALabel(WindowManager);
            lblDisplayMode.Name = nameof(lblDisplayMode);
            lblDisplayMode.X = UIDesignConstants.EMPTY_SPACE_SIDES_NEW;
            lblDisplayMode.Y = UIDesignConstants.EMPTY_SPACE_TOP_NEW + 2;
            lblDisplayMode.Text = "Display Mode:";
            AddChild(lblDisplayMode);

            ddDisplayMode = new XNAClientDropDown(WindowManager);
            ddDisplayMode.Name = nameof(ddDisplayMode);
            ddDisplayMode.X = lblDisplayMode.X + 112;
            ddDisplayMode.Y = lblDisplayMode.Y - 2;
            ddDisplayMode.Width = 134;
            AddChild(ddDisplayMode);
            ddDisplayMode.AddItem("Fullscreen");
            // ddDisplayMode.AddItem("Native Fullscreen");
            ddDisplayMode.AddItem("Windowed");
            ddDisplayMode.ToolTip.Text = "Defines how the in-game window is displayed.";
            ddDisplayMode.SelectedIndexChanged += (s, e) => RefreshScaleFactors(false);

            var lblIngameResolution = new XNALabel(WindowManager);
            lblIngameResolution.Name = "lblIngameResolution";
            lblIngameResolution.ClientRectangle = new Rectangle(lblDisplayMode.X, ddDisplayMode.Bottom + 16, 0, 0);
            lblIngameResolution.Text = "In-Game Resolution:";

            ddIngameResolution = new XNAClientDropDown(WindowManager);
            ddIngameResolution.Name = "ddIngameResolution";
            ddIngameResolution.ClientRectangle = new Rectangle(
                ddDisplayMode.X, lblIngameResolution.Y - 2, ddDisplayMode.Width, ddDisplayMode.Height);
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

            var lblRenderer = new XNALabel(WindowManager);
            lblRenderer.Name = nameof(lblRenderer);
            lblRenderer.X = lblDisplayMode.X;
            lblRenderer.Y = ddScaleFactor.Bottom + 16;
            lblRenderer.Text = "Renderer:";
            AddChild(lblRenderer);

            ddRenderer = new XNAClientDropDown(WindowManager);
            ddRenderer.Name = nameof(ddRenderer);
            ddRenderer.X = ddDisplayMode.X;
            ddRenderer.Y = lblRenderer.Y - 2;
            ddRenderer.Width = ddDisplayMode.Width;
            ddRenderer.Height = ddDisplayMode.Height;
            AddChild(ddRenderer);
            for (int i = 0; i < viniferaRenderers.Count; i++)
            {
                ddRenderer.AddItem(new XNADropDownItem() { Text = viniferaRenderers[i].UIName, Tag = viniferaRenderers[i].InternalType });
            }

            var lblDetailLevel = new XNALabel(WindowManager);
            lblDetailLevel.Name = "lblDetailLevel";
            lblDetailLevel.ClientRectangle = new Rectangle(lblIngameResolution.X,
                ddRenderer.Bottom + 16, 0, 0);
            lblDetailLevel.Text = "Detail Level:";

            ddDetailLevel = new XNAClientDropDown(WindowManager);
            ddDetailLevel.Name = "ddDetailLevel";
            ddDetailLevel.ClientRectangle = new Rectangle(
                ddDisplayMode.X,
                lblDetailLevel.Y - 2,
                ddDisplayMode.Width,
                ddDisplayMode.Height);
            ddDetailLevel.AddItem("Low");
            ddDetailLevel.AddItem("Medium");
            ddDetailLevel.AddItem("High");

            chkBackBufferInVRAM = new XNAClientCheckBox(WindowManager);
            chkBackBufferInVRAM.Name = "chkBackBufferInVRAM";
            chkBackBufferInVRAM.ClientRectangle = new Rectangle(
                lblDetailLevel.X,
                ddDetailLevel.Bottom + 12, 0, 0);
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
                ddDisplayMode.Height);
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
                lblIngameResolution.Y, 0, 0);
            chkBorderlessClient.Text = "Fullscreen Client";
            chkBorderlessClient.CheckedChanged += ChkBorderlessMenu_CheckedChanged;
            chkBorderlessClient.Checked = true;

            var lblClientTheme = new XNALabel(WindowManager);
            lblClientTheme.Name = "lblClientTheme";
            lblClientTheme.X = lblClientResolution.X;
            lblClientTheme.Y = ddScaleFactor.Y;
            lblClientTheme.Text = "Client Theme:";

            ddClientTheme = new XNAClientDropDown(WindowManager);
            ddClientTheme.Name = "ddClientTheme";
            ddClientTheme.ClientRectangle = new Rectangle(
                ddClientResolution.X,
                ddScaleFactor.Y,
                ddClientResolution.Width,
                ddClientResolution.Height);

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
            AddChild(lblDetailLevel);
            AddChild(ddDetailLevel);
            AddChild(lblIngameResolution);
            AddChild(ddIngameResolution);
        }

        private void RefreshScaleFactors(bool applyRecommendedFactor = true)
        {
            if (ddIngameResolution.SelectedItem == null)
                return;

            double selectedScaleFactor = 1.0;

            if (ddScaleFactor.SelectedItem != null)
                selectedScaleFactor = (double)ddScaleFactor.SelectedItem.Tag;

            ddScaleFactor.SelectedIndex = -1;
            ddScaleFactor.Items.Clear();

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
            if (applyRecommendedFactor)
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

        /// <summary>
        /// Loads settings.
        /// </summary>
        public void PostInit()
        {
            Load();
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
        }

        private void ChkBorderlessMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBorderlessClient.Checked)
            {
                ddClientResolution.AllowDropDown = false;
                ScreenResolution nativeRes = new ScreenResolution(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                int nativeResIndex = ddClientResolution.Items.FindIndex(i => ((ScreenResolution)i.Tag).Equals(nativeRes));
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

        private static Dictionary<Point, double> defaultScaleFactors = new Dictionary<Point, double>()
        {
            { new Point(640, 480), 1.0 },
            { new Point(800, 600), 1.0 },
            { new Point(800, 1280), 1.25 },
            { new Point(1024, 768), 1.0 },
            { new Point(1152, 864), 1.25 },
            { new Point(1280, 720), 1.25 },
            { new Point(1280, 768), 1.25 },
            { new Point(1280, 800), 1.25 },
            { new Point(1280, 960), 1.25 },
            { new Point(1280, 1024), 1.25 },
            { new Point(1360, 768), 1.25 },
            { new Point(1366, 768), 1.25 },
            { new Point(1440, 900), 1.25 },
            { new Point(1440, 1050), 1.5 },
            { new Point(1600, 900), 1.25 },
            { new Point(1600, 1200), 1.5 },
            { new Point(1680, 1050), 1.5 },
            { new Point(1920, 1080), 1.5 },
            { new Point(1920, 1200), 1.5 },
            { new Point(2048, 1080), 1.5 },
            { new Point(2048, 1536), 2.0 },
            { new Point(2560, 1080), 1.5 },
            { new Point(2560, 1440), 2.0 },
            { new Point(2560, 1600), 2.0 },
            { new Point(2880, 1800), 3.0 },
            { new Point(3440, 1440), 2.0 },
            { new Point(3840, 2160), 3.0 },
            { new Point(5120, 1440), 2.0 },
            { new Point(5120, 2160), 3.0 },
            { new Point(5120, 2880), 4.0 },
            { new Point(7680, 4320), 6.0 },
            { new Point(8192, 4320), 6.0 },
        };

        public override void Load()
        {
            base.Load();

            ddDetailLevel.SelectedIndex = IniSettings.DetailLevel;
            string displayModeString = IniSettings.DisplayMode;
            if (Enum.TryParse(displayModeString, true, out GameDisplayMode gameDisplayMode))
            {
                if (gameDisplayMode == GameDisplayMode.NativeFullscreen)
                    gameDisplayMode = GameDisplayMode.BorderlessWindowed; // Vinifera no longer supports native fullscreen

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

            ddRenderer.SelectedIndex = (int)UserINISettings.Instance.RendererDriver.Value;

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

            if (regKey != null)
            {
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

            GameDisplayMode displayMode = (GameDisplayMode)ddDisplayMode.SelectedIndex;
            IniSettings.DisplayMode.Value = displayMode.ToString();
            IniSettings.Windowed.Value = displayMode == GameDisplayMode.Windowed;
            IniSettings.VideoWindowed.Value = IniSettings.Windowed.Value;

            SaveIngameResolutionAndRenderer(displayMode);

            IniSettings.RendererDriver.Value = (ViniferaRenderer)ddRenderer.SelectedIndex;

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

        private void SaveIngameResolutionAndRenderer(GameDisplayMode gameDisplayMode)
        {
            // If the user has manually set the custom resolution flag in the settings INI,
            // it tells us that they absolutely don't want the client to do anything regarding
            // resolution. In that case, skip.
            if (!UserINISettings.Instance.IsCustomResolution && ddIngameResolution.SelectedItem != null)
            {
                ScreenResolution unscaledIngameResolution = (ScreenResolution)ddIngameResolution.SelectedItem.Tag;

                IniSettings.UnscaledScreenWidth.Value = unscaledIngameResolution.Width;
                IniSettings.UnscaledScreenHeight.Value = unscaledIngameResolution.Height;

                if (gameDisplayMode == GameDisplayMode.Windowed)
                {
                    IniSettings.WindowWidth.Value = unscaledIngameResolution.Width;
                    IniSettings.WindowHeight.Value = unscaledIngameResolution.Height;
                }
                else
                {
                    IniSettings.WindowWidth.Value = -1;
                    IniSettings.WindowHeight.Value = -1;
                }

                int scaledWidth = unscaledIngameResolution.Width;
                int scaledHeight = unscaledIngameResolution.Height;

                // Vinifera's SDL renderer supports scaling
                double scaleFactor = (double)ddScaleFactor.SelectedItem.Tag;
                scaledWidth = (int)(unscaledIngameResolution.Width / scaleFactor);
                scaledHeight = (int)(unscaledIngameResolution.Height / scaleFactor);
                IniSettings.ScaleFactor.Value = scaleFactor;

                IniSettings.ScaledScreenWidth.Value = scaledWidth;
                IniSettings.ScaledScreenHeight.Value = scaledHeight;

                // Calculate drag selection distance, scale it with resolution width
                int dragDistance = (int)((double)scaledWidth / ORIGINAL_RESOLUTION_WIDTH) * DRAG_DISTANCE_DEFAULT;
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
