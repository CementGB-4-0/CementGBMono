using System;

namespace CementGB;

public static class CementEvents
{
    public static event Action OnUpdate;


    internal static void InvokeUpdate() => OnUpdate();
}