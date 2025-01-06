using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;

namespace BigPresent;
public class PluginConfig
{
    // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs
    public ConfigEntry<string> ConfigBigPresentSpawnWeight;
    public ConfigEntry<string> ConfigBlacklistEnemies;

    public PluginConfig(ConfigFile cfg)
    {
        ConfigBigPresentSpawnWeight = cfg.Bind("Big Present",
                                "Big Present | SpawnWeight",
                                "Modded:69,Vanilla:69",
                                "The spawn chance weight for Big Present, relative to other existing items.\n" +
                                "Goes up from 0, lower is more rare, 100 and up is very common. \n" +
                                "Allows the use of Moon names in the config.");
        ConfigBlacklistEnemies = cfg.Bind("Big Present",
                                "Big Present | Blacklist Enemies",
                                "",
                                "Comma separated list of enemies to blacklist from the Big Present spawning.");
        ClearUnusedEntries(cfg);
    }

    private void ClearUnusedEntries(ConfigFile cfg) {
        // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
        PropertyInfo orphanedEntriesProp = cfg.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg, null);
        orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
        cfg.Save(); // Save the config file to save these changes
    }
}