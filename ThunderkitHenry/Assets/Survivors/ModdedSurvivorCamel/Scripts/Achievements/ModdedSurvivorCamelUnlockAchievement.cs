﻿using RoR2;
using System;
using UnityEngine;
using ModdedSurvivorCamel.Modules;
using RoR2.Networking;

namespace ModdedSurvivorCamel.Achievements
{
   internal class ModdedSurvivorCamelUnlock : UnlockableCreator.ModdedSurvivorCamelAchievement
    {
        // this prefix variable is ROBVALE_ModdedSurvivorCaps_BODY_UNLOCK_ by default.
        public override string Prefix => ModdedSurvivorCamelPlugin.developerPrefix + Tokens.ModdedSurvivorLowerPrefix + "UNLOCK_";

        // Requires Tokens created in tokens.cs, as they are displayed to the player.
        public override string AchievementNameToken => Prefix + "SURVIVOR_NAME";
        public override string AchievementDescToken => Prefix + "SURVIVOR_DESC";

        // Used for referencing and must be unique to the achievement.
        public override string AchievementIdentifier => Prefix + "SURVIVOR_ID";
        public override string UnlockableIdentifier => Prefix + "SURVIVOR_REWARD_ID";

        // If PrerequisiteUnlockableIdentifier matches the name of an existing AchievementIdentifier, 
        // you need to have the Achievement unlocked in order to be able to unlock this achievement.
        // In this case this ID doesn't (shouldn't) match anything, so no required achievement in order to unlock this.
        public override string PrerequisiteUnlockableIdentifier => Prefix + "SURVIVOR_PREREQ_ID";

        // make sure this matches the NAME of the UnlockableDef you create for the achievement.
        public override UnlockableDef UnlockableDef => Modules.Assets.mainAssetBundle.LoadAsset<UnlockableDef>("Characters.ModdedSurvivorCamel");
        public override Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texModdedSurvivorCamelAchievement");

        public override void Initialize()
        {
            UnlockableCreator.AddUnlockable<ModdedSurvivorCamelUnlock>(true);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            GameNetworkManager.onServerSceneChangedGlobal += StageCheck;
        }
        public override void OnUninstall()
        {
            base.OnUninstall();

            GameNetworkManager.onServerSceneChangedGlobal -= StageCheck;
        }

        private void StageCheck(string sceneName)
        {
            if (sceneName == "blackbeach" || sceneName == "blackbeach2")
            {
                base.Grant();
            }
        }

        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString(AchievementNameToken),
            Language.GetString(AchievementDescToken)
        });
        public override Func<string> GetUnlocked => () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString(AchievementNameToken),
            Language.GetString(AchievementDescToken)
        });


    }
}