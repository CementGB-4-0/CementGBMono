using System.Reflection;
using CementGB.Utilities;
using HarmonyLib;
using UnityEngine;

namespace CementGB.Patches;

internal static class DisplayCreditsPatch
{
    [HarmonyPatch(typeof(DisplayCredits), nameof(DisplayCredits.ApplyText))]
    private static class ApplyText
    {
        private static TextAsset? textAsset;

        private static void Prefix(DisplayCredits __instance)
        {
            if (textAsset) return;

            textAsset = new TextAsset(
                $"{EmbeddedUtilities.ReadEmbeddedText(Assembly.GetExecutingAssembly(), "CementGB.Mod.Assets.CreditsText.txt")}\n\n{__instance.textFile.text}");
            __instance.textFile = textAsset;
        }
    }
}