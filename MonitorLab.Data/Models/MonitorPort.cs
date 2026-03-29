using System.ComponentModel.DataAnnotations;
using static MonitorLab.Data.Common.DatabaseConstants.MonitorPort;
namespace MonitorLab.Data.Models
{
    public class MonitorPort
    {
        [Required]
        public Guid MonitorId { get; set; }

        public Monitor Monitor { get; set; } = null!;

        [Required]
        public Guid PortId { get; set; }

        public Port Port { get; set; } = null!;

        [Range(CountMin, CountMax)]
        public int Count { get; set; }

    }
}
