using System;

namespace qu.nuget.azure.Dto
{
    public class AcrDto
    {
        public Sku sku { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public Tags tags { get; set; }
        public Properties properties { get; set; }
    }

    public class Sku
    {
        public string name { get; set; }
        public string tier { get; set; }
    }

    public class Tags
    {
    }

    public class Properties
    {
        public string loginServer { get; set; }
        public DateTime creationDate { get; set; }
        public string provisioningState { get; set; }
        public bool adminUserEnabled { get; set; }
    }
}