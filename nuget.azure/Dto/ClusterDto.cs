using System.Collections.Generic;

namespace qu.nuget.azure.Dto
{
    public class ClusterDto
    {
        public string id { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public ClusterProperties properties { get; set; }

        public class AdminUser
        {
            public string id { get; set; }
            public string location { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public UserProperties properties { get; set; }
        }

        public class AdminUsers
        {
            public List<Kubeconfig> kubeconfigs { get; set; }
        }

        public class AgentPoolProfile
        {
            public string name { get; set; }
            public int count { get; set; }
            public string vmSize { get; set; }
            public int osDiskSizeGB { get; set; }
            public string storageProfile { get; set; }
            public int maxPods { get; set; }
            public string osType { get; set; }
        }

        public class ServicePrincipalProfile
        {
            public string clientId { get; set; }
        }

        public class HttpApplicationRouting
        {
            public bool enabled { get; set; }
            public object config { get; set; }
        }

        public class AddonProfiles
        {
            public HttpApplicationRouting httpApplicationRouting { get; set; }
        }

        public class NetworkProfile
        {
            public string networkPlugin { get; set; }
            public string podCidr { get; set; }
            public string serviceCidr { get; set; }
            public string dnsServiceIP { get; set; }
            public string dockerBridgeCidr { get; set; }
        }

        public class ClusterProperties
        {
            public string provisioningState { get; set; }
            public string kubernetesVersion { get; set; }
            public string dnsPrefix { get; set; }
            public string fqdn { get; set; }
            public List<AgentPoolProfile> agentPoolProfiles { get; set; }
            public ServicePrincipalProfile servicePrincipalProfile { get; set; }
            public AddonProfiles addonProfiles { get; set; }
            public string nodeResourceGroup { get; set; }
            public bool enableRBAC { get; set; }
            public NetworkProfile networkProfile { get; set; }
        }

        public class Kubeconfig
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class UserProperties
        {
            public string kubeConfig { get; set; }
        }
    }
}