using System;
using System.Collections.Generic;

namespace qu.nuget.devops.Dto
{
    public class ReleaseDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public ModifiedBy modifiedBy { get; set; }
        public CreatedBy createdBy { get; set; }
        public Variables variables { get; set; }
        public List<object> variableGroups { get; set; }
        public ReleaseDefinition releaseDefinition { get; set; }
        public int releaseDefinitionRevision { get; set; }
        public string description { get; set; }
        public string reason { get; set; }
        public string releaseNameFormat { get; set; }
        public bool keepForever { get; set; }
        public int definitionSnapshotRevision { get; set; }
        public string logsContainerUrl { get; set; }
        public string url { get; set; }
        public Links4Release _links { get; set; }
        public List<object> tags { get; set; }
        public object triggeringArtifactAlias { get; set; }
        public ProjectReference projectReference { get; set; }
        public PropertiesRelease properties { get; set; }
    }

    public class ReleasesDto
    {
        public int count { get; set; }
        public List<ReleaseDto> value { get; set; }
    }

    public class AvatarRelease
    {
        public string href { get; set; }
    }

    public class LinksRelease
    {
        public Avatar avatar { get; set; }
    }

    public class ModifiedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Avatar2Release
    {
        public string href { get; set; }
    }

    public class Links2Release
    {
        public Avatar2 avatar { get; set; }
    }

    public class CreatedBy
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public Links2 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class Variables
    {
    }

    public class SelfRelease
    {
        public string href { get; set; }
    }

    public class WebRelease
    {
        public string href { get; set; }
    }

    public class Links3Release
    {
        public Self self { get; set; }
        public Web web { get; set; }
    }

    public class ReleaseDefinition
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public Links3 _links { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Web2
    {
        public string href { get; set; }
    }

    public class Links4Release
    {
        public Self2 self { get; set; }
        public Web2 web { get; set; }
    }

    public class ProjectReference
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class PropertiesRelease
    {
    }
}