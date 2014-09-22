using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Netwatch.Model.Entities;

namespace Netwatch.ServiceLayer.Monitoring
{
    class PingMethod : IMonitoringMethod
    {
        public bool Is(string identifier)
        {
            return identifier != null && identifier.Equals("ping", StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<bool> Check(MonitoredService service, string identifier)
        {
            using (var ping = new Ping())
            {
                var pingResult = await ping.SendPingAsync(service.Address);

                return pingResult != null && pingResult.Status == IPStatus.Success;
            }
        }
    }
}
