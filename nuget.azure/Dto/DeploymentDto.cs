using System;
using System.Collections.Generic;

namespace qu.nuget.azure.Dto
{
    public class DeploymentDto
    {
        public string kind { get; set; }
        public string apiVersion { get; set; }
        public Metadata metadata { get; set; }
        public List<Item> items { get; set; }

        public class Metadata
        {
            public string selfLink { get; set; }
            public string resourceVersion { get; set; }
        }

        public class Metadata2Labels
        {
            public string app { get; set; }
            public string chart { get; set; }
            public string heritage { get; set; }
            public string release { get; set; }
            public string component { get; set; }
            public string type { get; set; }
            public string version { get; set; }
            public string name { get; set; }
            public string tier { get; set; }
        }

        public class Metadata2
        {
            public string name { get; set; }
            public string @namespace { get; set; }
            public string selfLink { get; set; }
            public string uid { get; set; }
            public string resourceVersion { get; set; }
            public int generation { get; set; }
            public DateTime creationTimestamp { get; set; }
            public Metadata2Labels labels { get; set; }
        }

        public class MatchLabels
        {
            public string app { get; set; }
            public string release { get; set; }
            public string component { get; set; }
            public string type { get; set; }
            public string name { get; set; }
        }

        public class Selector
        {
            public MatchLabels matchLabels { get; set; }
        }

        public class Metadata3Labels
        {
            public string app { get; set; }
            public string release { get; set; }
            public string component { get; set; }
            public string type { get; set; }
            public string version { get; set; }
            public string name { get; set; }
        }

        public class Metadata3
        {
            public object creationTimestamp { get; set; }
            public Metadata3Labels labels { get; set; }
            public string name { get; set; }
        }

        public class HostPath
        {
            public string path { get; set; }
            public string type { get; set; }
        }

        public class Item2
        {
            public string key { get; set; }
            public string path { get; set; }
        }

        public class ConfigMap
        {
            public string name { get; set; }
            public List<Item2> items { get; set; }
            public int defaultMode { get; set; }
            public bool? optional { get; set; }
        }

        public class Volume
        {
            public string name { get; set; }
            public HostPath hostPath { get; set; }
            public ConfigMap configMap { get; set; }
        }

        public class FieldRef
        {
            public string apiVersion { get; set; }
            public string fieldPath { get; set; }
        }

        public class ValueFrom
        {
            public FieldRef fieldRef { get; set; }
        }

        public class Env
        {
            public string name { get; set; }
            public string value { get; set; }
            public ValueFrom valueFrom { get; set; }
        }

        public class Limits
        {
            public string cpu { get; set; }
            public string memory { get; set; }
        }

        public class Requests
        {
            public string cpu { get; set; }
            public string memory { get; set; }
        }

        public class Resources
        {
            public Limits limits { get; set; }
            public Requests requests { get; set; }
        }

        public class VolumeMount
        {
            public string name { get; set; }
            public string mountPath { get; set; }
            public bool? readOnly { get; set; }
        }

        public class Port
        {
            public string name { get; set; }
            public int containerPort { get; set; }
            public string protocol { get; set; }
        }

        public class HttpGet
        {
            public string path { get; set; }
            public int port { get; set; }
            public string scheme { get; set; }
        }

        public class Exec
        {
            public List<string> command { get; set; }
        }

        public class LivenessProbe
        {
            public HttpGet httpGet { get; set; }
            public int initialDelaySeconds { get; set; }
            public int timeoutSeconds { get; set; }
            public int periodSeconds { get; set; }
            public int successThreshold { get; set; }
            public int failureThreshold { get; set; }
            public Exec exec { get; set; }
        }

        public class HttpGet2
        {
            public string path { get; set; }
            public int port { get; set; }
            public string scheme { get; set; }
        }

        public class ReadinessProbe
        {
            public HttpGet2 httpGet { get; set; }
            public int initialDelaySeconds { get; set; }
            public int timeoutSeconds { get; set; }
            public int periodSeconds { get; set; }
            public int successThreshold { get; set; }
            public int failureThreshold { get; set; }
        }

        public class Capabilities
        {
            public List<string> add { get; set; }
            public List<string> drop { get; set; }
        }

        public class SecurityContext
        {
            public Capabilities capabilities { get; set; }
            public int runAsUser { get; set; }
            public bool? privileged { get; set; }
        }

