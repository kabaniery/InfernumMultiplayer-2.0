using InfernumMultiplayer.Core;
using InfernumMultiplayer.Helpers;
using System;
using System.Reflection;
using Terraria;

namespace InfernumMultiplayer.Content.InfernumOverrides
{
    public class DoG : HookTemplate
    {
        private delegate void orig_DoPassiveFlyMovement(NPC npc, ref float jawRotation, ref float chompEffectsCountdown, bool flyHigherUp);
        private delegate void hook_DoPassiveFlyMovement(orig_DoPassiveFlyMovement orig, NPC npc, ref float jawRotation, ref float chompEffectsCountdown, bool flyHigherUp);

        private delegate void orig_DoAggressiveFlyMovement(NPC npc, Player target, bool dontChompYet, bool chomping, ref float jawRotation, ref float chompEffectsCountdown, ref float universalFightTimer, ref float flyAcceleration);
        private delegate void hook_DoAggressiveFlyMovement(orig_DoAggressiveFlyMovement orig, NPC npc, Player target, bool dontChompYet, bool chomping, ref float jawRotation, ref float chompEffectsCountdown, ref float universalFightTimer, ref float flyAcceleration);

        private static int aggro_delay = 0;
        private static void DoPassiveFlyMovementPatch(orig_DoPassiveFlyMovement orig, NPC npc, ref float jawRotation, ref float chompEffectsCountdown, bool flyHigherUp)
        {
            orig(npc, ref jawRotation, ref chompEffectsCountdown, flyHigherUp);
            aggro_delay = 30;
        }

        private static void DoAggressiveFlyMovementPatch(orig_DoAggressiveFlyMovement orig, NPC npc, Player target, bool dontChompYet, bool chomping, ref float jawRotation, ref float chompEffectsCountdown, ref float universalFightTimer, ref float flyAcceleration)
        {
            if (aggro_delay > 0)
            {
                aggro_delay--;
                return;
            }
            orig(npc, target, dontChompYet, chomping, ref jawRotation, ref chompEffectsCountdown, ref universalFightTimer, ref flyAcceleration);
        }

        public override void Load()
        {
            Type source = MetaManager.getType("InfernumMode.Content.BehaviorOverrides.BossAIs.DoG.DoGPhase2HeadBehaviorOverride");

            MethodInfo target1 = source.GetMethod("DoPassiveFlyMovement", BindingFlags.Static | BindingFlags.Public);
            MethodInfo target2 = source.GetMethod("DoAggressiveFlyMovement", BindingFlags.Static | BindingFlags.Public);

            if (target1 == null || target2 == null)
            {
                return;
            }

            hooks.Add(new MonoMod.RuntimeDetour.Hook(target1, (hook_DoPassiveFlyMovement)DoPassiveFlyMovementPatch));
            hooks.Add(new MonoMod.RuntimeDetour.Hook(target2, (hook_DoAggressiveFlyMovement)DoAggressiveFlyMovementPatch));
        }
    }
}
