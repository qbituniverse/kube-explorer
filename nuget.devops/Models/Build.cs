using System.Collections.Generic;
using qu.nuget.devops.Dto;

namespace qu.nuget.devops.Models
{
    public class Build : BuildDto
    {
    }

    public class Builds
    {
        public int count { get; set; }
        public List<Build> value { get; set; }
    }
}