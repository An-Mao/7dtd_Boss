using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boss {
    public class BossDamageConfig {
        public int BossDamageCooldown { get;set; }
        public int BossDamageLimit { get; set; }
        public double BossDamageCooldownScale { get; set; }
        public int MaxDamage { get; set; }
        public int DamageLimit { get; set; }
        public int CooldownSeconds { get; set; }
        public float ExBossDamageMultiplier { get; set; }
        public int TeleportChance { get; set; }
        public int ExBossTeleportChance { get; set; }
        public List<string> BossTags { get; set; }
        public List<string> ExBossTags { get; set; }
        public int KillRange { get; set; }
        public String Reward {  get; set; }
    }
}
