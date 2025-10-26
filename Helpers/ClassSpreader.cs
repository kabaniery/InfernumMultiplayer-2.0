using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace InfernumMultiplayer.Helpers
{
    public class ClassSpreader
    {
        public Mod InfernumMod;

        public Dictionary<String, Type> GettedTypes = new Dictionary<string, Type>();

        public ClassSpreader()
        {
            if (!ModLoader.TryGetMod("InfernumMode", out InfernumMod))
            {
                InfernumMod.Logger.Error("ИНФЕРНУМ НЕ ГРУЗИТСЯ, СУКА");
            }
        }
    }
}
