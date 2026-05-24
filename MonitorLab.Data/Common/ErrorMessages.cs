namespace MonitorLab.Data.Common
{
    public static class ErrorMessages
    {
        public static class Monitor
        {
            public const string InvalidContrastRatio = "Некоректна стойност за контраст. Пример: 20:1";
            public const string MonitorNotFound = "Мониторът с това ID не съществува.";
            public const string BrandRequired =
            "Марката е задължителна.";

            public const string BrandLength =
                "Марката трябва да бъде между {2} и {1} символа.";

            public const string ModelRequired =
                "Моделът е задължителен.";

            public const string ModelLength =
                "Моделът трябва да бъде между {2} и {1} символа.";

            public const string ResolutionRequired =
                "Резолюцията е задължителна.";

            public const string PanelTypeRequired =
                "Типът панел е задължителен.";

            public const string ScreenSizeRange =
                "Размерът трябва да бъде между {1} и {2} инча.";

            public const string RefreshRateRange =
                "Честотата трябва да бъде между {1} и {2} Hz.";

            public const string ResponseTimeRange =
                "Времето за реакция трябва да бъде между {1} и {2} ms.";

            public const string BrightnessRange =
                "Яркостта трябва да бъде между {1} и {2} nits.";

            public const string ContrastRatioRequired =
                "Контрастът е задължителен.";

            public const string ContrastRatioLength =
                "Контрастът не може да надвишава {1} символа.";

            public const string DescriptionRequired =
                "Описанието е задължително.";

            public const string DescriptionLength =
                "Описанието трябва да бъде между {2} и {1} символа.";

            public const string ReleaseYearRange =
                "Годината трябва да бъде между {1} и {2}.";
        }

        public static class Common
        {
            public const string FieldIsRequired = "{0} е задължително поле.";
            public const string InvalidLoginAttempt = "Невалиден опит за вход.";
        }
        
    }
}
