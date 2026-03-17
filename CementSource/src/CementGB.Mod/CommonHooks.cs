using System;
using UnityEngine.SceneManagement;

namespace CementGB;

/// <summary>
///     Provides some useful shorthand hooks for certain in-game events.
/// </summary>
public static class CommonHooks
{
    private static bool _menuFirstBoot;

    /// <summary>
    ///     Fired when the Menu scene loads for the first time in the app's lifespan. Will reset on application quit.
    /// </summary>
    public static event Action? OnMenuFirstBoot;

    internal static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

    private static void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Menu" || _menuFirstBoot) return;

        _menuFirstBoot = true;
        OnMenuFirstBoot?.Invoke();
    }
}