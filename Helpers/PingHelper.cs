using System;
using System.Net.NetworkInformation;

namespace TransporterService.Helpers
{
    public static class PingHelper
    {
        public static void PingHost(string hostIp, int timeout = 1000)
        {
            var ping = new Ping();
            
            var pingReply = ping.Send(hostIp, timeout);
            if (pingReply?.Status != IPStatus.Success)
            {
                throw new Exception($"{hostIp} is unreachable");
            }
        }
    }
}