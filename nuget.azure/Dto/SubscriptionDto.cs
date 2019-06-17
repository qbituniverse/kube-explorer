namespace qu.nuget.azure.Dto
{
    public class SubscriptionDto
    {
        public string id { get; set; }
        public string subscriptionId { get; set; }
        public string tenantId { get; set; }
        public string displayName { get; set; }
        public string state { get; set; }
        public Policies subscriptionPolicies { get; set; }
        public string authorizationSource { get; set; }

        public class Policies
        {
            public string locationPlacementId { get; set; }
            public string quotaId { get; set; }
            public string spendingLimit { get; set; }
        }

        public class Count
        {
            public string type { get; set; }
            public int value { get; set; }
        }
    }
}