        public class Container
        {
            public string name { get; set; }
            public string image { get; set; }
            public List<Env> env { get; set; }
            public Resources resources { get; set; }
            public List<VolumeMount> volumeMounts { get; set; }
            public string terminationMessagePath { get; set; }
            public string terminationMessagePolicy { get; set; }
            public string imagePullPolicy { get; set; }
            public List<string> args { get; set; }
            public List<Port> ports { get; set; }
            public LivenessProbe livenessProbe { get; set; }
            public ReadinessProbe readinessProbe { get; set; }
            public SecurityContext securityContext { get; set; }
            public List<string> command { get; set; }
        }

        public class SecurityContext2
        {
        }

        public class MatchExpression
        {
            public string key { get; set; }
            public string @operator { get; set; }
        }

        public class NodeSelectorTerm
        {
            public List<MatchExpression> matchExpressions { get; set; }
        }

        public class RequiredDuringSchedulingIgnoredDuringExecution
        {
            public List<NodeSelectorTerm> nodeSelectorTerms { get; set; }
        }

        public class NodeAffinity
        {
            public RequiredDuringSchedulingIgnoredDuringExecution requiredDuringSchedulingIgnoredDuringExecution
            {
                get;
                set;
            }
        }

        public class MatchExpression2
        {
            public string key { get; set; }
            public string @operator { get; set; }
            public List<string> values { get; set; }
        }

        public class LabelSelector
        {
            public List<MatchExpression2> matchExpressions { get; set; }
        }

        public class PodAffinityTerm
        {
            public LabelSelector labelSelector { get; set; }
            public string topologyKey { get; set; }
        }

        public class PreferredDuringSchedulingIgnoredDuringExecution
        {
            public int weight { get; set; }
            public PodAffinityTerm podAffinityTerm { get; set; }
        }

        public class PodAntiAffinity
        {
            public List<PreferredDuringSchedulingIgnoredDuringExecution> preferredDuringSchedulingIgnoredDuringExecution
            {
                get;
                set;
            }
        }

        public class Affinity
        {
            public NodeAffinity nodeAffinity { get; set; }
            public PodAntiAffinity podAntiAffinity { get; set; }
        }

        public class Toleration
        {
            public string key { get; set; }
            public string @operator { get; set; }
        }

        public class Spec2
        {
            public List<Volume> volumes { get; set; }
            public List<Container> containers { get; set; }
            public string restartPolicy { get; set; }
            public int terminationGracePeriodSeconds { get; set; }
            public string dnsPolicy { get; set; }
            public SecurityContext2 securityContext { get; set; }
            public string schedulerName { get; set; }
            public string serviceAccountName { get; set; }
            public string serviceAccount { get; set; }
            public Affinity affinity { get; set; }
            public List<Toleration> tolerations { get; set; }
            public string priorityClassName { get; set; }
            public bool? automountServiceAccountToken { get; set; }
        }

        public class Template
        {
            public Metadata3 metadata { get; set; }
            public Spec2 spec { get; set; }
        }

        public class RollingUpdate
        {
            public object maxUnavailable { get; set; }
            public object maxSurge { get; set; }
        }

        public class Strategy
        {
            public string type { get; set; }
            public RollingUpdate rollingUpdate { get; set; }
        }

        public class Spec
        {
            public int replicas { get; set; }
            public Selector selector { get; set; }
            public Template template { get; set; }
            public Strategy strategy { get; set; }
            public int minReadySeconds { get; set; }
            public int revisionHistoryLimit { get; set; }
            public int progressDeadlineSeconds { get; set; }
        }

        public class Condition
        {
            public string type { get; set; }
            public string status { get; set; }
            public DateTime lastUpdateTime { get; set; }
            public DateTime lastTransitionTime { get; set; }
            public string reason { get; set; }
            public string message { get; set; }
        }

        public class Status
        {
            public int observedGeneration { get; set; }
            public int replicas { get; set; }
            public int updatedReplicas { get; set; }
            public int unavailableReplicas { get; set; }
            public List<Condition> conditions { get; set; }
        }

        public class Item
        {
            public Metadata2 metadata { get; set; }
            public Spec spec { get; set; }
            public Status status { get; set; }
        }
    }
}