using System.Collections.Generic;

namespace qu.nuget.azure.Dto
{
    public class ImageDto
    {
        public class Tag
        {
            public string name { get; set; }
            public List<string> tags { get; set; }
        }

        public class FsLayer
        {
            public string blobSum { get; set; }
        }

        public class History
        {
            public string v1Compatibility { get; set; }
        }

        public class Jwk
        {
            public string crv { get; set; }
            public string kid { get; set; }
            public string kty { get; set; }
            public string x { get; set; }
            public string y { get; set; }
        }

        public class Header
        {
            public Jwk jwk { get; set; }
            public string alg { get; set; }
        }

        public class Signature
        {
            public Header header { get; set; }
            public string signature { get; set; }
            public string @protected { get; set; }
        }

        public class ManifestData
        {
            public int schemaVersion { get; set; }
            public string name { get; set; }
            public string tag { get; set; }
            public string architecture { get; set; }
            public List<FsLayer> fsLayers { get; set; }
            public List<History> history { get; set; }
            public List<Signature> signatures { get; set; }
        }

        public class Labels
        {
            public string description { get; set; }
            public string maintainer { get; set; }
        }

        public class Config
        {
            public string Hostname { get; set; }
            public string Domainname { get; set; }
            public string User { get; set; }
            public bool AttachStdin { get; set; }
            public bool AttachStdout { get; set; }
            public bool AttachStderr { get; set; }
            public bool Tty { get; set; }
            public bool OpenStdin { get; set; }
            public bool StdinOnce { get; set; }
            public List<string> Env { get; set; }
            public List<string> Cmd { get; set; }
            public bool ArgsEscaped { get; set; }
            public string Image { get; set; }
            public object Volumes { get; set; }
            public string WorkingDir { get; set; }
            public object Entrypoint { get; set; }
            public object OnBuild { get; set; }
            public Labels Labels { get; set; }
        }

        public class Labels2
        {
            public string description { get; set; }
            public string maintainer { get; set; }
        }

        public class ContainerConfig
        {
            public string Hostname { get; set; }
            public string Domainname { get; set; }
            public string User { get; set; }
            public bool AttachStdin { get; set; }
            public bool AttachStdout { get; set; }
            public bool AttachStderr { get; set; }
            public bool Tty { get; set; }
            public bool OpenStdin { get; set; }
            public bool StdinOnce { get; set; }
            public List<string> Env { get; set; }
            public List<string> Cmd { get; set; }
            public bool ArgsEscaped { get; set; }
            public string Image { get; set; }
            public object Volumes { get; set; }
            public string WorkingDir { get; set; }
            public object Entrypoint { get; set; }
            public object OnBuild { get; set; }
            public Labels2 Labels { get; set; }
        }

        public class V1Compatibility
        {
            public string architecture { get; set; }
            public Config config { get; set; }
            public ContainerConfig container_config { get; set; }
            public string created { get; set; }
            public string docker_version { get; set; }
            public string id { get; set; }
            public string os { get; set; }
            public string parent { get; set; }
        }
    }
}