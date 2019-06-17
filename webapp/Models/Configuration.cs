namespace qu.kubeexplorer.webapp.Models
{
    public class Configuration
    {
        public ApplicationConfiguration Application { get; set; }
        public AzureConfiguration Azure { get; set; }
        public DevOpsConfiguration DevOps { get; set; }

        public class ApplicationConfiguration
        {
            public string Name { get; set; } = string.Empty;
            public bool ShowException { get; set; } = false;
            public bool CacheDataSets { get; set; } = false;
            public int CacheDurationMinutes { get; set; } = 0;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public bool AutoLogin { get; set; } = false;
        }

        public class AzureConfiguration
        {
            public string TenantId { get; set; } = string.Empty;
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
        }

        public class DevOpsConfiguration
        {
            public string Organisation { get; set; } = string.Empty;
            public string Projects { get; set; } = string.Empty;
            public string PatToken { get; set; } = string.Empty;
        }
    }
}