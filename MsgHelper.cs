using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Boss {
    public class MsgHelper {
        public static void sendMsg(String msg,List<int> players) {
            GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, msg, players, EMessageSender.Server);
        }
    }
}
