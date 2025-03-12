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
        //private static BossDamageConfig config;
        //private static FastTags<TagGroup.Global> bossGroup;
        //private static FastTags<TagGroup.Global> exBossGroup;

        //private static FastTags<TagGroup.Global> bossGroup = FastTags<TagGroup.Global>.Parse("boss,exboss");
        //private static FastTags<TagGroup.Global> exBossGroup = FastTags<TagGroup.Global>.Parse("exboss");

        /*
        private static Dictionary<int, int> bosses = new Dictionary<int, int>();
        //{player , time}
        private static Dictionary<int, long> cooldown = new Dictionary<int, long>();
        private static Dictionary<int, int> playerDamage = new Dictionary<int, int>();

        private static Dictionary<int, long> bossCooldown = new Dictionary<int, long>();
        private static Dictionary<int, int> bossDamage = new Dictionary<int, int>();

        private static Dictionary<int, bool> skills = new Dictionary<int, bool>();

        private static Dictionary<int, long> msg = new Dictionary<int, long>();
        */



        [HarmonyPatch("ProcessDamageResponseLocal")]
        [HarmonyPrefix]
        public static bool ProcessDamageResponseLocalPrefix(EntityAlive __instance, ref DamageResponse _dmResponse) {
            if (bosses.ContainsKey(__instance.entityId)) {
                bosses[__instance.entityId].Hurt(ref _dmResponse);
            }
            return false;
        }
        /*
        public static bool ProcessDamageResponseLocalPrefixOld(EntityAlive __instance, ref DamageResponse _dmResponse) {
            if (config == null) {
                Log.Error("boss 配置加载失败！");
                return true;
            }
            if (__instance != null && _dmResponse.Source != null) {
                if (__instance.HasAnyTags(bossGroup)) {
                    EntityPlayer player = GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityPlayer;
                    if (player == null) {

                        __instance.OnUpdateEntity();
                        return false;
                    }
                    //初始

                    int pid = player.entityId;
                    int bid = __instance.entityId;

                    if (!cooldown.ContainsKey(pid)) cooldown.Add(pid, 0);
                    if (!playerDamage.ContainsKey(pid)) playerDamage.Add(pid, 0);

                    if (!bosses.ContainsKey(bid)) bosses.Add(bid, 0);
                    if (!bossCooldown.ContainsKey(bid)) bossCooldown.Add(bid, 0);
                    if (!bossDamage.ContainsKey(bid)) cooldown.Add(bid, 0);
                    if (!skills.ContainsKey(bid)) skills.Add(bid, false);

                    if (!msg.ContainsKey(bid)) msg.Add(bid, 0);


                    long t = cooldown[pid];
                    DateTimeOffset now = DateTimeOffset.UtcNow;
                    long nowTime = now.ToUnixTimeSeconds();
                    if (nowTime - t > config.CooldownSeconds) {
                        cooldown[pid] = nowTime;
                        playerDamage[pid] = 0; //重置伤害
                    } else {
                        if (playerDamage[pid] >= config.DamageLimit) {
                            __instance.OnUpdateEntity();
                            return false;
                        }
                    }

                    int scaleDamage = _dmResponse.Strength > config.MaxDamage ? config.MaxDamage : _dmResponse.Strength;
                    bool isExBoss = __instance.HasAnyTags(exBossGroup);
                    if (isExBoss) scaleDamage = (int)(scaleDamage * config.ExBossDamageMultiplier);

                    playerDamage[pid] += scaleDamage;

                    if (isExBoss) {
                        long cd = bossCooldown[bid];
                        if (nowTime - cd > config.BossDamageCooldown) {
                            bossCooldown[bid] = nowTime;
                            bossDamage[bid] = 0;
                        } else {
                            int dm = scaleDamage + bossDamage[bid];
                            bossDamage[bid] = dm;
                            if (dm >= config.BossDamageLimit) scaleDamage = (int)(scaleDamage * config.BossDamageCooldownScale);
                        }
                    }

                    t = msg[bid];
                    if (nowTime - t > 10) {
                        msg[bid] = nowTime;
                        List<EntityAlive> players = FindNearbyEntities(__instance, config.KillRange);
                        List<int> ps = new List<int>();
                        players.ForEach(p => ps.Add(p.entityId));
                        GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, $"Boss({bid})剩余生命值：{__instance.GetMaxHealth() - bosses[bid]}", ps, EMessageSender.Server);
                        //GameManager.Instance.ChatMessageClient(EChatType.Global, bid, "client", null, EMessageSender.Server);
                    }

                    //???
                    _dmResponse.Strength = scaleDamage;

                    int d = bosses[bid] + scaleDamage;
                    bosses[bid] = d;
                    if (d >= __instance.GetMaxHealth()) {

                        bosses.Remove(bid);
                        bossCooldown.Remove(bid);
                        bossDamage.Remove(bid);
                        skills.Remove(bid);
                        msg.Remove(bid);
                        cooldown.Clear();
                        playerDamage.Clear();

                        giveReward(FindNearbyEntities(__instance, config.KillRange));
                        __instance.ClientKill(_dmResponse);
                        __instance.Health = 1;
                        _dmResponse.Strength = 2;

                        return true;
                    }



                    System.Random random = new System.Random();
                    int randomNumber = random.Next(100);
                    if (randomNumber < (isExBoss ? config.ExBossTeleportChance : config.TeleportChance)) {
                        __instance.SetPosition(player.GetPosition());
                    }

                    if (isExBoss) {
                        if (!skills[bid] && d >= __instance.GetMaxHealth() * 0.5) {
                            List<EntityAlive> players = FindNearbyEntities(__instance, config.KillRange);
                            foreach (EntityAlive ent in players) {
                                ent.Health = 1;
                                ent.OnUpdateEntity();
                            }
                        }
                    }
                    __instance.OnUpdateEntity();
                    return false;
                }
            }
            return true;
        }
        */
        /*
        private static void giveReward(List<EntityAlive> players) {
            ItemValue itemValue = ItemClass.GetItem(config.Reward);

            Log.Out($"send reward {players.Count} " );
            if (itemValue != null) {
                players.ForEach(p => {
                    if (p is EntityPlayer player) {
                        GiveItem(player.entityId.ToString(),new ItemStack(itemValue, 1));
                        //GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, $"已为您发放boss击杀奖励", "Server", false, null);
                    }
                });
            }


        }
        */
        /*
        public void GiveItemToServerPlayer(string playerName, string itemName, int count) {
            ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(playerName, true, false); //尝试获取玩家信息
            if (clientInfo != null) {
                string command = $"give {playerName} {itemName} {count}"; //或者 string command = $"give self {itemName} {count}";
                ExecuteCommandServer(command, clientInfo);
            } else {
                Log.Error("无法找到玩家: " + playerName);
            }
        }
        public void ExecuteCommandServer(string command, ClientInfo clientInfo = null) {
            if (GameManager.Instance != null) {
                //GameManager.Instance.adminTools
                //GameManager.Instance(command, clientInfo);
            }
        }
        */

        /*
        public static bool ProcessDamageResponseLocalPrefix(EntityAlive __instance, ref DamageResponse _dmResponse) {
            if (__instance != null && _dmResponse.Source != null) {
                //判断标签
                if (__instance.HasAnyTags(bossGroup)) {
                    EntityPlayer player = GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityPlayer;
                    if (player == null) return false;
                    long t = 0;
                    if (cooldown.ContainsKey(player.entityId)) t = cooldown[player.entityId];
                    DateTimeOffset now = DateTimeOffset.UtcNow;

                    if (now.ToUnixTimeSeconds() - t > 20) cooldown[player.entityId] = now.ToUnixTimeSeconds();
                    else {
                        //检查记录
                        if(playerDamage.ContainsKey(player.entityId) && playerDamage[player.entityId] >= damageLimit) return false;
                    }


                    //更改伤害
                    int scaleDamage = _dmResponse.Strength > maxDamage ? maxDamage : _dmResponse.Strength;
                    bool isExBoss = __instance.HasAnyTags(exBossGroup);
                    if (isExBoss) scaleDamage = (int)(scaleDamage * 0.5);

                    playerDamage[player.entityId] += scaleDamage;

                    //设置伤害
                    _dmResponse.Strength = scaleDamage;

                    //记录伤害
                    if (bosses.ContainsKey(__instance.entityId)) {
                        int d = bosses[__instance.entityId] + scaleDamage;
                        bosses[__instance.entityId] = d;
                        if(d >= __instance.GetMaxHealth()) {
                            //累计伤害达到最大生命值时 kill
                            __instance.Kill(_dmResponse);
                            return false;
                        }
                    } else {
                        bosses.Add(__instance.entityId, scaleDamage);
                    }

                    System.Random random = new System.Random(); // 使用系统时钟作为种子

                    // 生成一个非负随机整数
                    int randomNumber = random.Next(100);
                    if (randomNumber < (isExBoss ? 20:10) ) __instance.SetPosition(player.position);

                    if (isExBoss) {
                        player.Health -= (int)(scaleDamage * 0.01);
                    }

                    return false;
                }
            }
            return true;
        }
        */
        [HarmonyPatch("Kill")]
        [HarmonyPrefix]
        public static bool KillPrefix(EntityAlive __instance, ref DamageResponse _dmResponse) {
            if (__instance != null) {
                if (bosses.ContainsKey(__instance.entityId)) {
                    if (bosses[__instance.entityId].Kill(ref _dmResponse)) return true;
                    bosses.Remove(__instance.entityId);
                }
                //if (GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) is EntityPlayer player) {
                //    Log.Out(" entity killed is player");
                    /*
                    var cp = GameManager.Instance.World.GetLocalPlayerFromID(player.clientEntityId);

                    if (cp.inventory.AddItem(new ItemStack(ItemClass.GetItem(config.Reward), 1))) {
                        Log.Out($"{true}");
                    } else {
                        Log.Out($"{false}");
                    }
                    
                    */
                    //GiveItem(player.entityId.ToString(),new ItemStack(ItemClass.GetItem(config.Reward), 1));
                    //player.OnUpdateEntity();
                    //SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync("give 171 questRewardAmmoCraftingBundle 2", null);
                //}
            



                /*
                if (bosses.ContainsKey(__instance.entityId)) {
                    int damage = bosses[__instance.entityId];
                    if (damage < __instance.GetMaxHealth()) {
                        return false;
                    }
                }
                */
            }
            return true;
        }
        /*
        private static void GiveItem(String pid, ItemStack itemStack) {

            World world = GameManager.Instance.World;
            var playersDict = world.Players.dict;
            ClientInfo cInfo = ConsoleHelper.ParseParamEntityIdToClientInfo(pid);
            if (cInfo != null && playersDict.TryGetValue(cInfo.entityId, out EntityPlayer player)
                && player.IsSpawned() && !player.IsDead()) {
                var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData() {
                    entityClass = EntityClass.FromString("item"),
                    id = EntityFactory.nextEntityID++,
                    itemStack = itemStack,
                    pos = player.position,
                    rot = new Vector3(20F, 0F, 20F),
                    lifetime = 60F,
                    belongsPlayerId = cInfo.entityId
                });

                world.SpawnEntityInWorld(entityItem);
                cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, cInfo.entityId));
                world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);

            }
        }
        */
        //[HarmonyPatch("OnUpdateEntity")]
        //[HarmonyPrefix]
        /*
        public static bool OnUpdateEntityPrefix(EntityAlive __instance) {
            // 只在服务器上，并且是 Boss 的情况下阻止
            if (!GameManager.Instance.World.IsRemote() && __instance.HasAnyTags(bossGroup)) {
                
                if (bosses.ContainsKey(__instance.entityId)) {
                    int damage = bosses[__instance.entityId];
                    if (damage < __instance.GetMaxHealth()) {
                        return false;
                    }

                }
            }
            return true; // 其他情况允许 Update 执行
        }
        */
        /*
        [HarmonyPatch(new Type[]{
                                typeof(DamageSource),
                                typeof(int),
                                typeof(bool),
                                typeof(Vector3),
                                typeof(float)
                                })]
        */
        //[HarmonyPatch("DamageEntity")]
        //[HarmonyPrefix]
        public static bool DamageEntityPrefix(EntityAlive __instance, ref DamageSource _damageSource,ref int _strength,ref bool _criticalHit,ref float _impulseScale) {
            Log.Out(" -->> DamageEntityPrefix 1");
            if (__instance != null && _damageSource != null) {
                Log.Out(" -->>DamageEntityPrefix");
            }
            return true;
        }


        private static List<EntityAlive> FindNearbyEntities(EntityAlive player, float range) {
            List<EntityAlive> entities = new List<EntityAlive>();
            foreach (Entity entity in GameManager.Instance.World.Entities.list) {
                if (entity is EntityAlive alive && !alive.IsDead() && alive is EntityPlayer) {
                    if (Vector3.Distance(player.position, alive.position) <= range) {
                        entities.Add(alive);
                    }
                }
            }
            return entities;
        }
        private static EntityAlive FindNearestEntity(EntityAlive player) {
            float minDistance = 50f; // 限制搜索范围
            EntityAlive nearestEntity = null;

            foreach (Entity entity in GameManager.Instance.World.Entities.list) {
                if (entity is EntityAlive alive && !alive.IsDead() && alive is EntityPlayer) { // 关键修改：排除所有 EntityPlayer
                    float distance = Vector3.Distance(player.position, alive.position);
                    if (distance < minDistance) {
                        minDistance = distance;
                        nearestEntity = alive;
                    }
                }
            }
            return nearestEntity;
        }
    }
}
