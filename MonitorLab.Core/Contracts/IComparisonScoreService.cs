using MonitorLab.Data.EntityDTOs;

namespace MonitorLab.Core.Contracts
{
    public interface IComparisonScoreService
    {
        CompareDTO ApplyScores(CompareDTO model);
        CompareDTO ApplyRecommendations(CompareDTO model);
    }
}
