using Rampastring.Tools;

namespace DTAClient.Domain.Singleplayer
{
    /// <summary>
    /// Represents a mission unlock that is gated behind the bonus
    /// selected for a mission.
    /// </summary>
    public class BonusDependentMissionUnlock
    {
        public BonusDependentMissionUnlock(string unlockMissionName, string bonusName)
        {
            UnlockMissionName = unlockMissionName;
            BonusName = bonusName;
        }

        public string UnlockMissionName { get; }
        public string BonusName { get; }

        public static BonusDependentMissionUnlock FromString(string str)
        {
            // Syntax:
            // UnlockMissionName|BonusName

            string[] parts = str.Split('|');
            if (parts.Length != 2)
            {
                Logger.Log("Parsing BonusDependentMissionUnlock failed: " + str);
                return null;
            }

            return new BonusDependentMissionUnlock(parts[0], parts[1]);
        }
    }
}
