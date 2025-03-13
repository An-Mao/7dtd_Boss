using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ItemActionAttack;
using static UnityDistantTerrain;

namespace Boss {
    public class BossEnchant {
        public static BossDamageConfig config;
        public static FastTags<TagGroup.Global> bossGroup;
        public static FastTags<TagGroup.Global> exBossGroup;

        EntityAlive boss;
        int bid;
        bool canSkill = false;
        int health = 0;
         DamageRecord DR = new DamageRecord();
        Dictionary<int, DamageRecord> PDR = new Dictionary<int, DamageRecord>();
        long msgTime = 0;
        public BossEnchant(EntityAlive entityAlive) {
            this.boss = entityAlive;
            this.bid = this.boss.entityId;
        }

        public void Hurt(ref DamageResponse damageResponse) {
            if (IsBoss() && damageResponse.Source != null) {
                int pid = damageResponse.Source.getEntityId();
                if (GameManager.Instance.World.GetEntity(pid) is EntityPlayer player) {
                    //初始化伤害记录
                    if (!PDR.ContainsKey(pid)) PDR.Add(pid, new DamageRecord());

                    //获取当前时间
                    DateTimeOffset now = DateTimeOffset.UtcNow;
                    long nowTime = now.ToUnixTimeSeconds();

                    //判断玩家是否处于冷却，冷却则修正伤害为0，并返回
                    DamageRecord _pdr = PDR[pid];
                    if (nowTime - _pdr.time > config.CooldownSeconds) {
                        _pdr.time = nowTime;
                        _pdr.damage = 0;

                    } else if (_pdr.damage >= config.DamageLimit) {
                        damageResponse.Strength = 0;
                        return;
                    }

                    //进行伤害修正
                    int scaleDamage = damageResponse.Strength > config.MaxDamage ? config.MaxDamage : damageResponse.Strength;
                    //判断是否为EX
                    bool isExBoss = this.boss.HasAnyTags(exBossGroup);
                    //额外修正
                    if (isExBoss) {
                        scaleDamage = (int)(scaleDamage * config.ExBossDamageMultiplier);
                        //累计修正
                        if (nowTime - DR.time > config.BossDamageCooldown) {
                            DR.time = nowTime;
                            DR.damage = 0;
                        } else if (DR.damage >= config.BossDamageLimit) {
                            scaleDamage = (int)(scaleDamage * config.BossDamageCooldownScale);
                        }
                        //记录伤害
                        DR.damage += scaleDamage;
                    }

                    //记录玩家伤害
                    _pdr.damage += scaleDamage;
                    //记录总伤害
                    this.health += scaleDamage;
                    Log.Out($"health: {this.health}");
                    //发送血量信息
                    if (nowTime - msgTime > 10) {
                        msgTime = nowTime;
                        List<EntityAlive> players = EntityHelper.FindNearbyEntities(this.boss, config.KillRange);
                        List<int> ps = new List<int>();
                        players.ForEach(p => ps.Add(p.entityId));
                        MsgHelper.sendMsg($"Boss({bid})剩余生命值：{this.boss.GetMaxHealth() - health}", ps);
                    }

                    damageResponse.Strength = scaleDamage;
                    //处理死亡
                    if (this.health >= this.boss.GetMaxHealth()) {
                        if (isExBoss) {
                            ItemHelper.GiveItemToPlayers(EntityHelper.FindNearbyEntities(this.boss, config.KillRange), config.Reward);
                        }
                        this.boss.Health = 1;
                        damageResponse.Strength = 2;
                        return;
                    }


                    //触发传送技能
                    if (config.ExBossTeleportChance > 0 || config.TeleportChance > 0) {
                        System.Random random = new System.Random();
                        int randomNumber = random.Next(100);
                        if (randomNumber < (isExBoss ? config.ExBossTeleportChance : config.TeleportChance)) {
                            this.boss.SetPosition(player.GetPosition());
                        }
                    }
                    //触发EX技能
                    if (isExBoss) {
                        if (!this.canSkill && this.health >= this.boss.GetMaxHealth() * 0.5)
                            EntityHelper.FindNearbyEntities(this.boss, config.KillRange).ForEach(p => p.Health = 1);
                        
                    }


                }
            }
        }
        public void Attack(DamageResponse damageResponse) { }
        public bool Kill(ref DamageResponse damageResponse) { 
            return false;
        }

        public bool IsBoss() {
            return boss != null && boss.IsAlive() && boss.HasAnyTags(bossGroup);
        }

        public static bool CheckBoss(EntityAlive entity) {
            return entity != null && entity.IsAlive() && entity.HasAnyTags(bossGroup);
        }

    }
    public class DamageRecord {

        public long time { get; set; }
        public int damage { get; set; }
        public DamageRecord():this(0,0) {
        }
        public DamageRecord(long time, int damage) {
            this.time = time;
            this.damage = damage;
        }
    }
}
