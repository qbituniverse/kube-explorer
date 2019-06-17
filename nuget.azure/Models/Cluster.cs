using System.Collections.Generic;
using qu.nuget.azure.Dto;

namespace qu.nuget.azure.Models
{
    public class ClustersCatalog : Dictionary<string, Clusters>
    {
    }

    public class Cluster : ClusterDto
    {
        public string accessToken { get; set; } = string.Empty;
        public Deployment deployment { get; set; } = new Deployment();
    }

    public class Clusters
    {
        public List<Cluster> value { get; set; } = new List<Cluster>();
    }
}