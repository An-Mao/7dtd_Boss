using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Boss.patch;
using HarmonyLib;

namespace Boss
{
    /*
    public class Boss : IModApi {

        private static Harmony harmony = new Harmony("nws.dev.7d2d.boss");
        private static String configFile = GetDllDirectory() + "\\config.xml";
        public void InitMod(Mod _modInstance) {
            EntityAlivePatch.LoadConfig(configFile);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Assembly.Load("Boss");
            XmlWatcher watcher = new XmlWatcher(configFile);
            await watcher.StartWatching();
            Log.Out("Boss Mod dll load");
        }
        public static string GetDllDirectory() {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetDirectoryName(assemblyLocation);
        }


    }
    */
    public class Boss : IModApi {
        private static Harmony harmony = new Harmony("nws.dev.7d2d.boss");
        private static string configFile = GetDllDirectory() + "\\config.xml";
        private XmlWatcher watcher; // 保存 XmlWatcher 实例

        public void InitMod(Mod _modInstance) {
            LoadConfigs.LoadBossConfig(configFile);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // 确保 Boss 程序集已经被加载
            try {
                Assembly.Load("Boss"); // 确保 Boss 程序集已经被加载
            } catch (Exception e) {
                Log.Error($"Failed to load Boss assembly: {e}");
            }

            // 创建 XmlWatcher 实例
            //watcher = new XmlWatcher(configFile);

            // 启动 XML 监视 (使用 Task.Run 避免阻塞主线程)
            /*
            Task.Run(async () =>
            {
                try {
                    await watcher.StartWatching();
                } catch (Exception e) {
                    Log.Error($"Error starting XML watcher: {e}");
                }
            });
            */

            Log.Out("Boss Mod dll load");
        }

        // 在 Mod 卸载时停止监视
        public void ShutdownMod() {
            Log.Out("Shutting down Boss Mod.");
            watcher?.StopWatching(); // 确保 watcher 不为 null
            //harmony?.UnpatchAll("nws.dev.7d2d.boss"); //卸载 Harmony 补丁
        }

        public static string GetDllDirectory() {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return System.IO.Path.GetDirectoryName(assemblyLocation);
        }
    }
}
