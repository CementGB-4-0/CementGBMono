using System;
using BepInEx;
using UnityEngine;
using Color = System.Drawing.Color;

namespace CementSource;

[BepInPlugin("com.cementgb.cement", "CementGB", "0.0.1")]
public class Mod : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogMessage("CementGB Loaded");
    }
}