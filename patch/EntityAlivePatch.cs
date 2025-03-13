using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using HarmonyLib;
using MiniJSON;
using UnityEngine;
using static Twitch.PubSub.PubSubChannelPointMessage;

namespace Boss.patch {
    [HarmonyPatch(typeof(EntityAlive))]
    public class EntityAlivePatch {
        private static Dictionary<int, BossEnchant> bosses = new Dictionary<int, BossEnchant>();

        [HarmonyPatch("ProcessDamageResponseLocal")]
        [HarmonyPrefix]
        public static bool ProcessDamageResponseLocalPrefix(EntityAlive __instance, ref DamageResponse _dmResponse) {
            if (bosses.ContainsKey(__instance.entityId)) {
                bosses[__instance.entityId].Hurt(ref _dmResponse);
            } else if (BossEnchant.CheckBoss(__instance)) {
                Log.Out("add new boss");
                bosses.Add(__instance.entityId, new BossEnchant(__instance));
                bosses[__instance.entityId].Hurt(ref _dmResponse);
            }
            return true;
        }

        [HarmonyPatch("Kill")]
        [HarmonyPrefix]
        public static bool KillPrefix(EntityAlive __instance, ref DamageResponse _dmResponse) {
            if (__instance != null) {
                if (bosses.ContainsKey(__instance.entityId)) {
                    if (bosses[__instance.entityId].Kill(ref _dmResponse)) return false;
                    bosses.Remove(__instance.entityId);
                }
            }
            return true;
        }
    }
}
