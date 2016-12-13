namespace ASPWiki
{
    public static class Constants
    {
        public static string AuthenticationLoginUrl { get { return "https://127.0.0.1:8081/login"; } }

        public static string AuthenticationValidationUrl { get { return "https://127.0.0.1:8081/test"; } }

        public static string AuthenticationScheme { get { return "Cookies"; } }

        public static string ENV_VARIABLE_PREFIX { get { return "ASPWiki"; } }

        #region Logging
        public static string LoggingFilePath { get { return "Logs"; } }
        public static string LoggingFileName { get { return "log.txt"; } }
        #endregion

        #region MonboDB
        public static int DatabasePort { get { return 27017; } }
        public static string DatabaseName { get { return "wiki"; } }
        public static string TestDatabaseName { get { return "wikipagesTest"; } }
        public static string WikiPagesCollectionName { get { return "wikipages"; } }
        #endregion

        #region EventCodes
        public static int WARNING_CODE_UNAUTHORIZED { get { return 100; } }

        public static int ERROR_CODE_EXPECTION { get { return 101; } }
        #endregion
    }
}
