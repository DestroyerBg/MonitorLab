namespace MonitorLab.Data.EntityDTOs
{
    public class CompareDTO
    {
        public IList<MonitorComparisonCardDTO> Monitors { get; set; }
       = new List<MonitorComparisonCardDTO>();

        public ComparisonRecommendationDTO Recommendations { get; set; }
            = new();
    }
}
