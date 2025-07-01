using ClientCore;
using Rampastring.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DTAConfig
{
    struct DirectDrawWrapperConfigValue
    {
        public string Section;
        public string Key;
        public string Value;

        public DirectDrawWrapperConfigValue(string section, string key, string value)
        {
            Section = section;
            Key = key;
            Value = value;
        }

        public static bool TryParse(string str, out DirectDrawWrapperConfigValue directDrawWrapperConfigValue)
        {
            var valueSplit = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (valueSplit.Length > 2)
            {
                // In case the value contains commas, join elements back together after first two
                var configKeyValue = string.Join(",", valueSplit.Skip(2));
                directDrawWrapperConfigValue = new DirectDrawWrapperConfigValue(valueSplit[0], valueSplit[1], configKeyValue);
                return true;
            }

            directDrawWrapperConfigValue = new DirectDrawWrapperConfigValue(string.Empty, string.Empty, string.Empty);
            return false;
        }
    }

    /// <summary>
    /// A DirectDraw wrapper option.
    /// </summary>
    class DirectDrawWrapper
    {
        /// <summary>
        /// Creates a new DirectDrawWrapper instance and parses its configuration
        /// from an INI file.
        /// </summary>
        /// <param name="internalName">The internal name of the renderer.</param>
        /// <param name="iniFile">The file to parse the renderer's options from.</param>
        public DirectDrawWrapper(string internalName, IniFile iniFile)
        {
            InternalName = internalName;
            Parse(iniFile.GetSection(InternalName));
        }

        public string InternalName { get; private set; }
        public string UIName { get; private set; }

        /// <summary>
        /// If not null or empty, windowed mode will be written to an INI key
        /// in this section of the renderer settings file instead
        /// of the regular game settings INI file.
        /// </summary>
        public string WindowedModeSection { get; private set; }

        /// <summary>
        /// If not null or empty, windowed mode will be written to this INI key
        /// in the section defined in <see cref="DirectDrawWrapper.WindowedModeSection"/> 
        /// instead of the regular settings INI file.
        /// </summary>
        public string WindowedModeKey { get; private set; }

        /// <summary>
        /// If not null or empty, the setting that controls whether the game is 
        /// run in borderless windowed mode will be written to this INI key in
        /// the section defined by
        /// <see cref="DirectDrawWrapper.WindowedModeSection"/> instead of the
        /// regular settings INI file.
        /// </summary>
        public string BorderlessWindowedModeKey { get; private set; }

        /// <summary>
        /// If set, borderless mode is enabled if the setting is "false"
        /// and disabled if the setting is "true".
        /// </summary>
        public bool IsBorderlessWindowedModeKeyReversed { get; private set; }

        public bool Hidden { get; private set; }

        public bool SupportsScaling { get; private set; }
        public string ScalingSection { get; private set; }
        public string ScaledWidthKey { get; private set; }
        public string ScaledHeightKey { get; private set; }
        public bool SupportsSharpScaling { get; private set; }
        public DirectDrawWrapperConfigValue SharpScalingConfigValue { get; private set; }
        public DirectDrawWrapperConfigValue NonSharpScalingConfigValue { get; private set; }

        /// <summary>
        /// Many ddraw wrappers need qres.dat to set the desktop to 16 bit mode
        /// </summary>
        public bool UseQres { get; private set; } = false;

        /// <summary>
        /// If set to false, the client won't set single-core affinity
        /// to the game executable when this renderer is used.
        /// </summary>
        public bool SingleCoreAffinity { get; private set; } = true;

        /// <summary>
        /// The filename of the configuration INI of the renderer in the game directory.
        /// </summary>
        public string ConfigFileName { get; private set; }

        public List<DirectDrawWrapperConfigValue> ForcedConfigValues { get; private set; } = new List<DirectDrawWrapperConfigValue>();

        private string ddrawDLLPath;
        private string resConfigFileName;
        private List<string> filesToCopy = new List<string>();
        private List<OSVersion> disallowedOSList = new List<OSVersion>();

        /// <summary>
        /// Reads the properties of this DirectDrawWrapper from an INI section.
        /// </summary>
        /// <param name="section">The INI section.</param>
        private void Parse(IniSection section)
        {
            if (section == null)
            {
                Logger.Log("DirectDrawWrapper: Configuration for renderer '" + InternalName + "' not found!");
                return;
            }
                
            UIName = section.GetStringValue("UIName", "Unnamed renderer");

            if (section.GetBooleanValue("IsDxWnd", false))
            {
                // For backwards compatibility with previous client versions
                WindowedModeSection = "DxWnd";
                WindowedModeKey = "RunInWindow";
                BorderlessWindowedModeKey = "NoWindowFrame";
            }

            WindowedModeSection = section.GetStringValue("WindowedModeSection", WindowedModeSection);
            WindowedModeKey = section.GetStringValue("WindowedModeKey", WindowedModeKey);
            BorderlessWindowedModeKey = section.GetStringValue("BorderlessWindowedModeKey", BorderlessWindowedModeKey);
            IsBorderlessWindowedModeKeyReversed = section.GetBooleanValue("IsBorderlessWindowedModeKeyReversed",
                IsBorderlessWindowedModeKeyReversed);

            if (BorderlessWindowedModeKey != null && WindowedModeSection == null)
            {
                throw new DirectDrawWrapperConfigurationException(
                    "BorderlessWindowedModeKey= is defined for renderer" +
                    $" {InternalName} but WindowedModeSection= is not!");
            }

            Hidden = section.GetBooleanValue("Hidden", Hidden);
            UseQres = section.GetBooleanValue("UseQres", UseQres);
            SingleCoreAffinity = section.GetBooleanValue("SingleCoreAffinity", SingleCoreAffinity);
            ddrawDLLPath = section.GetStringValue("DLLName", string.Empty);
            ConfigFileName = section.GetStringValue("ConfigFileName", string.Empty);
            resConfigFileName = section.GetStringValue("ResConfigFileName", ConfigFileName);

            SupportsScaling = section.GetBooleanValue(nameof(SupportsScaling), SupportsScaling);
            if (SupportsScaling)
            {
                ScalingSection = section.GetStringValue(nameof(ScalingSection), string.Empty);
                ScaledWidthKey = section.GetStringValue(nameof(ScaledWidthKey), string.Empty);
                ScaledHeightKey = section.GetStringValue(nameof(ScaledHeightKey), string.Empty);

                SupportsSharpScaling = section.GetBooleanValue(nameof(SupportsSharpScaling), SupportsSharpScaling);

                string sharpScalingConfigValueString = section.GetStringValue(nameof(SharpScalingConfigValue), string.Empty);
                if (DirectDrawWrapperConfigValue.TryParse(sharpScalingConfigValueString, out var sharpScalingConfigValue))
                {
                    SharpScalingConfigValue = sharpScalingConfigValue;
                }
                else
                {
                    Logger.Log($"DirectDrawWrapper: Incorrect {nameof(SharpScalingConfigValue)} in renderer {InternalName}!");
                }

                string nonSharpScalingConfigValueString = section.GetStringValue(nameof(NonSharpScalingConfigValue), string.Empty);
                if (DirectDrawWrapperConfigValue.TryParse(nonSharpScalingConfigValueString, out var nonSharpScalingConfigValue))
                {
                    NonSharpScalingConfigValue = nonSharpScalingConfigValue;
                }
                else
                {
                    Logger.Log($"DirectDrawWrapper: Incorrect {nameof(NonSharpScalingConfigValue)} in renderer {InternalName}!");
                }
            }

            filesToCopy = section.GetStringValue("AdditionalFiles", string.Empty).Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            string[] disallowedOSs = section.GetStringValue("DisallowedOperatingSystems", string.Empty).Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string os in disallowedOSs)
            {
                OSVersion disallowedOS = (OSVersion)Enum.Parse(typeof(OSVersion), os.Trim());
                disallowedOSList.Add(disallowedOS);
            }

            if (!string.IsNullOrEmpty(ddrawDLLPath) && 
                !File.Exists(ProgramConstants.GetBaseResourcePath() + ddrawDLLPath))
                Logger.Log("DirectDrawWrapper: File specified in DLLPath= for renderer '" + InternalName + "' does not exist!");

            if (!string.IsNullOrEmpty(resConfigFileName) &&
                !File.Exists(ProgramConstants.GetBaseResourcePath() + resConfigFileName))
                Logger.Log("DirectDrawWrapper: File specified in ConfigFileName= for renderer '" + InternalName + "' does not exist!");

            foreach (var file in filesToCopy)
            {
                if (!File.Exists(ProgramConstants.GetBaseResourcePath() + file))
                    Logger.Log("DirectDrawWrapper: Additional file '" + file + "' for renderer '" + InternalName + "' does not exist!");
            }

            int configOptionIndex = 0;
            while (true)
            {
                string keyName = "ForcedConfigValue" + configOptionIndex.ToString(CultureInfo.InvariantCulture);

                if (section.KeyExists(keyName))
                {
                    string keyValue = section.GetStringValue(keyName, string.Empty);
                    bool success = DirectDrawWrapperConfigValue.TryParse(keyValue, out var configValue);

                    if (success)
                    {
                        ForcedConfigValues.Add(configValue);
                    }
                    else
                    {
                        Logger.Log($"Incorrect forced config value for renderer {InternalName}: {keyValue}");
                    }

                    configOptionIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns true if this wrapper is compatible with the given operating
        /// system, otherwise false.
        /// </summary>
        /// <param name="os">The operating system.</param>
        public bool IsCompatibleWithOS(OSVersion os)
        {
            return !disallowedOSList.Contains(os);
        }

        /// <summary>
        /// Applies the renderer's files to the game directory.
        /// </summary>
        public void Apply()
        {
            if (!string.IsNullOrEmpty(ddrawDLLPath))
            {
                File.Copy(ProgramConstants.GetBaseResourcePath() + ddrawDLLPath,
                    ProgramConstants.GamePath + "ddraw.dll", true);
            }
            else
            {
                File.Delete(ProgramConstants.GamePath + "ddraw.dll");
            }

            if (!string.IsNullOrEmpty(ConfigFileName) && !string.IsNullOrEmpty(resConfigFileName)
                && !File.Exists(ProgramConstants.GamePath + ConfigFileName)) // Do not overwrite settings
            {
                File.Copy(ProgramConstants.GetBaseResourcePath() + resConfigFileName,
                    ProgramConstants.GamePath + Path.GetFileName(ConfigFileName));
            }

            foreach (var file in filesToCopy)
            {
                File.Copy(ProgramConstants.GetBaseResourcePath() + file,
                    ProgramConstants.GamePath + Path.GetFileName(file), true);
            }
        }

        /// <summary>
        /// Call to clean the renderer's files from the game directory.
        /// </summary>
        public void Clean()
        {
            if (!string.IsNullOrEmpty(ConfigFileName))
                File.Delete(ProgramConstants.GamePath + Path.GetFileName(ConfigFileName));

            foreach (var file in filesToCopy)
                File.Delete(ProgramConstants.GamePath + Path.GetFileName(file));
        }

        /// <summary>
        /// Checks whether this renderer enables windowed mode through its
        /// own configuration INI file instead of the game settings INI file.
        /// </summary>
        public bool UsesCustomWindowedOption()
        {
            return !string.IsNullOrEmpty(WindowedModeSection) && 
                !string.IsNullOrEmpty(WindowedModeKey);
        }
    }

    /// <summary>
    /// An exception that is thrown when configuration for DirectDraw wrapper contains
    /// invalid or unexpected settings / data or required settings / data are missing.
    /// </summary>
    class DirectDrawWrapperConfigurationException : Exception
    {
        public DirectDrawWrapperConfigurationException(string message) : base(message)
        {
        }
    }
}
