using HarmonyLib;
using Coatsink.Platform;
using Coatsink.Platform.Systems.UI;
using CS.CorePlatform;
using CS.CorePlatform.CSPlatform;
using GB.Core;
using static Coatsink.Platform.UI;

namespace CementGB.Modules.NetBeardModule.Patches;

internal static class MultiRunPatches
{
    [HarmonyPatch(typeof(CStoCorePlatform), nameof(CStoCorePlatform.OnInitializeComplete))]
    private static class OnInitializeCompletePatch
    {
        private static bool Prefix(CStoCorePlatform __instance, ref TaskResult<bool> obj)
        {
            if (NetBeardModule.IsServer)
            {
                obj = new TaskResult<bool>();
                obj.Complete(1u); // ??
            }

            return true;
        }
    }
}