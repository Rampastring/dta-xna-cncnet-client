using Rampastring.Tools;
using System.Collections.Generic;

namespace DTAClient.Domain.Singleplayer
{
    /// <summary>
    /// Represents a mission unlock that is gated behind a 
    /// condition based on global variable states at the end of a mission.
    /// 
    /// Example: Mission 4B of Dawn of the Tiberium Age's The Toxic Diversion campaign
    /// should only be unlocked after beating mission #3 if the player did not save
    /// the civilians in mission #2 and then killed the civilians in mission #3.
    /// </summary>
    public class ConditionalMissionUnlock
    {
        public ConditionalMissionUnlock(string unlockMissionName)
        {
            UnlockMissionName = unlockMissionName;
        }

        public string UnlockMissionName { get; }
        public List<GlobalVariableCondition> PrerequisiteGlobalVariableStates { get; } = new List<GlobalVariableCondition>(1);

        public static ConditionalMissionUnlock FromString(string str)
        {
            // Syntax:
            // UnlockMissionName|Variable1Name:Variable1State,Variable2Name:Variable2State,...

            string[] parts = str.Split('|');
            if (parts.Length != 2)
            {
                Logger.Log("Parsing MissionUnlockCondition failed: " + str);
                return null;
            }

            var missionUnlockCondition = new ConditionalMissionUnlock(parts[0]);

            string[] conditions = parts[1].Split(',');
            foreach (string condition in conditions)
            {
                string[] conditionParts = condition.Split(':');
                if (conditionParts.Length != 2)
                {
                    Logger.Log("Parsing MissionUnlockCondition failed: " + str);
                    return null;
                }

                var globalVariableCondition = new GlobalVariableCondition(conditionParts[0], Conversions.BooleanFromString(conditionParts[1], false));
                missionUnlockCondition.PrerequisiteGlobalVariableStates.Add(globalVariableCondition);
            }

            return missionUnlockCondition;
        }
    }
}
