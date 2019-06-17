namespace qu.kubeexplorer.webapp.Helpers
{
    public static class Constants
    {
        public static class Scopes
        {
            public const string Pull = "pull";
            public const string Push = "*";
        }

        public static class Roles
        {
            public const string AcrReader = "AcrReader";
            public const string AcrContributor = "AcrContributor";

            public const string AksReader = "AksReader";

            public const string DevOpsReader = "DevOpsReader";
        }

        public static class AccessTokens
        {
            public const string AzureAccessToken = "AzureAccessToken";
            public const string AcrAccessToken = "AcrAccessToken";
            public const string DevOpsAccessToken = "DevOpsAccessToken";
        }

        public static class Policies
        {
            public const string Acr = "Acr";
            public const string Aks = "Aks";
            public const string DevOps = "DevOps";
        }

        public static class Uri
        {
            public const string AcrUri = "AcrUri";
        }
    }
}