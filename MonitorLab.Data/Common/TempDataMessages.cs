namespace MonitorLab.Data.Common
{
    public static class TempDataMessages
    {
        public const string ToastType = "ToastType";
        public const string ToastMessage = "ToastMessage";

        public const string Success = "success";
        public const string Error = "error";

        public const string MonitorNotFound = "Мониторът не беше намерен.";
        public const string CompareLimitReached = "Могат да се сравняват максимум 3 монитора.";

        public static string MonitorAlreadyAdded(string brand, string model)
            => $"{brand} {model} вече е добавен в списъка за сравнение.";

        public static string MonitorAddedSuccessfully(string brand, string model)
            => $"{brand} {model} е добавен успешно за сравнение.";
    }
}
