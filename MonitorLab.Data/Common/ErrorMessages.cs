namespace MonitorLab.Data.Common
{
    public static class ErrorMessages
    {
        public static class Monitor
        {
            public const string InvalidContrastRatio = "Некоректна стойност за контраст. Пример: 20:1";
            public const string MonitorNotFound = "Мониторът с това ID не съществува.";
        }

        public static class Common
        {
            public const string FieldIsRequired = "{0} е задължително поле.";
            public const string InvalidLoginAttempt = "Невалиден опит за вход.";
        }
        
    }
}
