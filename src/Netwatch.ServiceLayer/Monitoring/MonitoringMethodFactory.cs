using System.Collections.Generic;
using System.Linq;

namespace Netwatch.ServiceLayer.Monitoring
{
    public class MonitoringMethodFactory
    {
        private static readonly IEnumerable<IMonitoringMethod> Methods;

        static MonitoringMethodFactory()
        {
            Methods = new List<IMonitoringMethod>
            {
                new PingMethod(),
                new RequestMethod()
            };
        }

        public static IMonitoringMethod GetMethod(string identifier)
        {
            return Methods.FirstOrDefault(_ => _.Is(identifier));
        }
    }
}
