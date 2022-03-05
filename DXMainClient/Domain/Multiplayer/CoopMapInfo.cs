using Rampastring.Tools;
using System;
using System.Collections.Generic;

namespace DTAClient.Domain.Multiplayer
{
    public class CoopMapInfo
    {
        public List<CoopHouseInfo> EnemyHouses = new List<CoopHouseInfo>();
        public List<CoopHouseInfo> AllyHouses = new List<CoopHouseInfo>();
        public List<int> DisallowedPlayerSides = new List<int>();
        public List<int> DisallowedPlayerColors = new List<int>();


        private List<CoopHouseInfo> GetGenericHouseInfo(IniSection iniSection, string keyName)
        {
            var houseList = new List<CoopHouseInfo>();

            for (int i = 0; ; i++)
            {
                string[] houseInfo = iniSection.GetStringValue(keyName + i, string.Empty).Split(
                    new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (houseInfo.Length == 0)
                    break;

                int[] info = Conversions.IntArrayFromStringArray(houseInfo);
                var chInfo = new CoopHouseInfo(info[0], info[1], info[2]);

                houseList.Add(new CoopHouseInfo(info[0], info[1], info[2]));
            }

            return houseList;
        }

        public void ReadFromINI(IniSection iniSection)
        {
            EnemyHouses = GetGenericHouseInfo(iniSection, "EnemyHouse");
            AllyHouses = GetGenericHouseInfo(iniSection, "AllyHouse");
            DisallowedPlayerSides = iniSection.GetListValue("DisallowedPlayerSides", ',', s => int.Parse(s));
            DisallowedPlayerColors = iniSection.GetListValue("DisallowedPlayerColors", ',', s => int.Parse(s));
        }

        public void WriteToINI(IniSection iniSection)
        {
            for (int i = 0; i < EnemyHouses.Count; i++)
            {
                var enemyHouseInfo = EnemyHouses[i];
                iniSection.SetStringValue($"EnemyHouse{i}", $"{enemyHouseInfo.Side},{enemyHouseInfo.Color},{enemyHouseInfo.StartingLocation}");
            }

            for (int i = 0; i < AllyHouses.Count; i++)
            {
                var allyHouseInfo = AllyHouses[i];
                iniSection.SetStringValue($"AllyHouse{i}", $"{allyHouseInfo.Side},{allyHouseInfo.Color},{allyHouseInfo.StartingLocation}");
            }

            iniSection.SetStringValue("DisallowedPlayerSides", string.Join(",", DisallowedPlayerSides));
            iniSection.SetStringValue("DisallowedPlayerColors", string.Join(",", DisallowedPlayerColors));
        }
    }
}
