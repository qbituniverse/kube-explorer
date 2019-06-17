using System.Collections.Generic;
using qu.nuget.azure.Dto;

namespace qu.nuget.azure.Models
{
    public class AcrsCatalog : Dictionary<string, Acrs>
    {
    }

    public class Acrs : AcrsDto
    {
    }

    public class Acr : AcrDto
    {
    }
}