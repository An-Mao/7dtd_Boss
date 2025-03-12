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
    }
}
