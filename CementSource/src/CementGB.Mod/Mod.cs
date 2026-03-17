using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using CementGB.Modules;
using CementGB.Utilities;

namespace CementGB;

/// <summary>
///     The main entrypoint for Cement. This is where everything initializes from. Public members include important paths
///     and MelonMod overrides.
/// </summary>
[BepInPlugin(MyPluginInfo.Guid, MyPluginInfo.Name, MyPluginInfo.Version)]
public class Mod : BaseUnityPlugin
{
    /// <summary>
    ///     Cement's UserData path ("Gang Beasts\UserData\CementGB"). Created in <see cref="OnInitializeMelon" />.
    /// </summary>
    public static readonly string UserDataPath = Path.Combine(Paths.ConfigPath, "CementGB");

    /// <summary>
    ///     Publicized BepInEx Logger
    /// </summary>
    public static ManualLogSource Logger { get; private set; }

    public static readonly string ModulesPath =
        Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "..", "UserLibs",
            "CementGBModules"));

    public static string? MapArg => CommandLineParser.Instance.GetValueForKey("-map", false);
    public static string? ModeArg => CommandLineParser.Instance.GetValueForKey("-mode", false);
    public static bool DebugArg => Environment.GetCommandLineArgs().Contains("-debug");

    /// <summary>
    ///     Fires when Cement loads. Since Cement's MelonPriority is set to a very low number, the mod should initialize before
    ///     any other.
    /// </summary>
    public void Awake()
    {
        Logger = base.Logger;
        
        // Setup directories and folder structure
        FileStructure();

        // Initialize static classes that need initializing
        CementPreferences.Initialize();
        if (!CementPreferences.VerboseMode)
        {
            Logger.LogMessage("Verbose Mode disabled! Enable verbose mode in UserData/CementGB/CementGB.cfg for more detailed logging.");
        }

        CommonHooks.Initialize();
    }

    /// <summary>
    ///     Fires just before Cement is unloaded from the game. Usually this happens when the application closes/crashes, but
    ///     mods can also be unloaded manually.
    ///     This method saves MelonPreferences for Cement via <c>CementPreferences.Deinitialize()</c>, which is an internal
    ///     method.
    /// </summary>
    public void OnApplicationQuit()
    {
        CementPreferences.Deinitialize();
    }

    /// <summary>
    ///     Fires after the first few Unity MonoBehaviour.Start() methods. Creates components that couldn't be loaded before
    ///     Unity's runtime started.
    /// </summary>
    public void Start()
    {
        foreach (var file in Directory.GetFiles(ModulesPath, "*.dll", SearchOption.AllDirectories))
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                InstancedCementModule.BootstrapAllCementModulesInAssembly(assembly);
            }
            catch
            {
                Logger.LogError($"Failed to auto-load CementGB modules from assembly file {Path.GetFileName(file)}!");
            }
        }
    }

    private static void FileStructure()
    {
        _ = Directory.CreateDirectory(UserDataPath);
        _ = Directory.CreateDirectory(ModulesPath);
    }

    public void Update()
    {
        MainThreadDispatcher.DispatchActions();
        CementEvents.InvokeUpdate();
    }
}