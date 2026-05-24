using MonitorLab.Data.Common;
using System.ComponentModel.DataAnnotations;

public class MonitorPortCreateViewModel
{
    public Guid PortId { get; set; }

    public string Name { get; set; } = null!;

    public double Version { get; set; }

    public bool IsSelected { get; set; }

    [Range(
        DatabaseConstants.MonitorPort.CountMin,
        DatabaseConstants.MonitorPort.CountMax)]
    public int Count { get; set; } = 1;
}