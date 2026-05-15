using MonitorLab.Data.EntityDTOs;

namespace MonitorLab.Core.Contracts
{
    public interface IComparisonScoreService
    {
        void ApplyScores(CompareDTO model);
        void ApplyRecommendations(CompareDTO model);
    }
}
