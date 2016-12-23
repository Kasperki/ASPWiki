using Microsoft.AspNetCore.Authentication.Twitter;

namespace ASPWiki
{
    public static class Constants
    {
        public static string AuthenticationSchemeKey { get { return TwitterDefaults.AuthenticationScheme; } }

        public static string AuthenticationSchemeCookies { get { return "Cookies"; } }

        public const string LoginRedirectRoute = "/";

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
