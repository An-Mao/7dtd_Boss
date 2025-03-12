using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Boss.patch {
    //[HarmonyPatch(typeof(ConnectionManager))]
    public class ConnectionManagerPatch {
        //[HarmonyPatch("ProcessPackages")]
        //[HarmonyPrefix]
        public static bool ProcessPackagesPrefix(ConnectionManager __instance, INetConnection _connection, NetPackageDirection _disallowedDirection, ClientInfo _clientInfo) {
            // 只在服务器端拦截
            if (!GameManager.Instance.World.IsRemote()) {
                // 获取当前连接的 ClientInfo
                ClientInfo clientInfo = _clientInfo; //可能为null

                // 获取数据包列表
                _connection.GetPackages(__instance.packagesToProcess); // 使用 GetPackages() 获取队列

            }

            return true; // 允许原始方法执行
        }
    }
}
