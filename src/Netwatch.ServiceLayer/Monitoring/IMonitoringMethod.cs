using System.Threading.Tasks;
using Netwatch.Model.Entities;

namespace Netwatch.ServiceLayer.Monitoring
{
    public interface IMonitoringMethod
    {
        bool Is(string identifier);

        Task<bool> Check(MonitoredService service, string identifier);
    }
}
