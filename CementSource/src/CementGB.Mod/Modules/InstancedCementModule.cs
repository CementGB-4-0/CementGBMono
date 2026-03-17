using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using CementGB.Utilities;
using UnityEngine;

namespace CementGB.Modules;

public abstract class InstancedCementModule : MonoBehaviour
{
    private static readonly List<InstancedCementModule> ModuleHolder = [];

    protected readonly HarmonyLib.Harmony HarmonyInstance;

    public readonly ManualLogSource Logger;
    protected readonly Assembly ModuleAssembly;

    protected InstancedCementModule()
    {
        HarmonyInstance = new HarmonyLib.Harmony(GetType().FullName);
        ModuleAssembly = Assembly.GetCallingAssembly();
        Logger = new ManualLogSource($"Module_{ModuleAssembly.GetName().Name}");
        SubscribeInternalMethods();
    }

    public static InstancedCementModule? GetModule<T>() where T : InstancedCementModule
    {
        return ModuleHolder.Find(instancedModule => instancedModule.GetType() == typeof(T)) ?? null;
    }

    public static void BootstrapAllCementModulesInAssembly(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        foreach (var moduleType in GetAssemblyModuleTypes(assembly))
        {
            LoggingUtilities.VerboseLog($"Found module type: \"{moduleType.FullName}\" | Bootstrapping module. . .");
            var moduleInstance = BootstrapModule(moduleType);

            if (moduleInstance != null)
                ModuleHolder.Add(moduleInstance);
        }
    }

    private static List<Type> GetAssemblyModuleTypes(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        var assemblyTypes = assembly.GetTypes();

        return [.. assemblyTypes.Where(IsModuleType)];
    }

    private static bool IsModuleType(Type type)
    {
        var baseType = typeof(InstancedCementModule);
        return (baseType.IsAssignableFrom(type) || type == baseType) &&
               type is { IsAbstract: false, IsInterface: false };
    }

    private static InstancedCementModule? BootstrapModule(Type moduleType)
    {
        if (!IsModuleType(moduleType)) return null;

        if (Activator.CreateInstance(moduleType) is not InstancedCementModule instance) return null;
        instance.OnInitialize_Internal();

        return instance;
    }

    protected virtual void OnInitialize()
    {
    }

    protected virtual void DoManualPatches()
    {
        Mod.Logger.LogMessage($"Cement Module {GetType().Name} applying patches. . .");
        HarmonyInstance.PatchAll(ModuleAssembly);
        Mod.Logger.LogMessage("Done!");
    }

    protected virtual void OnUpdate()
    {
    }

    private void SubscribeInternalMethods()
    {
        CementEvents.OnUpdate += OnUpdate;
    }

    private void OnInitialize_Internal()
    {
        DoManualPatches();
        OnInitialize();
    }
}