using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityDistantTerrain;

namespace Boss {
    public class ItemHelper {
        public static void GiveItemToPlayers(List<EntityAlive> players,String itemName) {
            ItemValue itemValue = ItemClass.GetItem(itemName);

            Log.Out($"send reward {players.Count} ");
            if (itemValue != null) {
                players.ForEach(p => {
                    if (p is EntityPlayer player) {
                        GiveItem(player.entityId.ToString(), new ItemStack(itemValue, 1));
                        //GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, $"已为您发放boss击杀奖励", "Server", false, null);
                    }
                });
            }

        }

        public static void GiveItem(String pid, ItemStack itemStack) {
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
    }
}
