# kube-explorer

**kube-explorer** is available on: [GitHub](https://github.com/qbituniverse/kube-explorer) - [DockerHub](https://cloud.docker.com/repository/docker/qbituniverse/kube-explorer) - [Web](http://qbituniverse.com)

## Description

**kube-explorer** is designed to help you browse and manage your Azure Container Registry (ACR) with ease and in one place. 

**kube-explorer** gives you visibility into your Azure Kubernetes Service (AKS) cluster so that you know where your Images and Helm Charts are deployed.

**kube-explorer** is free, container based tool, stateless, easy to configure and deploy. The installation of **kube-explorer** doesn't pollute your system with lots of binaries or redundant files. In fact, the footprint of **kube-explorer** on your system is **none, zero**, as it runs on your Docker Engine instance.

You can run **kube-explorer** on any platform: Linux, Windows or MacOS, as long as the operating system is Docker aware.

This is image for running **kube-explorer** application. Please refer to the *Configuration* and *Deployment* sections regarding instructions on how to set **kube-explorer** up.

## Key

Key terms used in this document:
- **ACR** - Azure Container Registry
- **AKS** - Azure Kubernetes Service
- **Principal** - App Registration in Azure to allow principal access to resources in Azure as either Reader or Contributor for the purposes to run **kube-explorer**

## Limitations

**kube-explorer's** session is fixed to 60 minutes, after which you will be logged out and you will have to login again. Future upgrades will make this configurable beyond 60 minute timeouts.

**kube-explorer** can only be linked to Azure cloud at the moment. Future upgrades will allow connections to Amazon Web Services (AWS) and Google Cloud.

**kube-explorer** can read from a single ACR at the moment. 

However, **kube-explorer** can access Kubernetes Cluster details from multiple AKSs even on multiple Azure Subscriptions.

## Configuration

### Images & Helm Charts Linking with AKS

**kube-explorer** will cross-reference usage of your Images and Helm Charts in ACR across your AKS Cluster Deployments. It's important, however, to understand how the linking works and how to set it up.

Without the config below **kube-explorer** will not be able to cross reference your Images & Helm Charts in ACR with your AKS Cluster Deployments.

#### Images linking with AKS

The link is based on Image Name as follows *myrepo.registry.io/my-image:tag*

```
myrepo.registry.io/web/api:2.2.12877
```

*Deployment.yaml* template will need the Image Name, *myrepo.registry.io/web/api:2.2.12877*, passed in the **spec.template.spec.containers.image** section of the template. 

```
spec.template.spec.containers.image: myrepo.registry.io/web/api:2.2.12877
```

**kube-explorer** will then access this information from AKS Cluster Deployment and cross reference its data sets from ACR to establish a visual indicator for your Images in the tool.

#### Helm Charts linking with AKS

The link is based on Helm Chart Name as follows *my-chart-version*

```
web-api-2.2.12877
```

*Deployment.yaml* template will need the Chart Name, *web-api-2.2.12877*, passed in the **spec.template.metadata.labels.chart** section of the template. 

```
spec.template.metadata.labels.chart: web-api-2.2.12877
```

**kube-explorer** will then access this information from AKS Cluster Deployment and cross reference its data sets from ACR to establish a visual indicator for your Helm Charts in the tool.

### Authorisation

Without the config below **kube-explorer** will not be able to pull data from either ACR or AKS.

| Config | Type | Description |
| ----------- | ----------- | ----------- |
| TenantId | GUID | ID of your Azure Subscription where ACR and AKS are hosted |
| ClientId | GUID | Application ID of your Principal in Azure, for example you can call it *kube-explorer-principal* |
| ClientSecret | string | This is the secret key created when you create the Principal, *kube-explorer-principal* |

Once you have the above config items, **kube-explorer** can be setup in either Read or Contributor Role for either ACR or AKS access.

| Role | Azure Setup | Description |
| ----------- | ----------- | ----------- |
| AcrReader | Assign your Principal, *kube-explorer-principal*, as Reader on ACR | This will allow **kube-explorer** to pull Images & Helm Charts on ACR, so you will only be able to browse these |
| AcrContributor | Assign your Principal, *kube-explorer-principal*, as Contributor on ACR | This will allow **kube-explorer** to both pull and push Images & Helm Charts on ACR, so you will be able, in addition to browsing, to *Delete* Image & Helm Chart Repositories |
| AksContributor | Assign your Principal, *kube-explorer-principal*, as Contributor on AKS | This will allow **kube-explorer** to pull AKS details and Deployments, so you will be able to browse these |

### Environment Variables

| Variable | Type | Description |
| ----------- | ----------- | ----------- |
| ASPNETCORE_ENVIRONMENT | string | Development, Staging, Production |

### Application Settings

| Setting | Type | Description |
| ----------- | ----------- | ----------- |
| Configuration__Application__Name | string, default = "kube-explorer" | Name for the application |
| Configuration__Application__ShowException | bool, default = false | Allows to define if exception message is to be shown on screen |
| Configuration__Application__CacheDataSets | bool, default = false | Define if you want to cache ACR & AKS data |
| Configuration__Application__CacheDurationMinutes | int, >0, default = 60 | Cache duration for the data sets, in minutes |
| Configuration__Application__Username | string, default = "" | Username for access |
| Configuration__Application__Password | string, default = "" | Password for access |
| Configuration__Application__AutoLogin | bool, default = false | Allows to skip Username+Password configuration, simply click login without providing credentials  |
| Configuration__Azure__TenantId | string, default = "" | Azure tenant ID |
| Configuration__Azure__ClientId | string, default = "" | Client ID (Principal) to access Azure resources (ACR and/or AKS). |
| Configuration__Azure__ClientSecret | string, default = "" | Client Key (Principal) to access Azure resources (ACR and/or AKS). |

## Deployment

### Docker "run" with default configuration

1. Replace the *latest* tag with desired version for the image
2. Run the *docker run* command as below

```
docker run `
-dit `
-p 80:80 `
-m 1g `
--cpus="1" `
--restart=always `
--read-only `
--tmpfs /tmp `
--cap-drop=ALL `
--cap-add=NET_BIND_SERVICE `
--name kube-explorer `
-e ASPNETCORE_ENVIRONMENT=Production `
qbituniverse/kube-explorer:latest
```

### Docker "run" with custom configuration

1. Replace the *latest* tag with desired version of the image
2. Pass required parameters for the *-e* key=value pairs
3. Run the *docker run* command as below

```
docker run `
-dit `
-p 80:80 `
-m 1g `
--cpus="1" `
--restart=always `
--read-only `
--tmpfs /tmp `
--cap-drop=ALL `
--cap-add=NET_BIND_SERVICE `
--name kube-explorer `
-e ASPNETCORE_ENVIRONMENT=Production `
-e Configuration__Application__Name=name `
-e Configuration__Application__ShowException=false `
-e Configuration__Application__CacheDataSets=false `
-e Configuration__Application__CacheDurationMinutes=60 `
-e Configuration__Application__Username=username `
-e Configuration__Application__Password=password `
-e Configuration__Application__AutoLogin=false `
-e Configuration__Azure__TenantId=tenantid `
-e Configuration__Azure__ClientId=clienid `
-e Configuration__Azure__ClientSecret=clientsecret `
qbituniverse/kube-explorer:latest
```

### Docker compose with default configuration

1. Replace the *latest* tag with desired version of the image inside of the Compose-kube-explorer.yaml file 
2. Run the *docker-compose* command to create application with default configuration

#### Compose-kube-explorer.yaml

```
version: '2.4'
services:
  api:
    image: qbituniverse/kube-explorer:latest
    container_name: kube-explorer
    ports:
    - "80:80"
    read_only: true
    tmpfs:
    - /tmp
    restart: always
    cpus: 1
    mem_limit: 1g
    cap_drop:
    - ALL
    cap_add:
    - NET_BIND_SERVICE
    environment:
    - ASPNETCORE_ENVIRONMENT=Production
```

```
docker-compose -f Compose-kube-explorer.yaml up -d
```

### Docker compose with custom configuration

1. Replace the *latest* tag with desired version of the image inside of the Compose-kube-explorer.yaml file 
2. Modify the Compose-values.yaml with custom configuration
3. Run the *docker-compose* command to create application with custom configuration

#### Compose-values.yaml

```
version: '2.4'
services:
  api:
    ports:
    - "80:80"
    environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - Configuration__Application__Name=name
    - Configuration__Application__ShowException=false
    - Configuration__Application__CacheDataSets=false
    - Configuration__Application__CacheDurationMinutes=60
    - Configuration__Application__Username=username
    - Configuration__Application__Password=password
    - Configuration__Application__AutoLogin=false
    - Configuration__Azure__TenantId=tenantid
    - Configuration__Azure__ClientId=clienid
    - Configuration__Azure__ClientSecret=clientsecret
```

```
docker-compose -f Compose-kube-explorer.yaml -f Compose-values.yaml up -d
```

### Helm Chart

#### Default Installation
```
helm install --name kube-explorer local/kube-explorer
```

#### Using Configuration
```
helm install --name kube-explorer `
--set ASPNETCORE_ENVIRONMENT=Production `
--set Configuration__Application__Name=name `
--set Configuration__Application__ShowException=false `
--set Configuration__Application__CacheDataSets=false `
--set Configuration__Application__CacheDurationMinutes=60 `
--set Configuration__Application__Username=username `
--set Configuration__Application__Password=password `
--set Configuration__Application__AutoLogin=false `
--set Configuration__Azure__TenantId=tenantid `
--set Configuration__Azure__ClientId=clienid `
--set Configuration__Azure__ClientSecret=clientsecret `
local/kube-explorer
```

#### Using Values File
```
helm install --name kube-explorer -f values.yaml local/kube-explorer
```

#### values.yaml
```
name: ""
namespace: ""
secrets:
  dockerHub: ""
configuration:
  environment: ""
  applicationName: ""
  applicationShowException: ""
  applicationCacheDataSets: ""
  applicationCacheDurationMinutes: ""
  applicationUsername: ""
  applicationPassword: ""
  applicationAutoLogin: ""
  azureTenantId: ""
  azureClientId: ""
  azureClientSecret: ""
```

## Links

**kube-explorer** is available on: [GitHub](https://github.com/qbituniverse/kube-explorer) - [DockerHub](https://cloud.docker.com/repository/docker/qbituniverse/kube-explorer) - [Web](http://qbituniverse.com)