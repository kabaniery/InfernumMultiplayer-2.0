using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

/// <summary>
/// Summary description for Class1
/// </summary>
public static class InfernumHooks
{
	private static Hook _bereftPortalHook;
    private static Type WorldSaveSystem;
    private static Type LostColosseum;
    private static double NearPortal = 0;

    private delegate void orig_NearbyEffects(object self, int i, int j, bool closer);
    private delegate void hook_NearbyEffects(orig_NearbyEffects orig, object self, int i, int j, bool closer);
	private static void NearbyEffectsPatch(orig_NearbyEffects orig, object self, int i, int j, bool closer)
	{
        var isPortalOpen = WorldSaveSystem.GetProperty("HasOpenedLostColosseumPortal", BindingFlags.Public | BindingFlags.Static);
        if (isPortalOpen is not null && (bool)isPortalOpen.GetValue(null))
        {
            NearPortal = 0;
            return;
        }

        if (NearPortal <= 3f)
        {
            if (closer)
                NearPortal += 0.028f;
            return;
        }

        Player localPlayer = Main.LocalPlayer;

        localPlayer.QuickSpawnItem(localPlayer.GetSource_GiftOrReward(), ItemID.FallenStar, 5);

    }

    public static void Load()
    {
        if (!ModLoader.TryGetMod("InfernumMode", out var infernum)) return;

        // Найдём тип и метод внутри сборки Infernum
        var t = infernum.Code?.GetType("InfernumMode.Content.Tiles.Colosseum.ColosseumPortal"); // точное полное имя типа
        WorldSaveSystem = infernum.Code?.GetType("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem");
        LostColosseum = infernum.Code?.GetType("InfernumMode.Content.Subworlds.LostColosseum");
        var m = t?.GetMethod("NearbyEffects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (m is null) return;

        _bereftPortalHook = new Hook(m, (hook_NearbyEffects)NearbyEffectsPatch);
    }

    public static void Unload()
    {
        _bereftPortalHook?.Dispose();
        _bereftPortalHook = null;
    }
}
