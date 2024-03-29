﻿using ClientCore;
using Rampastring.Tools;
using System;
using System.Collections.Generic;

namespace DTAClient.Domain.Multiplayer
{
    /// <summary>
    /// A multiplayer game mode.
    /// </summary>
    public class GameMode
    {
        public GameMode(string name)
        {
            Name = name;
            Initialize();
        }

        private const string BASE_INI_PATH = "INI/Map Code/";
        private const string SPAWN_INI_OPTIONS_SECTION = "ForcedSpawnIniOptions";

        /// <summary>
        /// The internal (INI) name of the game mode.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user-interface name of the game mode.
        /// </summary>
        public string UIName { get; private set; }

        public string Description { get; private set; }

        public bool MultiplayerOnly { get; private set; }

        public bool HumanPlayersOnly { get; private set; }

        public bool ForceRandomStartLocations { get; private set; }

        public bool ForceNoTeams { get; private set; }
        public bool DifficultyBasedAINames { get; private set; }


        public List<int> DisallowedPlayerSides = new List<int>();

        private string mapCodeININame;

        private string forcedOptionsSection;

        public List<Map> Maps = new List<Map>();

        public List<KeyValuePair<string, bool>> ForcedCheckBoxValues = new List<KeyValuePair<string, bool>>();
        public List<KeyValuePair<string, int>> ForcedDropDownValues = new List<KeyValuePair<string, int>>();

        private List<KeyValuePair<string, string>> ForcedSpawnIniOptions = new List<KeyValuePair<string, string>>();

        public int CoopDifficultyLevel { get; set; }

        public void Initialize()
        {
            IniFile mpMapsIni = new IniFile(ProgramConstants.GamePath + ClientConfiguration.Instance.MPMapsIniPath);

            CoopDifficultyLevel = mpMapsIni.GetIntValue(Name, "CoopDifficultyLevel", 0);
            UIName = mpMapsIni.GetStringValue(Name, "UIName", Name);
            Description = mpMapsIni.GetStringValue(Name, "Description", string.Empty);
            MultiplayerOnly = mpMapsIni.GetBooleanValue(Name, "MultiplayerOnly", false);
            HumanPlayersOnly = mpMapsIni.GetBooleanValue(Name, "HumanPlayersOnly", false);
            ForceRandomStartLocations = mpMapsIni.GetBooleanValue(Name, "ForceRandomStartLocations", false);
            ForceNoTeams = mpMapsIni.GetBooleanValue(Name, "ForceNoTeams", false);
            DifficultyBasedAINames = mpMapsIni.GetBooleanValue(Name, "DifficultyBasedAINames", false);
            forcedOptionsSection = mpMapsIni.GetStringValue(Name, "ForcedOptions", string.Empty);
            mapCodeININame = mpMapsIni.GetStringValue(Name, "MapCodeININame", Name + ".ini");

            string[] disallowedSides = mpMapsIni
                .GetStringValue(Name, "DisallowedPlayerSides", string.Empty)
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string sideIndex in disallowedSides)
                DisallowedPlayerSides.Add(int.Parse(sideIndex));

            ParseForcedOptions(mpMapsIni);

            ParseSpawnIniOptions(mpMapsIni);
        }

        private void ParseForcedOptions(IniFile forcedOptionsIni)
        {
            if (string.IsNullOrEmpty(forcedOptionsSection))
                return;

            List<string> keys = forcedOptionsIni.GetSectionKeys(forcedOptionsSection);

            if (keys == null)
                return;

            foreach (string key in keys)
            {
                string value = forcedOptionsIni.GetStringValue(forcedOptionsSection, key, string.Empty);

                int intValue = 0;
                if (int.TryParse(value, out intValue))
                {
                    ForcedDropDownValues.Add(new KeyValuePair<string, int>(key, intValue));
                }
                else
                {
                    ForcedCheckBoxValues.Add(new KeyValuePair<string, bool>(key, Conversions.BooleanFromString(value, false)));
                }
            }
        }

        private void ParseSpawnIniOptions(IniFile forcedOptionsIni)
        {
            string section = forcedOptionsIni.GetStringValue(Name, "ForcedSpawnIniOptions", Name + SPAWN_INI_OPTIONS_SECTION);

            List<string> spawnIniKeys = forcedOptionsIni.GetSectionKeys(section);

            if (spawnIniKeys == null)
                return;

            foreach (string key in spawnIniKeys)
            {
                ForcedSpawnIniOptions.Add(new KeyValuePair<string, string>(key, 
                    forcedOptionsIni.GetStringValue(section, key, string.Empty)));
            }
        }

        public void ApplySpawnIniCode(IniFile spawnIni)
        {
            if (DifficultyBasedAINames)
                spawnIni.SetBooleanValue("Settings", "DifficultyBasedAINames", DifficultyBasedAINames);

            foreach (KeyValuePair<string, string> key in ForcedSpawnIniOptions)
                spawnIni.SetStringValue("Settings", key.Key, key.Value);
        }

        public IniFile GetMapRulesIniFile()
        {
            return new IniFile(ProgramConstants.GamePath + BASE_INI_PATH + mapCodeININame);
        }
    }
}
