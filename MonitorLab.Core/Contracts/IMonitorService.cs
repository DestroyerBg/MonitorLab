using Monitor = MonitorLab.Data.Models.Monitor;
namespace MonitorLab.Core.Contracts
{
    public interface IMonitorService
    {
        IEnumerable<Monitor> GetAllMonitors();
        
    }
}
