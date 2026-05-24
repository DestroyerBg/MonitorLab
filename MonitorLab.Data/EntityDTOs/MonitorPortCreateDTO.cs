namespace MonitorLab.Data.EntityDTOs
{
    public class MonitorPortCreateDTO
    {
        public Guid PortId { get; set; }
        public string Name { get; set; } = null!;
        public double Version { get; set; }
        public bool IsSelected { get; set; }
        public int Count { get; set; } = 1;
    }
}
