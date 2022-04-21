using ClientCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Rampastring.Tools;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Utilities = Rampastring.Tools.Utilities;

namespace DTAClient.Domain.Multiplayer
{
    public struct ExtraMapPreviewTexture
    {
        public string TextureName;
        public Point Point;

        public ExtraMapPreviewTexture(string textureName, Point point)
        {
            TextureName = textureName;
            Point = point;
        }
    }

    /// <summary>
    /// Custom JSON converter class for <see cref="ExtraMapPreviewTexture"/>.
    /// Necessary because the converter doesn't automatically handle 
    /// converting the XNA Point struct.
    /// </summary>
    public class ExtraMapPreviewTextureConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ExtraMapPreviewTexture);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string textureName = string.Empty;
            int x = 0;
            int y = 0;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var propertyName = (string)reader.Value;
                if (!reader.Read())
                    continue;

                if (propertyName == "TextureName")
                {
                    textureName = serializer.Deserialize<string>(reader);
                }
                else if (propertyName == "X")
                {
                    x = serializer.Deserialize<int>(reader);
                }
                else if (propertyName == "Y")
                {
                    y = serializer.Deserialize<int>(reader);
                }
            }

            return new ExtraMapPreviewTexture(textureName, new Point(x, y));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var extraMapPreviewTexture = (ExtraMapPreviewTexture)value;

            writer.WriteStartObject();
            writer.WritePropertyName("TextureName");
            serializer.Serialize(writer, extraMapPreviewTexture.TextureName);
            writer.WritePropertyName("X");
            serializer.Serialize(writer, extraMapPreviewTexture.Point.X);
            writer.WritePropertyName("Y");
            serializer.Serialize(writer, extraMapPreviewTexture.Point.Y);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// A multiplayer map.
    /// </summary>
    public class Map
    {
        private const int MAX_PLAYERS = 8;

        public Map(string path, bool official)
        {
            BaseFilePath = path;
            Official = official;
        }

        public bool IsLoaded { get; set; }

        /// <summary>
        /// The name of the map.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The maximum amount of players supported by the map.
        /// </summary>
        public int MaxPlayers { get; private set; }

        /// <summary>
        /// The minimum amount of players supported by the map.
        /// </summary>
        public int MinPlayers { get; private set; }

        /// <summary>
        /// Whether to use AmountOfPlayers for limiting the player count of the map.
        /// If false (which is the default), AmountOfPlayers is only used for randomizing
        /// players to starting waypoints.
        /// </summary>
        public bool EnforceMaxPlayers { get; private set; }

        /// <summary>
        /// Controls if the map is meant for a co-operation game mode
        /// (enables briefing logic and forcing options, among others).
        /// </summary>
        public bool IsCoop { get; private set; }

        /// <summary>
        /// If set, this map won't be automatically transferred over CnCNet when
        /// a player doesn't have it.
        /// </summary>
        public bool Official { get; private set; }

        /// <summary>
        /// Contains co-op information.
        /// </summary>
        public CoopMapInfo CoopInfo { get; private set; }

        /// <summary>
        /// The briefing of the map.
        /// </summary>
        public string Briefing { get; private set; }

        /// <summary>
        /// The author of the map.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// The calculated SHA1 of the map.
        /// </summary>
        public string SHA1 { get; set; }

        /// <summary>
        /// The path to the map file.
        /// </summary>
        public string BaseFilePath { get; private set; }

        /// <summary>
        /// Returns the complete path to the map file.
        /// Includes the game directory in the path.
        /// </summary>
        public string CompleteFilePath => ProgramConstants.GamePath + BaseFilePath + ".map";

        /// <summary>
        /// The file name of the preview image.
        /// </summary>
        public string PreviewPath { get; private set; }

        /// <summary>
        /// If set, this map cannot be played on Skirmish.
        /// </summary>
        public bool MultiplayerOnly { get; private set; }

        /// <summary>
        /// If set, this map cannot be played with AI players.
        /// </summary>
        public bool HumanPlayersOnly { get; private set; }

        /// <summary>
        /// If set, players are forced to random starting locations on this map.
        /// </summary>
        public bool ForceRandomStartLocations { get; private set; }

        /// <summary>
        /// If set, players are forced to different teams on this map.
        /// </summary>
        public bool ForceNoTeams { get; private set; }

        /// <summary>
        /// The name of an extra INI file in INI\Map Code\ that should be
        /// embedded into this map's INI code when a game is started.
        /// </summary>
        public string ExtraININame { get; private set; }

        /// <summary>
        /// The game modes that the map is listed for.
        /// </summary>
        public string[] GameModes;

        /// <summary>
        /// The forced UnitCount for the map. -1 means none.
        /// </summary>
        private int UnitCount = -1;

        /// <summary>
        /// The forced starting credits for the map. -1 means none.
        /// </summary>
        private int Credits = -1;

        private int NeutralHouseColor = -1;

        private int SpecialHouseColor = -1;

        private int Bases = -1;

        private string[] localSize;

        private string[] actualSize;

        private IniFile mapIni;

        private List<string> waypoints = new List<string>();

        /// <summary>
        /// The pixel coordinates of the map's player starting locations.
        /// </summary>
        private List<Point> startingLocations;

        public Texture2D PreviewTexture { get; set; }


        public List<KeyValuePair<string, bool>> ForcedCheckBoxValues = new List<KeyValuePair<string, bool>>(0);
        public List<KeyValuePair<string, int>> ForcedDropDownValues = new List<KeyValuePair<string, int>>(0);

        private List<ExtraMapPreviewTexture> extraTextures = new List<ExtraMapPreviewTexture>(0);
        public List<ExtraMapPreviewTexture> GetExtraMapPreviewTextures() => extraTextures;

        private List<KeyValuePair<string, string>> ForcedSpawnIniOptions = new List<KeyValuePair<string, string>>(0);


        private readonly static object _locker = new object();

        public bool PreParse(IniFile iniFile)
        {
            string baseSectionName = iniFile.GetStringValue(BaseFilePath, "BaseSection", string.Empty);

            if (!string.IsNullOrEmpty(baseSectionName))
                iniFile.CombineSections(baseSectionName, BaseFilePath);

            var section = iniFile.GetSection(BaseFilePath);

            Name = section.GetStringValue("Description", "Unnamed map");
            Author = section.GetStringValue("Author", "Unknown author");
            GameModes = section.GetStringValue("GameModes", "Default").Split(',');
            PreviewPath = Path.GetDirectoryName(BaseFilePath) + "/" +
                    section.GetStringValue("PreviewImage", Path.GetFileNameWithoutExtension(BaseFilePath) + ".png");
            ExtraININame = section.GetStringValue("ExtraININame", string.Empty);

            // Parse forced options
            string forcedOptionsSections = iniFile.GetStringValue(BaseFilePath, "ForcedOptions", string.Empty);

            if (!string.IsNullOrEmpty(forcedOptionsSections))
            {
                string[] sections = forcedOptionsSections.Split(',');
                foreach (string foSection in sections)
                    ParseForcedOptions(iniFile, foSection);
            }

            string forcedSpawnIniOptionsSections = iniFile.GetStringValue(BaseFilePath, "ForcedSpawnIniOptions", string.Empty);

            if (!string.IsNullOrEmpty(forcedSpawnIniOptionsSections))
            {
                string[] sections = forcedSpawnIniOptionsSections.Split(',');
                foreach (string fsioSection in sections)
                    ParseSpawnIniOptions(iniFile, fsioSection);
            }
            

            // ReadExtraTextures(section);

            return true;
        }

        public void Load(IniFile mapCacheIni)
        {
            SHA1 = Utilities.CalculateSHA1ForFile(CompleteFilePath);

            if (!SetInfoFromCacheINI(mapCacheIni))
            {
                SetInfoFromMap(CompleteFilePath);
                WriteInfoToCacheINI(mapCacheIni);
            }
        }

        public bool SetInfoFromCacheINI(IniFile iniFile)
        {
            try
            {
                // GetSection is not thread-safe
                IniSection section = null;
                lock (_locker)
                {
                    section = iniFile.GetSection(SHA1);
                }

                if (section == null)
                    return false;

                if (!Official)
                {
                    Name = section.GetStringValue("Description", "Unnamed map");
                    Author = section.GetStringValue("Author", "Unknown author");
                    GameModes = section.GetStringValue("GameModes", "Default").Split(',');
                }

                MinPlayers = section.GetIntValue("MinPlayers", 0);
                MaxPlayers = section.GetIntValue("MaxPlayers", 0);
                EnforceMaxPlayers = section.GetBooleanValue("EnforceMaxPlayers", false);
                Briefing = section.GetStringValue("Briefing", string.Empty).Replace("@", Environment.NewLine);
                SHA1 = Utilities.CalculateSHA1ForFile(CompleteFilePath);
                IsCoop = section.GetBooleanValue("IsCoop", false);
                Credits = section.GetIntValue("Credits", -1);
                UnitCount = section.GetIntValue("UnitCount", -1);
                NeutralHouseColor = section.GetIntValue("NeutralColor", -1);
                SpecialHouseColor = section.GetIntValue("SpecialColor", -1);
                MultiplayerOnly = section.GetBooleanValue("MultiplayerOnly", false);
                HumanPlayersOnly = section.GetBooleanValue("HumanPlayersOnly", false);
                ForceRandomStartLocations = section.GetBooleanValue("ForceRandomStartLocations", false);
                ForceNoTeams = section.GetBooleanValue("ForceNoTeams", false);
                
                string bases = section.GetStringValue("Bases", string.Empty);
                if (!string.IsNullOrEmpty(bases))
                {
                    Bases = Convert.ToInt32(Conversions.BooleanFromString(bases, false));
                }

                if (IsCoop)
                {
                    CoopInfo = new CoopMapInfo();
                    CoopInfo.ReadFromINI(section);
                }

                localSize = section.GetStringValue("LocalSize", "0,0,0,0").Split(',');
                actualSize = section.GetStringValue("Size", "0,0,0,0").Split(',');

                for (int i = 0; i < MAX_PLAYERS; i++)
                {
                    string waypoint = section.GetStringValue("Waypoint" + i, string.Empty);

                    if (string.IsNullOrEmpty(waypoint))
                        break;

                    waypoints.Add(waypoint);
                }

#if !WINDOWSGL
                if (UserINISettings.Instance.PreloadMapPreviews)
                    PreviewTexture = LoadPreviewTexture();
#endif

                ForcedCheckBoxValues = JsonConvert.DeserializeObject<List<KeyValuePair<string, bool>>>(section.GetStringValue("ForcedCheckBoxValues", string.Empty));
                ForcedDropDownValues = JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(section.GetStringValue("ForcedDropDownValues", string.Empty));
                extraTextures = JsonConvert.DeserializeObject<List<ExtraMapPreviewTexture>>(section.GetStringValue("ExtraMapPreviewTextures", string.Empty));
                ForcedSpawnIniOptions = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(section.GetStringValue("ForcedSpawnIniOptions", string.Empty));

                IsLoaded = true;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Setting info for " + BaseFilePath + " failed! Reason: " + ex.Message);
                return false;
            }
        }

        public List<Point> GetStartingLocationPreviewCoords(Point previewSize)
        {
            if (startingLocations == null)
            {
                startingLocations = new List<Point>();

                foreach (string waypoint in waypoints)
                {
                    startingLocations.Add(GetWaypointCoords(waypoint, actualSize, localSize, previewSize));
                }
            }

            return startingLocations;
        }

        public Point MapPointToMapPreviewPoint(Point mapPoint, Point previewSize)
        {
            return GetIsoTilePixelCoord(mapPoint.X, mapPoint.Y, actualSize, localSize, previewSize);
        }


        /// <summary>
        /// Loads map information from a TS/RA2 map INI file.
        /// Returns true if succesful, otherwise false.
        /// </summary>
        /// <param name="path">The full path to the map INI file.</param>
        public bool SetInfoFromMap(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                IniFile iniFile = new IniFile();
                iniFile.FileName = path;
                iniFile.AddSection("Basic");
                iniFile.AddSection("Map");
                iniFile.AddSection("Waypoints");
                iniFile.AddSection("Preview");
                iniFile.AddSection("PreviewPack");
                iniFile.AddSection("ForcedOptions");
                iniFile.AddSection("ForcedSpawnIniOptions");
                iniFile.AddSection("Structures");
                iniFile.AddSection("INISystem");
                iniFile.AllowNewSections = false;

                iniFile.Parse();

                mapIni = iniFile;

                var basicSection = iniFile.GetSection("Basic");

                if (!Official)
                {
                    Name = basicSection.GetStringValue("Name", "Unnamed map"); // Official maps have their name defined in MPMaps.ini
                    Author = basicSection.GetStringValue("Author", "Unknown author");
                }

                if (!Official)
                {
                    string gameModesString = basicSection.GetStringValue("GameModes", string.Empty);
                    if (string.IsNullOrEmpty(gameModesString))
                    {
                        gameModesString = basicSection.GetStringValue("GameMode", "Default");
                    }

                    GameModes = gameModesString.Split(',');

                    if (GameModes.Length == 0)
                    {
                        Logger.Log("Custom map " + path + " has no game modes!");
                        return false;
                    }

                    for (int i = 0; i < GameModes.Length; i++)
                    {
                        string gameMode = GameModes[i].Trim();
                        GameModes[i] = gameMode.Substring(0, 1).ToUpperInvariant() + gameMode.Substring(1);
                    }
                }

                MinPlayers = basicSection.GetIntValue("ClientMinPlayer", 0);
                MaxPlayers = basicSection.GetIntValue("MaxPlayer", 0);
                MaxPlayers = basicSection.GetIntValue("ClientMaxPlayer", MaxPlayers);
                EnforceMaxPlayers = basicSection.GetBooleanValue("EnforceMaxPlayers", !Official);
                Briefing = basicSection.GetStringValue("Briefing", string.Empty).Replace("@", Environment.NewLine);
                IsCoop = basicSection.GetBooleanValue("IsCoop", basicSection.GetBooleanValue("IsCoopMission", IsCoop));
                Credits = basicSection.GetIntValue("Credits", -1);
                UnitCount = basicSection.GetIntValue("UnitCount", -1);
                NeutralHouseColor = basicSection.GetIntValue("NeutralColor", -1);
                SpecialHouseColor = basicSection.GetIntValue("SpecialColor", -1);
                HumanPlayersOnly = basicSection.GetBooleanValue("HumanPlayersOnly", false);
                ForceRandomStartLocations = basicSection.GetBooleanValue("ForceRandomStartLocations", false);
                ForceNoTeams = basicSection.GetBooleanValue("ForceNoTeams", false);

                if (string.IsNullOrWhiteSpace(PreviewPath))
                    PreviewPath = Path.ChangeExtension(path.Substring(ProgramConstants.GamePath.Length), ".png");

                MultiplayerOnly = basicSection.GetBooleanValue("ClientMultiplayerOnly", false);

                string bases = basicSection.GetStringValue("Bases", string.Empty);
                if (!string.IsNullOrEmpty(bases))
                {
                    Bases = Convert.ToInt32(Conversions.BooleanFromString(bases, false));
                }

                ReadExtraTextures(basicSection);
                bool autoFillExtraTextures = basicSection.GetBooleanValue("AutoFillExtraTextures", !IsCoop);
                if (autoFillExtraTextures)
                    AutoFillExtraTextures(mapIni);

                if (IsCoop)
                {
                    CoopInfo = new CoopMapInfo();
                    CoopInfo.ReadFromINI(basicSection);
                }

                localSize = iniFile.GetStringValue("Map", "LocalSize", "0,0,0,0").Split(',');
                actualSize = iniFile.GetStringValue("Map", "Size", "0,0,0,0").Split(',');

                int startingWaypointCount = basicSection.GetIntValue("StartingWaypointCount", MaxPlayers);

                for (int i = 0; i < MAX_PLAYERS && i < startingWaypointCount; i++)
                {
                    string waypoint = mapIni.GetStringValue("Waypoints", i.ToString(), string.Empty);

                    if (string.IsNullOrEmpty(waypoint))
                        break;

                    waypoints.Add(waypoint);
                }

                ParseForcedOptions(iniFile, "ForcedOptions");
                ParseSpawnIniOptions(iniFile, "ForcedSpawnIniOptions");

                IsLoaded = true;

                return true;
            }
            catch
            {
                Logger.Log("Loading map " + path + " failed!");
                return false;
            }
        }

        /// <summary>
        /// Goes through structures on the map and places extra textures on the preview
        /// based on the map file's contents (for example, oil refinery icons on oil refineries).
        /// </summary>
        private void AutoFillExtraTextures(IniFile mapIni)
        {
            var structuresSection = mapIni.GetSection("Structures");
            foreach (var kvp in structuresSection.Keys)
            {
                string[] parts = kvp.Value.Split(',');
                // ID=Owner,BuildingTypeID,HP,X,Y,...
                if (parts.Length < 5)
                    continue;

                string id = parts[1];
                if (id != "OILREFN" && id != "STEX")
                    continue;

                int x = Conversions.IntFromString(parts[3], 0);
                int y = Conversions.IntFromString(parts[4], 0);

                extraTextures.Add(new ExtraMapPreviewTexture(id + ".png", new Point(x, y)));
            }
        }

        private void ReadExtraTextures(IniSection section)
        {
            int i = 0;
            while (true)
            {
                // Format example:
                // ExtraTexture0=oilderrick.png,200,150

                string value = section.GetStringValue("ExtraTexture" + i, null);
                if (string.IsNullOrWhiteSpace(value))
                    break;

                string[] parts = value.Split(',');
                if (parts.Length != 3)
                {
                    Logger.Log($"Invalid format for ExtraTexture{i} in map " + BaseFilePath);
                    continue;
                }

                bool success = int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x);
                success &= int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y);
                extraTextures.Add(new ExtraMapPreviewTexture(parts[0], new Point(x, y)));

                i++;
            }
        }

        private void ParseForcedOptions(IniFile iniFile, string forcedOptionsSection)
        {
            List<string> keys = iniFile.GetSectionKeys(forcedOptionsSection);

            if (keys == null)
            {
                if (Official)
                    Logger.Log("Non-existent ForcedOptions section \"" + forcedOptionsSection + "\" in map " + BaseFilePath);

                return;
            }

            foreach (string key in keys)
            {
                string value = iniFile.GetStringValue(forcedOptionsSection, key, String.Empty);

                int intValue = 0;
                if (Int32.TryParse(value, out intValue))
                {
                    ForcedDropDownValues.Add(new KeyValuePair<string, int>(key, intValue));
                }
                else
                {
                    ForcedCheckBoxValues.Add(new KeyValuePair<string, bool>(key, Conversions.BooleanFromString(value, false)));
                }
            }
        }

        private void ParseSpawnIniOptions(IniFile forcedOptionsIni, string spawnIniOptionsSection)
        {
            List<string> spawnIniKeys = forcedOptionsIni.GetSectionKeys(spawnIniOptionsSection);

            if (spawnIniKeys == null)
            {
                if (Official)
                {
                    Logger.Log($"Non-existent ForcedSpawnIniOptions section {spawnIniOptionsSection} " +
                        $"specified for map {BaseFilePath}");
                }
                    
                return;
            }

            foreach (string key in spawnIniKeys)
            {
                ForcedSpawnIniOptions.Add(new KeyValuePair<string, string>(key, 
                    forcedOptionsIni.GetStringValue(spawnIniOptionsSection, key, String.Empty)));
            }
        }

        /// <summary>
        /// Loads and returns the map preview texture.
        /// </summary>
        public Texture2D LoadPreviewTexture()
        {
            if (File.Exists(ProgramConstants.GamePath + PreviewPath))
                return AssetLoader.LoadTextureUncached(PreviewPath);

            if (!Official)
            {
                // Extract preview from the map itself

                if (mapIni == null)
                {
                    mapIni = new IniFile();
                    mapIni.FileName = CompleteFilePath;
                    mapIni.AddSection("Preview");
                    mapIni.AddSection("PreviewPack");
                    mapIni.AllowNewSections = false;

                    mapIni.Parse();
                }

                System.Drawing.Bitmap preview = MapPreviewExtractor.ExtractMapPreview(mapIni);

                if (preview != null)
                {
                    Texture2D texture = AssetLoader.TextureFromImage(preview);
                    if (texture != null)
                        return texture;
                }
            }

            return AssetLoader.CreateTexture(Color.Black, 10, 10);
        }

        public IniFile GetMapIni()
        {
            var mapIni = new IniFile(CompleteFilePath);

            if (!string.IsNullOrEmpty(ExtraININame))
            {
                var extraIni = new IniFile(ProgramConstants.GamePath + "INI/Map Code/" + ExtraININame);
                IniFile.ConsolidateIniFiles(mapIni, extraIni);
            }

            return mapIni;
        }

        public void ApplySpawnIniCode(IniFile spawnIni, int totalPlayerCount, 
            int aiPlayerCount, int coopDifficultyLevel)
        {
            foreach (KeyValuePair<string, string> key in ForcedSpawnIniOptions)
                spawnIni.SetStringValue("Settings", key.Key, key.Value);

            if (Credits != -1)
                spawnIni.SetIntValue("Settings", "Credits", Credits);

            if (UnitCount != -1)
                spawnIni.SetIntValue("Settings", "UnitCount", UnitCount);

            int neutralHouseIndex = totalPlayerCount + 1;
            int specialHouseIndex = totalPlayerCount + 2;

            if (IsCoop)
            {
                var allyHouses = CoopInfo.AllyHouses;
                var enemyHouses = CoopInfo.EnemyHouses;

                int multiId = totalPlayerCount + 1;
                foreach (var houseInfo in allyHouses.Concat(enemyHouses))
                {
                    spawnIni.SetIntValue("HouseHandicaps", "Multi" + multiId, coopDifficultyLevel);
                    spawnIni.SetIntValue("HouseCountries", "Multi" + multiId, houseInfo.Side);
                    spawnIni.SetIntValue("HouseColors", "Multi" + multiId, houseInfo.Color);
                    spawnIni.SetIntValue("SpawnLocations", "Multi" + multiId, houseInfo.StartingLocation);

                    multiId++;
                }

                for (int i = 0; i < allyHouses.Count; i++)
                {
                    int aMultiId = totalPlayerCount + i + 1;

                    int allyIndex = 0;

                    // Write alliances
                    for (int pIndex = 0; pIndex < totalPlayerCount + allyHouses.Count; pIndex++)
                    {
                        int allyMultiIndex = pIndex;

                        if (pIndex == aMultiId - 1)
                            continue;

                        spawnIni.SetIntValue("Multi" + aMultiId + "_Alliances",
                            "HouseAlly" + HouseAllyIndexToString(allyIndex), allyMultiIndex);
                        spawnIni.SetIntValue("Multi" + (allyMultiIndex + 1) + "_Alliances",
                            "HouseAlly" + HouseAllyIndexToString(totalPlayerCount + i - 1), aMultiId - 1);
                        allyIndex++;
                    }
                }

                for (int i = 0; i < enemyHouses.Count; i++)
                {
                    int eMultiId = totalPlayerCount + allyHouses.Count + i + 1;

                    int allyIndex = 0;

                    // Write alliances
                    for (int enemyIndex = 0; enemyIndex < enemyHouses.Count; enemyIndex++)
                    {
                        int allyMultiIndex = totalPlayerCount + allyHouses.Count + enemyIndex;

                        if (enemyIndex == i)
                            continue;

                        spawnIni.SetIntValue("Multi" + eMultiId + "_Alliances",
                            "HouseAlly" + HouseAllyIndexToString(allyIndex), allyMultiIndex);
                        allyIndex++;
                    }
                }

                spawnIni.SetIntValue("Settings", "AIPlayers", aiPlayerCount +
                    allyHouses.Count + enemyHouses.Count);

                neutralHouseIndex += allyHouses.Count + enemyHouses.Count;
                specialHouseIndex += allyHouses.Count + enemyHouses.Count;
            }

            if (NeutralHouseColor > -1)
                spawnIni.SetIntValue("HouseColors", "Multi" + neutralHouseIndex, NeutralHouseColor);

            if (SpecialHouseColor > -1)
                spawnIni.SetIntValue("HouseColors", "Multi" + specialHouseIndex, SpecialHouseColor);

            if (Bases > -1)
                spawnIni.SetBooleanValue("Settings", "Bases", Convert.ToBoolean(Bases));
        }

        private static string HouseAllyIndexToString(int index)
        {
            string[] houseAllyIndexStrings = new string[]
            {
                "One",
                "Two",
                "Three",
                "Four",
                "Five",
                "Six",
                "Seven"
            };

            return houseAllyIndexStrings[index];
        }

        public string GetSizeString()
        {
            if (actualSize == null || actualSize.Length < 4)
                return "Not available";
            return actualSize[2] + "x" + actualSize[3];
        }

        /// <summary>
        /// Converts a waypoint's coordinate string into pixel coordinates on the preview image.
        /// </summary>
        /// <returns>The waypoint's location on the map preview as a point.</returns>
        private static Point GetWaypointCoords(string waypoint, string[] actualSizeValues, string[] localSizeValues,
            Point previewSizePoint)
        {
            int xCoordIndex = waypoint.Length - 3;

            int isoTileY = Convert.ToInt32(waypoint.Substring(0, xCoordIndex));
            int isoTileX = Convert.ToInt32(waypoint.Substring(xCoordIndex));

            return GetIsoTilePixelCoord(isoTileX, isoTileY, actualSizeValues, localSizeValues, previewSizePoint);
        }

        private static Point GetIsoTilePixelCoord(int isoTileX, int isoTileY, string[] actualSizeValues, string[] localSizeValues, Point previewSizePoint)
        {
            int rx = isoTileX - isoTileY + Convert.ToInt32(actualSizeValues[2]) - 1;
            int ry = isoTileX + isoTileY - Convert.ToInt32(actualSizeValues[2]) - 1;

            int pixelPosX = rx * MainClientConstants.MAP_CELL_SIZE_X / 2;
            int pixelPosY = ry * MainClientConstants.MAP_CELL_SIZE_Y / 2;

            pixelPosX = pixelPosX - (Convert.ToInt32(localSizeValues[0]) * MainClientConstants.MAP_CELL_SIZE_X);
            pixelPosY = pixelPosY - (Convert.ToInt32(localSizeValues[1]) * MainClientConstants.MAP_CELL_SIZE_Y);

            // Calculate map size
            int mapSizeX = Convert.ToInt32(localSizeValues[2]) * MainClientConstants.MAP_CELL_SIZE_X;
            int mapSizeY = Convert.ToInt32(localSizeValues[3]) * MainClientConstants.MAP_CELL_SIZE_Y;

            double ratioX = Convert.ToDouble(pixelPosX) / mapSizeX;
            double ratioY = Convert.ToDouble(pixelPosY) / mapSizeY;

            int pixelX = Convert.ToInt32(ratioX * previewSizePoint.X);
            int pixelY = Convert.ToInt32(ratioY * previewSizePoint.Y);

            return new Point(pixelX, pixelY);
        }

        public void WriteInfoToCacheINI(IniFile cacheIni)
        {
            var section = new IniSection(SHA1);

            section.SetStringValue("Description", Name);
            section.SetIntValue("MaxPlayers", MaxPlayers);
            section.SetIntValue("MinPlayers", MinPlayers);
            section.SetBooleanValue("EnforceMaxPlayers", EnforceMaxPlayers);
            section.SetStringValue("Author", Author);
            section.SetStringValue("PreviewPath", PreviewPath);
            section.SetBooleanValue("MultiplayerOnly", MultiplayerOnly);
            section.SetBooleanValue("HumanPlayersOnly", HumanPlayersOnly);
            section.SetBooleanValue("ForceRandomStartLocations", ForceRandomStartLocations);
            section.SetBooleanValue("ForceNoTeams", ForceNoTeams);
            if (!string.IsNullOrWhiteSpace(ExtraININame))
                section.SetStringValue("ExtraININame", ExtraININame);
            section.SetStringValue("GameModes", string.Join(",", GameModes));

            if (UnitCount > -1)
                section.SetIntValue("UnitCount", UnitCount);

            if (Credits > -1)
                section.SetIntValue("Credits", Credits);

            if (NeutralHouseColor > -1)
                section.SetIntValue("NeutralHouseColor", NeutralHouseColor);

            if (SpecialHouseColor > -1)
                section.SetIntValue("SpecialHouseColor", SpecialHouseColor);

            if (Bases > -1)
                section.SetIntValue("Bases", Bases);

            section.SetStringValue("LocalSize", string.Join(",", localSize));
            section.SetStringValue("Size", string.Join(",", actualSize));
            for (int i = 0; i < waypoints.Count; i++)
            {
                section.SetStringValue("Waypoint" + i, waypoints[i]);
            }

            section.SetBooleanValue("IsCoop", IsCoop);
            if (CoopInfo != null)
                CoopInfo.WriteToINI(section);

            if (!string.IsNullOrWhiteSpace(Briefing))
                section.SetStringValue("Briefing", Briefing.Replace(Environment.NewLine, "@"));

            section.SetStringValue("ForcedCheckBoxValues", JsonConvert.SerializeObject(ForcedCheckBoxValues));

            section.SetStringValue("ForcedDropDownValues", JsonConvert.SerializeObject(ForcedDropDownValues));

            section.SetStringValue("ExtraMapPreviewTextures", JsonConvert.SerializeObject(extraTextures));

            section.SetStringValue("ForcedSpawnIniOptions", JsonConvert.SerializeObject(ForcedSpawnIniOptions));

            lock (_locker)
            {
                cacheIni.RemoveSection(SHA1);
                cacheIni.AddSection(section);
            }
        }
    }
}
