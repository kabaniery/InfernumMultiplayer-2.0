using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfernumMultiplayer.Core
{
    public abstract class HookTemplate
    {
        protected List<Hook> hooks = new List<Hook>();

        // Он всё равно уникальный для каждого типа
        public abstract void Load();

        public void Unload()
        {
            foreach (Hook hook in hooks)
            {
                hook.Dispose();
            }
            hooks = new List<Hook>();
        }
    }
}
