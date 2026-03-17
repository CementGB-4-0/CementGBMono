using HarmonyLib;
using UnityEditor.PostProcessing;
using UnityEngine;

namespace CementGB.Modules.NetBeardModule.Patches;

[HarmonyPatch(typeof(GraphicsManager), nameof(GraphicsManager.LoadSettings))]
internal class LowGraphicsOnServerPatch
{
    public static void Postfix(GraphicsManager __instance)
    {
        if (!(NetBeardModule.IsServer && NetBeardModule.LowGraphicsMode))
        {
            return;
        }
        
        var newGraphicsSettings = ScriptableObject.CreateInstance<GraphicsSettings>();
        newGraphicsSettings.AmbientOcclusion = false;
        newGraphicsSettings.AnisotropicFiltering = false;
        newGraphicsSettings.ChromaticAberration = false;
        newGraphicsSettings.PostAntialiasing = GraphicsSettings.URPAntialiasingSetting.Off;
        newGraphicsSettings.Bloom = false;
        newGraphicsSettings.DepthOfField = false;
        newGraphicsSettings.FramerateCap = 60;
        newGraphicsSettings.Grain = false;
        newGraphicsSettings.ScreenSpaceReflection = GraphicsSettings.ScreenSpaceReflectionSetting.Off;
        newGraphicsSettings.Shadows = GraphicsSettings.ShadowSetting.Off;
        newGraphicsSettings.TextureQuality = GraphicsSettings.TextureQualitySetting.Low;
        newGraphicsSettings.Vignette = false;
        newGraphicsSettings.VSync = false;

        __instance.settings.Graphics = newGraphicsSettings;
    }
}