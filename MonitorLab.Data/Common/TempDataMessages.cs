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

        public const string MonitorAddedSuccessfullyIntoDatabase = "Мониторът беше добавен успешно.";

        public const string MonitorDeletedSuccessfully = "Мониторът беше изтрит успешно.";

        public static string MonitorAlreadyAdded(string brand, string model)
            => $"Мониторът {brand} {model} вече е добавен в списъка за сравнение.";

        public static string MonitorAddedSuccessfully(string brand, string model)
            => $"Мониторът {brand} {model} е добавен успешно за сравнение.";

        public static string MonitorRemovedSuccessfully(string brand, string model)
            => $"Мониторът {brand} {model} беше премахнат от сравнението.";

        public static string MonitorEditedSuccessfully(string brand, string model)
            => $"Мониторът {brand} {model} беше редактиран успешно.";
    }
}
    
