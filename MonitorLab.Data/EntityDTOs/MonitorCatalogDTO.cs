namespace MonitorLab.Data.EntityDTOs
{
    public class MonitorCatalogDTO
    {
        public IEnumerable<MonitorCardDto> Monitors { get; set; } = new List<MonitorCardDto>();

        public string? SearchTerm { get; set; }
        public string? Brand { get; set; }
        public string? Resolution { get; set; }
        public string? PanelType { get; set; }
        public int? MinRefreshRate { get; set; }

        public IEnumerable<string> Brands { get; set; } = new List<string>();
        public IEnumerable<string> Resolutions { get; set; } = new List<string>();
        public IEnumerable<string> PanelTypes { get; set; } = new List<string>();
    }
}
