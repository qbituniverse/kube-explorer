using System;
using System.Collections.Generic;

namespace qu.nuget.azure.Dto
{
    public class ChartDto
    {
        public string name { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public List<Maintainer> maintainers { get; set; }
        public string apiVersion { get; set; }
        public List<string> urls { get; set; }
        public DateTime created { get; set; }
        public string digest { get; set; }
        public Metadata acrMetadata { get; set; }
        public class Maintainer
        {
            public string name { get; set; }
            public string email { get; set; }
        }

        public class Metadata
        {
            public string manifestDigest { get; set; }
        }
    }
}