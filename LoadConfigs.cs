using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityDistantTerrain;
using System.Xml;
using System.IO;

namespace Boss {
    public class LoadConfigs {
        public static void LoadBossConfig(String configPath) {
            try {
                if (File.Exists(configPath)) {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(configPath);

                    XmlNode root = xmlDoc.DocumentElement; // 获取根节点

                    BossEnchant.config = new BossDamageConfig {
                        BossDamageCooldown = int.Parse(root.SelectSingleNode("BossDamageCooldown").InnerText),
                        BossDamageLimit = int.Parse(root.SelectSingleNode("BossDamageLimit").InnerText),
                        BossDamageCooldownScale = double.Parse(root.SelectSingleNode("BossDamageCooldownScale").InnerText),

                        MaxDamage = int.Parse(root.SelectSingleNode("MaxDamage").InnerText),
                        DamageLimit = int.Parse(root.SelectSingleNode("DamageLimit").InnerText),
                        CooldownSeconds = int.Parse(root.SelectSingleNode("CooldownSeconds").InnerText),
                        ExBossDamageMultiplier = float.Parse(root.SelectSingleNode("ExBossDamageMultiplier").InnerText),
                        TeleportChance = int.Parse(root.SelectSingleNode("TeleportChance").InnerText),
                        ExBossTeleportChance = int.Parse(root.SelectSingleNode("ExBossTeleportChance").InnerText),
                        BossTags = ReadTagList(root, "BossTags"),
                        ExBossTags = ReadTagList(root, "ExBossTags"),
                        KillRange = int.Parse(root.SelectSingleNode("KillRange").InnerText),
                        Reward = root.SelectSingleNode("Reward").InnerText
                    };
                    BossEnchant.bossGroup = FastTags<TagGroup.Global>.Parse(string.Join(",", BossEnchant.config.BossTags));
                    BossEnchant.exBossGroup = FastTags<TagGroup.Global>.Parse(string.Join(",", BossEnchant.config.ExBossTags));
                } else {
                    // 如果配置文件不存在，则创建默认配置并保存 (XML 格式)
                    BossEnchant.config = new BossDamageConfig {
                        BossDamageCooldown = 30,
                        BossDamageLimit = 100000,
                        BossDamageCooldownScale = 0.0001,

                        MaxDamage = 5,
                        DamageLimit = 50000,
                        CooldownSeconds = 20,
                        ExBossDamageMultiplier = 0.5f,
                        TeleportChance = 10,
                        ExBossTeleportChance = 20,
                        BossTags = new List<string> { "boss", "exboss" },
                        ExBossTags = new List<string> { "exboss" },

                        KillRange = 50,
                        Reward = "questRewardAmmoCraftingBundle"
                    };

                    XmlDocument xmlDoc = new XmlDocument();
                    XmlElement root = xmlDoc.CreateElement("BossDamageConfig");
                    xmlDoc.AppendChild(root);

                    AddXmlElement(xmlDoc, root, "BossDamageCooldown", BossEnchant.config.BossDamageCooldown.ToString());
                    AddXmlElement(xmlDoc, root, "BossDamageLimit", BossEnchant.config.BossDamageLimit.ToString());
                    AddXmlElement(xmlDoc, root, "BossDamageCooldownScale", BossEnchant.config.BossDamageCooldownScale.ToString());

                    AddXmlElement(xmlDoc, root, "MaxDamage", BossEnchant.config.MaxDamage.ToString());
                    AddXmlElement(xmlDoc, root, "DamageLimit", BossEnchant.config.DamageLimit.ToString());
                    AddXmlElement(xmlDoc, root, "CooldownSeconds", BossEnchant.config.CooldownSeconds.ToString());
                    AddXmlElement(xmlDoc, root, "ExBossDamageMultiplier", BossEnchant.config.ExBossDamageMultiplier.ToString());
                    AddXmlElement(xmlDoc, root, "TeleportChance", BossEnchant.config.TeleportChance.ToString());
                    AddXmlElement(xmlDoc, root, "ExBossTeleportChance", BossEnchant.config.ExBossTeleportChance.ToString());

                    AddXmlTagList(xmlDoc, root, "BossTags", BossEnchant.config.BossTags);
                    AddXmlTagList(xmlDoc, root, "ExBossTags", BossEnchant.config.ExBossTags);

                    AddXmlElement(xmlDoc, root, "KillRange", BossEnchant.config.KillRange.ToString());
                    AddXmlElement(xmlDoc, root, "Reward", BossEnchant.config.Reward);

                    xmlDoc.Save(configPath);
                    BossEnchant.bossGroup = FastTags<TagGroup.Global>.Parse(string.Join(",", BossEnchant.config.BossTags));
                    BossEnchant.exBossGroup = FastTags<TagGroup.Global>.Parse(string.Join(",", BossEnchant.config.ExBossTags));
                }
            } catch (Exception ex) {
                Log.Out($"[BossDamageMod] Error loading config: {ex}");
            }
        }
        // 辅助方法：读取标签列表
        private static List<string> ReadTagList(XmlNode root, string listName) {
            List<string> tags = new List<string>();
            XmlNodeList tagNodes = root.SelectSingleNode(listName)?.SelectNodes("Tag"); // 使用 ?. 避免空引用异常
            if (tagNodes != null) {
                foreach (XmlNode tagNode in tagNodes) {
                    tags.Add(tagNode.InnerText);
                }
            }
            return tags;
        }
        //辅助方法: 添加简单XML元素
        private static void AddXmlElement(XmlDocument doc, XmlElement parent, string name, string value) {
            XmlElement element = doc.CreateElement(name);
            element.InnerText = value;
            parent.AppendChild(element);
        }
        // 辅助方法：添加标签列表到 XML
        private static void AddXmlTagList(XmlDocument doc, XmlElement root, string listName, List<string> tags) {
            XmlElement listElement = doc.CreateElement(listName);
            foreach (string tag in tags) {
                XmlElement tagElement = doc.CreateElement("Tag");
                tagElement.InnerText = tag;
                listElement.AppendChild(tagElement);
            }
            root.AppendChild(listElement);
        }
    }
}
