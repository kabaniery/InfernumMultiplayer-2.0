using InfernumMultiplayer.Core;
using InfernumMultiplayer.Helpers;
using log4net.Repository.Hierarchy;
using MonoMod.RuntimeDetour;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernumMultiplayer.Content.InfernumOverrides
{
    public class ColosseumPortal : HookTemplate
    {
        private delegate void orig_NearbyEffects(object self, int i, int j, bool closer);
        private delegate void hook_NearbyEffects(orig_NearbyEffects orig, object self, int i, int j, bool closer);

        private static double NearPortal = 0;

        private static Type WorldSaveSystem;
        private static Type LostColosseum;
        private static MethodInfo IsActive;
        private static MethodInfo Enter;

        private static void NearbyEffectsPatch(orig_NearbyEffects orig, object self, int i, int j, bool closer)
        {
            var isPortalOpen = WorldSaveSystem.GetProperty("HasOpenedLostColosseumPortal", BindingFlags.Public | BindingFlags.Static);
            if (isPortalOpen is not null && !(bool)isPortalOpen.GetValue(null))
            {
                NearPortal = 0;
                return;
            }

            if (NearPortal <= 6f)
            {
                if (closer)
                    NearPortal += 0.028f;
                return;
            }

            // Вообще не нужно, но на всякий
            if (Main.netMode != NetmodeID.SinglePlayer && Main.netMode != NetmodeID.MultiplayerClient)
            {
                return;
            }

            Player localPlayer = Main.LocalPlayer;

            if (Math.Abs(localPlayer.position.X - i * 16) > 32 || Math.Abs(localPlayer.position.Y - j * 16) > 32)
            {
                return;
            }
            NearPortal = 0;


            if ((bool)IsActive.Invoke(null, null))
            {
                SubworldSystem.Exit();
            } else
            {
                Enter.Invoke(null, null);
            }

        }

        public override void Load()
        {

            // Найдём тип и метод внутри сборки Infernum
            Type source = MetaManager.getType("InfernumMode.Content.Tiles.Colosseum.ColosseumPortal"); // точное полное имя типа

            WorldSaveSystem = MetaManager.getType("InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem");
            LostColosseum = MetaManager.getType("InfernumMode.Content.Subworlds.LostColosseum");

            if (!ModLoader.TryGetMod("SubworldLibrary", out var swl)) {
                Console.Error.WriteLine("SubworldLibrary не грузится, сука");
            }

            Type SubworldSystemType = swl?.Code.GetType("SubworldLibrary.SubworldSystem", false);

            MethodInfo SubwordSystemIsActiveGeneric = SubworldSystemType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "IsActive" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
            IsActive = SubwordSystemIsActiveGeneric.MakeGenericMethod(LostColosseum);

            MethodInfo SubworldSystemEnterGeneric = SubworldSystemType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Enter" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
            Enter = SubworldSystemEnterGeneric.MakeGenericMethod(LostColosseum);


            MethodInfo target = source?.GetMethod("NearbyEffects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (target is null) return;

            hooks.Add(new Hook(target, (hook_NearbyEffects)NearbyEffectsPatch));
        }
    }
}
