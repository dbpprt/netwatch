using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Netwatch.Model.Entities;

namespace Netwatch.ServiceLayer.Monitoring
{
    class RequestMethod : IMonitoringMethod
    {
        public bool Is(string identifier)
        {
            return identifier != null && (
                identifier.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase) ||
                identifier.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> Check(MonitoredService service, string identifier)
        {
            var url = "";

            if (identifier.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase))
                url += "http://";
            else if (identifier.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase))
                url += "https://";
            else throw new ArgumentException("The identifier doenst provide a valid url schema " + identifier, "identifier");

            url += service.Address;
            url += identifier.Split(':').Last();
            var uri = new Uri(url);

            using (var client = new WebClient())
            {
                try
                {
                    await client.DownloadStringTaskAsync(uri);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
