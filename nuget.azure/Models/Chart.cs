using System.Collections.Generic;
using qu.nuget.azure.Dto;

namespace qu.nuget.azure.Models
{
    public class ChartsCatalog : Dictionary<string, List<Chart>>
    {
    }

    public class Chart : ChartDto
    {
    }
}