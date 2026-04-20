namespace MonitorLab.Data.Seed.DTOS
{
    public class PortSeedDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public double Version { get; set; }
    }
}
