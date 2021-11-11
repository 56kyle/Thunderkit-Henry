using BepInEx.Configuration;

namespace ModdedSurvivorCamel.Modules
{
    class Config
    {
        //Config entry variables go here. Use these to get the config settings in Mod Manager.
        public static ConfigEntry<bool> characterEnabled;

        public static void ReadConfig()
        {
            //Template
            //ModdedSurvivorCamelPlugin.instance.Config.Bind<bool>(new ConfigDefinition("section", "name"), false, new ConfigDescription("description"));

            //General
            characterEnabled = ModdedSurvivorCamelPlugin.instance.Config.Bind<bool>(new ConfigDefinition("General", "Character Enabled"), true, new ConfigDescription("Set to false to disable ModdedSurvivorCamel."));
        }
    }
}
