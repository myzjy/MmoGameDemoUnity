using System;
using System.Collections.Generic;

namespace FrostEngine
{
    [Serializable]
    public class ServerChannelInfo
    {
        public string channelName;
        public string curUseServerName;
        public List<ServerIpAndPort> serverIpAndPorts;
    }
}