using InfernumMultiplayer.Content.InfernumOverrides;
using InfernumMultiplayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace InfernumMultiplayer
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class InfernumMultiplayer : Mod
	{
        public static Mod Infernum;
        private static HookTemplate[] hooks = Array.Empty<HookTemplate>();
        public override void Load()
        {
            if (!ModLoader.TryGetMod("InfernumMode", out Infernum))
                return;
            // —юда вставл€ть все переопределени€
            hooks = [new ColosseumPortal(), new DoG()];
            foreach (HookTemplate hook in hooks)
            {
                hook.Load();
            }
        }

        public override void Unload()
        {
            if (hooks == null) {
                Console.Error.WriteLine("hueta");
                return;
            }
            foreach (HookTemplate hook in hooks)
            {
                hook.Unload();
            }

            Infernum = null;
        }
	}
}
