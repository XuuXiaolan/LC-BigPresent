using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System.IO;
using BigPresent.Utils;

namespace BigPresent;
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalLib.Plugin.ModGUID)] 
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger = null!;
    internal static PluginConfig BoundConfig { get; private set; } = null!;

    private void Awake()
    {
        Logger = base.Logger;

        // If you don't want your mod to use a configuration file, you can remove this line, Configuration.cs, and other references.
        BoundConfig = new PluginConfig(base.Config);

        // This should be ran before Network Prefabs are registered.
        InitializeNetworkBehaviours();

        // We load the asset bundle that should be next to our DLL file, with the specified name.
        // You may want to rename your asset bundle from the AssetBundle Browser in order to avoid an issue with
        // asset bundle identifiers being the same between multiple bundles, allowing the loading of only one bundle from one mod.
        // In that case also remember to change the asset bundle copying code in the csproj.user file.

        string itemBundleName = "bigpresentassets";
        var ItemAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), "Assets", itemBundleName));
        if (ItemAssets == null) {
            Logger.LogError($"Failed to load custom assets.");
            return;
        }

        RegisterBigPresent(ItemAssets);

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void RegisterBigPresent(AssetBundle ModAssets)
    {
        var BigPresent = ModAssets.LoadAsset<Item>("BigPresentDef");
        ContentHandler.Instance.RegisterScrapWithConfig(BoundConfig.ConfigBigPresentSpawnWeight.Value, BigPresent);
   }

    private static void InitializeNetworkBehaviours()
    {
        // See https://github.com/EvaisaDev/UnityNetcodePatcher?tab=readme-ov-file#preparing-mods-for-patching
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                {
                    method.Invoke(null, null);
                }
            }
        }
    } 
}