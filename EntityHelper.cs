using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Boss {
    public class EntityHelper {

        public static List<EntityAlive> FindNearbyEntities(EntityAlive tagerEntity, float range) {
            return FindNearbyEntities(tagerEntity.position,range);
        }
        public static List<EntityAlive> FindNearbyEntities(Vector3 pos, float range) {
            List<EntityAlive> entities = new List<EntityAlive>();
            foreach (Entity entity in GameManager.Instance.World.Entities.list) {
                if (entity is EntityAlive alive && !alive.IsDead() && alive is EntityPlayer) {
                    if (Vector3.Distance(pos, alive.position) <= range) {
                        entities.Add(alive);
                    }
                }
            }
            return entities;
        }

        private static EntityAlive FindNearestEntity(EntityAlive player, float minDistance) {
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
