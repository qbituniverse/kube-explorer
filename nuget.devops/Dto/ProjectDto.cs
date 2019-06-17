using System;
using System.Collections.Generic;

namespace qu.nuget.devops.Dto
{
    public class ProjectsDto
    {
        public int count { get; set; }
        public List<ProjectDto> value { get; set; }
    }

    public class ProjectDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string state { get; set; }
        public int revision { get; set; }
        public string visibility { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }
}