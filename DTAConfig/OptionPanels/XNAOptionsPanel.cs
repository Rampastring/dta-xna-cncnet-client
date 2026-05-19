using ClientCore;
using ClientGUI;
using Microsoft.Xna.Framework;
using Rampastring.Tools;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using System.Collections.Generic;

namespace DTAConfig.OptionPanels
{
    /// <summary>
    /// A base class for all option panels.
    /// Handles custom game-specific panel options
    /// defined in INI files.
    /// </summary>
    internal abstract class XNAOptionsPanel : XNAWindowBase
    {
        public XNAOptionsPanel(WindowManager windowManager, 
            UserINISettings iniSettings) : base(windowManager)
        {
            IniSettings = iniSettings;
            CustomGUICreator = optionsGUICreator;
        }

        private static readonly OptionsGUICreator optionsGUICreator = new OptionsGUICreator();

        private readonly List<IUserSetting> userSettings = new List<IUserSetting>();
        private List<FileSettingCheckBox> fileSettingCheckBoxes = new List<FileSettingCheckBox>();

        public override void Initialize()
        {
            ClientRectangle = new Rectangle(12, 47,
                Parent.Width - 24,
                Parent.Height - 94);
            BackgroundTexture = AssetLoader.CreateTexture(new Color(0, 0, 0, 128), 2, 2);
            PanelBackgroundDrawMode = PanelBackgroundImageDrawMode.STRETCHED;

            base.Initialize();
        }

        /// <summary>
        /// Parses user-defined game options from an INI file.
        /// </summary>
        /// <param name="iniFile">The INI file.</param>
        public void ParseUserOptions(IniFile iniFile)
        {
            GetAttributes(iniFile);
            ParseExtraControls(iniFile, Name + "ExtraControls");
            ReadChildControlAttributes(iniFile);
        }

        protected override void ParseExtraControls(IniFile iniFile, string sectionName)
        {
            base.ParseExtraControls(iniFile, sectionName);

            foreach (var control in Children)
            {
                if (!(control is FileSettingCheckBox controlAsFileSettingCheckBox))
                    continue;

                fileSettingCheckBoxes.Add(controlAsFileSettingCheckBox);
            }
        }

        public override void AddChild(XNAControl child)
        {
            base.AddChild(child);

            if (child is IUserSetting settingControl)
                userSettings.Add(settingControl);
        }

        protected UserINISettings IniSettings { get; private set; }

        /// <summary>
        /// Saves the options of this panel.
        /// Returns a bool that determines whether the 
        /// client needs to restart for changes to apply.
        /// </summary>
        public virtual bool Save()
        {
            foreach (var checkBox in fileSettingCheckBoxes)
                checkBox.Save();

            bool restartRequired = false;
            foreach (var setting in userSettings)
                restartRequired = setting.Save() || restartRequired;

            return restartRequired;
        }

        /// <summary>
        /// Loads the options of this panel.
        /// </summary>
        public virtual void Load()
        {
            foreach (var checkBox in fileSettingCheckBoxes)
                checkBox.Load();

            foreach (var setting in userSettings)
                setting.Load();
        }

        /// <summary>
        /// Enables or disables any options that should only be available when
        /// options window was opened in main menu.
        /// </summary>
        /// <param name="enable">If true enables options, disables if false.</param>
        public virtual void ToggleMainMenuOnlyOptions(bool enable)
        {
        }
    }
}
