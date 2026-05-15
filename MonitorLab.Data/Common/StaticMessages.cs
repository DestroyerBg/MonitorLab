using MonitorLab.Data.EntityDTOs;
namespace MonitorLab.Data.Common
{
    public static class StaticMessages
    {
        public const string Gaming = "гейминг";
        public const string Office = "офис работа";
        public const string Multimedia = "мултимедия";
        public const string Design = "дизайн и обработка";
        public static string GetRecommendationTextFromStaticMessages(MonitorComparisonCardDTO monitor, string usage)
        {
            return $"{monitor.Brand} {monitor.Model} е най-подходящ за {usage} според сравнените параметри.";
        }
        
    }
}
