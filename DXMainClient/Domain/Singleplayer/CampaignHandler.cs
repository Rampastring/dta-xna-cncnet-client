using ClientCore;
using Rampastring.Tools;
using System;
using System.Collections.Generic;

namespace DTAClient.Domain.Singleplayer
{
    public class CampaignConfigException : Exception
    {
        public CampaignConfigException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// The central parser and container for campaigns and global variables.
    /// </summary>
    public class CampaignHandler
    {
        private CampaignHandler()
        {
            InitCampaigns();
        }

        private static CampaignHandler _instance;

        public List<CampaignGlobalVariable> GlobalVariables { get; } = new List<CampaignGlobalVariable>();
        public List<Campaign> Campaigns { get; } = new List<Campaign>();

        private List<Mission> missions = new List<Mission>();

        /// <summary>
        /// Singleton pattern. We only need one instance of this class.
        /// </summary>
        public static CampaignHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CampaignHandler();

                return _instance;
            }
        }

        private void InitCampaigns()
        {
            const string CampaignsIniPath = "INI/Campaigns.ini";
            var campaignsIni = new IniFile(ProgramConstants.GamePath + CampaignsIniPath);

            if (campaignsIni == null)
            {
                Logger.Log($"{nameof(CampaignHandler)}: {CampaignsIniPath} not found!");
                return;
            }

            ReadGlobalVariables(campaignsIni);
            ReadCampaigns(campaignsIni);
            SanityCheckCampaigns();
        }

        private void ReadGlobalVariables(IniFile campaignsIni)
        {
            const string GlobalVariablesSectionName = "GlobalVariables";
            IniSection globalVariablesSection = campaignsIni.GetSection(GlobalVariablesSectionName);
            if (globalVariablesSection == null)
            {
                Logger.Log($"{nameof(CampaignHandler)}: [{GlobalVariablesSectionName}] not found from campaign config INI!");
                return;
            }

            int i = 0;
            while (true)
            {
                string variableInternalName = globalVariablesSection.GetStringValue(i.ToString(), null);
                if (string.IsNullOrWhiteSpace(variableInternalName))
                    break;

                var globalVariable = new CampaignGlobalVariable(i, variableInternalName);
                var section = campaignsIni.GetSection(variableInternalName);

                if (section != null)
                {
                    globalVariable.InitFromIniSection(section);
                }
                else
                {
                    Logger.Log($"Section for defined global variable [{variableInternalName}] not found from campaign config INI!");
                }

                GlobalVariables.Add(globalVariable);

                i++;
            }
        }

        private void ReadCampaigns(IniFile campaignsIni)
        {
            const string CampaignsSectionName = "Campaigns";
            IniSection campaignsSection = campaignsIni.GetSection(CampaignsSectionName);
            if (campaignsSection == null)
            {
                Logger.Log($"{nameof(CampaignHandler)}: [{CampaignsSectionName}] not found from campaign config INI!");
                return;
            }

            foreach (var kvp in campaignsSection.Keys)
            {
                string campaignInternalName = kvp.Value;
                if (string.IsNullOrWhiteSpace(campaignInternalName))
                    continue;

                IniSection campaignSection = campaignsIni.GetSection(campaignInternalName);
                if (campaignSection == null)
                {
                    Logger.Log($"Section for defined campaign [{campaignInternalName}] not found from campaign config INI!");
                    continue;
                }

                var campaign = new Campaign(campaignInternalName);
                Campaigns.Add(campaign);

                int i = 0;
                while (true)
                {
                    string missionInternalName = campaignSection.GetStringValue("Mission" + i, null);
                    if (string.IsNullOrWhiteSpace(missionInternalName))
                        break;

                    IniSection missionSection = campaignsIni.GetSection(missionInternalName);
                    if (missionSection == null)
                    {
                        Logger.Log($"Section for defined campaign mission [{missionInternalName}] not found from campaign config INI!");
                        break;
                    }

                    var mission = new Mission(missionSection, true);
                    campaign.Missions.Add(mission);

                    i++;
                }
            }
        }

        /// <summary>
        /// Verifies that all mission and global variable 
        /// references point to valid missions and global variables.
        /// </summary>
        private void SanityCheckCampaigns()
        {
            Campaigns.ForEach(c => c.Missions.ForEach(m => missions.Add(m)));

            foreach (var campaign in Campaigns)
            {
                foreach (var mission in campaign.Missions)
                {
                    // I'm starting to feel a bit like a functional programmer

                    Array.ForEach(mission.UnlockMissions, VerifyMissionExists);
                    Array.ForEach(mission.UsedGlobalVariables, VerifyGlobalVariableExists);
                    Array.ForEach(mission.UnlockGlobalVariables, VerifyGlobalVariableExists);

                    mission.ConditionalMissionUnlocks.ForEach(conditionalMissionUnlock =>
                    {
                        VerifyMissionExists(conditionalMissionUnlock.UnlockMissionName);
                        conditionalMissionUnlock.PrerequisiteGlobalVariableStates.ForEach(gvc => VerifyGlobalVariableExists(gvc.GlobalVariableName));
                    });
                }
            }
        }

        private void VerifyMissionExists(string missionName)
        {
            if (!missions.Exists(m => m.InternalName == missionName))
            {
                throw new CampaignConfigException($"Reference to singleplayer campaign mission '{missionName}' defined, " +
                    $"but the mission itself does not exist. Check the INI/Campaigns.ini file for mistakes." +
                    Environment.NewLine + Environment.NewLine +
                    $"If you are an end-user, please delete the INI/Campaigns.ini file and start the client then, or contact the game/mod authors for support.");
            }
                
        }

        private void VerifyGlobalVariableExists(string globalVariableName)
        {
            if (!GlobalVariables.Exists(m => m.InternalName == globalVariableName))
            {
                throw new CampaignConfigException($"Reference to global variable '{globalVariableName}' defined, " +
                    $"but the global variable itself does not exist. Check the INI/Campaigns.ini file for mistakes." +
                    Environment.NewLine + Environment.NewLine + 
                    $"If you are an end-user, please delete the INI/Campaigns.ini file and start the client then, or contact the game/mod authors for support.");
            }
        }
    }
}
