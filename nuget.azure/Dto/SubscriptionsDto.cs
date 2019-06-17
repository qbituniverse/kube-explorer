using System.Collections.Generic;

namespace qu.nuget.azure.Dto
{
    public class SubscriptionsDto
    {
        public List<SubscriptionDto> value { get; set; }
        public SubscriptionDto.Count count { get; set; }
    }
}