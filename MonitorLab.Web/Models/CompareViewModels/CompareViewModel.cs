namespace MonitorLab.Web.Models.CompareViewModels
{
    public class CompareViewModel
    {
        public IList<MonitorComparisonCardViewModel> Monitors { get; set; }
       = new List<MonitorComparisonCardViewModel>();

        public ComparisonRecommendationViewModel Recommendations { get; set; }
            = new();
    }
}
