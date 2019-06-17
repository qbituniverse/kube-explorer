var _Clusters_Catalog = [];

async function loadClusters() {
    await loadCatalogMatrix().then(function () {
        loadClustersCatalog();
    });
}

function loadClustersCatalog() {
    clearSearch();
    showGridLoading(0, "Loading clusters");
    var matrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix));
    if (matrix === null) {
        showGridError();
        return;
    }
    var clusters = matrix.clusters;
    if (clusters === null) {
        showGridError();
        return;
    }
    _Clusters_Catalog = clusters;
    populateClustersCatalog(clusters);
    showGridResults();
}

function populateClustersCatalog(results) {
    var subsInd = 1;
    var clusterInd = 1;
    var divLevel0 = $(".list-group-root");
    divLevel0.html("");
    $.each(results,
        function (k, r) {
            var divLevel1 = $("<div>", { class: "divlevel1" });
            var aLevel1 = $("<a>",
                {
                    href: "#item-" + subsInd,
                    class: "list-group-item level1",
                    "data-toggle": "collapse"
                });
            var iLevel1 = $("<i>", { class: "glyphicon glyphicon-star" });
            aLevel1.append(iLevel1);
            var spanCopyLevel1 = $("<span>", { id: "copy-item-" + subsInd, text: r.name });
            aLevel1.append(spanCopyLevel1);
            var spanCounter = $("<span>",
                {
                    class: "well well-sm",
                    text: countClustersInSubscription(k)
                });
            divLevel1.append(aLevel1);
            divLevel1.append(spanCounter);
            var spanCopyIconLevel1 = $("<span>",
                {
                    class: "glyphicon glyphicon-copy",
                    "aria-hidden": true
                });
            var buttonCopyLevel1 = $("<button>",
                {
                    class: "btn btn-light clipcopy",
                    "data-clipboard-target": "#copy-item-" + subsInd
                });
            buttonCopyLevel1.append(spanCopyIconLevel1);
            divLevel1.append(buttonCopyLevel1);
            var imageUsageBySubscription = searchSubscriptionLevelImageUsage(r.name);
            var chartUsageBySubscription = searchSubscriptionLevelChartUsage(r.name);
            if (imageUsageBySubscription.length > 0) {
                var spanLevel1ImageUsageSubscription = $("<span>", { class: "usage-image-subs" });
                var spanLevel1ImageUsageSubscriptionCount = $("<span>", { class: "usage-image-subs-count", text: imageUsageBySubscription.length });
                spanLevel1ImageUsageSubscription.append(spanLevel1ImageUsageSubscriptionCount);
                iLevel1.after(spanLevel1ImageUsageSubscription);

                var divSubscriptionModalBodyLevel1 = $("<div>");
                var imageSubscriptionUsageNameUnique = dedupeImageUsage(imageUsageBySubscription);
                var hImageUsage = $("<h4>", { text: "Images" });
                divSubscriptionModalBodyLevel1.append(hImageUsage);
                $.each(imageSubscriptionUsageNameUnique,
                    function (i, u) {
                        var pUsage = $("<p>", { class: "usage-k8s-items" });
                        var spanCluster = $("<span>", { text: u.cluster });
                        var spanImage = $("<span>", { text: u.image });
                        pUsage.append(spanCluster);
                        pUsage.append(spanImage);
                        divSubscriptionModalBodyLevel1.append(pUsage);
                    });
                if (chartUsageBySubscription.length > 0) {
                    var chartSubscriptionUsageNameUnique = dedupeChartUsage(chartUsageBySubscription);
                    var hChartUsage = $("<h4>", { text: "Charts" });
                    divSubscriptionModalBodyLevel1.append(hChartUsage);
                    $.each(chartSubscriptionUsageNameUnique,
                        function (i, u) {
                            var pUsage = $("<p>", { class: "usage-k8s-items" });
                            var spanCluster = $("<span>", { text: u.cluster });
                            var spanImage = $("<span>", { text: u.chart });
                            pUsage.append(spanCluster);
                            pUsage.append(spanImage);
                            divSubscriptionModalBodyLevel1.append(pUsage);
                        });
                }
                var divSubscriptionModalLevel1 = createModal(
                    "modal-" + subsInd,
                    "Images & Charts",
                    divSubscriptionModalBodyLevel1);
                $(".grid-modals").append(divSubscriptionModalLevel1);
                var btnSubscriptionShowDependenciesLevel1 = $("<button>",
                    {
                        type: "button",
                        class: "btn btn-info",
                        "data-toggle": "modal",
                        "data-target": "#modal-" + subsInd
                    });
                var spanSubscriptionDependenciesIconLevel1 = $("<span>",
                    {
                        class: "glyphicon glyphicon-wrench",
                        "aria-hidden": true
                    });
                btnSubscriptionShowDependenciesLevel1.append(spanSubscriptionDependenciesIconLevel1);
                btnSubscriptionShowDependenciesLevel1.append("Usage");
                divLevel1.append(btnSubscriptionShowDependenciesLevel1);
            }
            
            if (chartUsageBySubscription.length > 0) {
                var spanLevel1ChartUsageSubscription = $("<span>", { class: "usage-chart-subs" });
                var spanLevel1ChartUsageSubscriptionCount = $("<span>", { class: "usage-chart-subs-count", text: chartUsageBySubscription.length });
                spanLevel1ChartUsageSubscription.append(spanLevel1ChartUsageSubscriptionCount);
                iLevel1.after(spanLevel1ChartUsageSubscription);
            }
            var btnBrowseSubscription = $("<button>",
                {
                    type: "button",
                    class: "btn btn-primary",
                    onclick: "composeAzureUrl(\"" + r.resourceId + "\");"
                });
            var spanSubscriptionIcon = $("<span>",
                {
                    class: "glyphicon glyphicon-link",
                    "aria-hidden": true
                });
            btnBrowseSubscription.append(spanSubscriptionIcon);
            btnBrowseSubscription.append("Subscription");
            divLevel1.append(btnBrowseSubscription);
            divLevel0.append(divLevel1);
            var divLevel1Details = $("<div>",
                {
                    id: "item-" + subsInd,
                    class: "list-group collapse"
                });
            $.each(r.clusters,
                function (j, c) {
                    var divLevel2 = $("<div>", { class: "divlevel2" });
                    var aLevel2 = $("<a>",
                        {
                            href: "#item-" + subsInd + "-" + clusterInd,
                            class: "list-group-item level2",
                            "data-toggle": "collapse"
                        });
                    var iLevel2 = $("<i>", { class: "glyphicon glyphicon-hdd" });
                    aLevel2.append(iLevel2);
                    var spanCopyLevel2 = $("<span>", { id: "copy-item-" + subsInd + "-" + clusterInd, text: c.name });
                    aLevel2.append(spanCopyLevel2);
                    divLevel2.append(aLevel2);
                    var spanCounter = $("<span>",
                        {
                            class: "well well-sm",
                            text: countDeploymentsInCluster(c.deployment.items)
                        });
                    divLevel2.append(spanCounter);
                    var spanCopyIconLevel2 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyLevel2 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-item-" + subsInd + "-" + clusterInd
                        });
                    buttonCopyLevel2.append(spanCopyIconLevel2);
                    divLevel2.append(buttonCopyLevel2);
                    var imageUsageByCluster = searchClusterLevelImageUsage(c.name);
                    var chartUsageByCluster = searchClusterLevelChartUsage(c.name);
                    if (imageUsageByCluster.length > 0) {
                        var spanLevel2ImageUsageCluster = $("<span>", { class: "usage-image-clust" });
                        var spanLevel2ImageUsageClusterCount = $("<span>", { class: "usage-image-clust-count", text: imageUsageByCluster.length });
                        spanLevel2ImageUsageCluster.append(spanLevel2ImageUsageClusterCount);
                        iLevel2.after(spanLevel2ImageUsageCluster);

                        var divClusterModalBodyLevel2 = $("<div>");
                        var imageClusterUsageNameUnique = dedupeImageUsage(imageUsageByCluster);
                        var hImageUsage = $("<h4>", { text: "Images" });
                        divClusterModalBodyLevel2.append(hImageUsage);
                        $.each(imageClusterUsageNameUnique,
                            function (i, u) {
                                var pUsage = $("<p>", { class: "usage-k8s-items" });
                                var spanImage = $("<span>", { text: u.image });
                                pUsage.append(spanImage);
                                divClusterModalBodyLevel2.append(pUsage);
                            });
                        if (chartUsageByCluster.length > 0) {
                            var chartClusterUsageNameUnique = dedupeChartUsage(chartUsageByCluster);
                            var hChartUsage = $("<h4>", { text: "Charts" });
                            divClusterModalBodyLevel2.append(hChartUsage);
                            $.each(chartClusterUsageNameUnique,
                                function (i, u) {
                                    var pUsage = $("<p>", { class: "usage-k8s-items" });
                                    var spanImage = $("<span>", { text: u.chart });
                                    pUsage.append(spanImage);
                                    divClusterModalBodyLevel2.append(pUsage);
                                });
                        }
                        var divClusterModalLevel2 = createModal(
                            "modal-" + subsInd + "-" + clusterInd,
                            "Images & Charts",
                            divClusterModalBodyLevel2);
                        $(".grid-modals").append(divClusterModalLevel2);
                        var btnClusterShowDependenciesLevel2 = $("<button>",
                            {
                                type: "button",
                                class: "btn btn-info",
                                "data-toggle": "modal",
                                "data-target": "#modal-" + subsInd + "-" + clusterInd
                            });
                        var spanClusterDependenciesIconLevel2 = $("<span>",
                            {
                                class: "glyphicon glyphicon-wrench",
                                "aria-hidden": true
                            });
                        btnClusterShowDependenciesLevel2.append(spanClusterDependenciesIconLevel2);
                        btnClusterShowDependenciesLevel2.append("Usage");
                        divLevel2.append(btnClusterShowDependenciesLevel2);
                    }
                    
                    if (chartUsageByCluster.length > 0) {
                        var spanLevel2ChartUsageCluster = $("<span>", { class: "usage-chart-clust" });
                        var spanLevel2ChartUsageClusterCount = $("<span>", { class: "usage-chart-clust-count", text: chartUsageByCluster.length });
                        spanLevel2ChartUsageCluster.append(spanLevel2ChartUsageClusterCount);
                        iLevel2.after(spanLevel2ChartUsageCluster);
                    }
                    var btnBrowseCluster = $("<button>",
                        {
                            type: "button",
                            class: "btn btn-primary",
                            onclick: "composeAzureUrl(\"" + c.id + "\");"
                        });
                    var spanClusterIcon = $("<span>",
                        {
                            class: "glyphicon glyphicon-link",
                            "aria-hidden": true
                        });
                    btnBrowseCluster.append(spanClusterIcon);
                    btnBrowseCluster.append("Cluster");
                    divLevel2.append(btnBrowseCluster);
                    var btnBrowseComponents = $("<button>",
                        {
                            type: "button",
                            class: "btn btn-primary",
                            onclick: "composeAzureUrl(\"" + r.resourceId + "/resourcegroups/" + c.properties.nodeResourceGroup + "\");"
                        });
                    var spanComponentsIcon = $("<span>",
                        {
                            class: "glyphicon glyphicon-link",
                            "aria-hidden": true
                        });
                    btnBrowseComponents.append(spanComponentsIcon);
                    btnBrowseComponents.append("Components");
                    divLevel2.append(btnBrowseComponents);
                    divLevel1Details.append(divLevel2);
                    var divLevel2Details = $("<div>",
                        {
                            id: "item-" + subsInd + "-" + clusterInd,
                            class: "list-group collapse"
                        });
                    var divLevel3 = $("<div>", { class: "list-group-item divlevel3 row" });
                    var divDetails = $("<div>", { class: "col-lg-12 details-header" });
                    var h3Details = $("<h4>", { text: "Cluster Details" });
                    divDetails.append(h3Details);
                    divLevel3.append(divDetails);
                    var divDetails0 = $("<div>", { class: "col-lg-12 well well-sm" });
                    var divDetails1 = $("<div>", { class: "col-lg-12" });
                    var pDetails1 = $("<p>");
                    var labelDetails1 = $("<label>", { text: "Uri:" });
                    var spanDetails1 = $("<span>", { id: "copy-name-item-" + subsInd + "-" + clusterInd + "-" + 1, text: c.properties.fqdn });
                    var spanCopyIconDiv1 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyDiv1 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-name-item-" + subsInd + "-" + clusterInd + "-" + 1
                        });
                    buttonCopyDiv1.append(spanCopyIconDiv1);
                    pDetails1.append(labelDetails1);
                    pDetails1.append(spanDetails1);
                    pDetails1.append(buttonCopyDiv1);
                    divDetails1.append(pDetails1);
                    divDetails0.append(divDetails1);
                    var divDetails2 = $("<div>", { class: "col-lg-12" });
                    var pDetails2 = $("<p>");
                    var labelDetails2 = $("<label>", { text: "Dns:" });
                    var spanDetails2 = $("<span>", { id: "copy-name-item-" + subsInd + "-" + clusterInd + "-" + 2, text: composeAzureDns(c.properties.dnsPrefix, c.location) });
                    var spanCopyIconDiv2 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyDiv2 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-name-item-" + subsInd + "-" + clusterInd + "-" + 2
                        });
                    buttonCopyDiv2.append(spanCopyIconDiv2);
                    pDetails2.append(labelDetails2);
                    pDetails2.append(spanDetails2);
                    pDetails2.append(buttonCopyDiv2);
                    divDetails2.append(pDetails2);
                    divDetails0.append(divDetails2);
                    var divDetails3 = $("<div>", { class: "col-lg-6" });
                    var pDetails3 = $("<p>");
                    var labelDetails3 = $("<label>", { text: "Cluster Name:" });
                    var spanDetails3 = $("<span>", { id: "copy-name-item-" + subsInd + "-" + clusterInd + "-" + 3, text: deriveClusterName(c.id) });
                    var spanCopyIconDiv3 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyDiv3 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-name-item-" + subsInd + "-" + clusterInd + "-" + 3
                        });
                    buttonCopyDiv3.append(spanCopyIconDiv3);
                    pDetails3.append(labelDetails3);
                    pDetails3.append(spanDetails3);
                    pDetails3.append(buttonCopyDiv3);
                    divDetails3.append(pDetails3);
                    divDetails0.append(divDetails3);
                    var divDetails4 = $("<div>", { class: "col-lg-6" });
                    var pDetails4 = $("<p>");
                    var labelDetails4 = $("<label>", { text: "Location:" });
                    var spanDetails4 = $("<span>", { text: c.location });
                    pDetails4.append(labelDetails4);
                    pDetails4.append(spanDetails4);
                    divDetails4.append(pDetails4);
                    divDetails0.append(divDetails4);
                    var divDetails5 = $("<div>", { class: "col-lg-6" });
                    var pDetails5 = $("<p>");
                    var labelDetails5 = $("<label>", { text: "Resource Group:" });
                    var spanDetails5 = $("<span>", { id: "copy-name-item-" + subsInd + "-" + clusterInd + "-" + 5, text: deriveResourceGroup(c.id) });
                    var spanCopyIconDiv5 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyDiv5 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-name-item-" + subsInd + "-" + clusterInd + "-" + 5
                        });
                    buttonCopyDiv5.append(spanCopyIconDiv5);
                    pDetails5.append(labelDetails5);
                    pDetails5.append(spanDetails5);
                    pDetails5.append(buttonCopyDiv5);
                    divDetails5.append(pDetails5);
                    divDetails0.append(divDetails5);
                    var divDetails6 = $("<div>", { class: "col-lg-6" });
                    var pDetails6 = $("<p>");
                    var labelDetails6 = $("<label>", { text: "RBAC Enabled:" });
                    var spanDetails6 = $("<span>", { text: c.properties.enableRBAC });
                    pDetails6.append(labelDetails6);
                    pDetails6.append(spanDetails6);
                    divDetails6.append(pDetails6);
                    divDetails0.append(divDetails6);
                    var divDetails7 = $("<div>", { class: "col-lg-6" });
                    var pDetails7 = $("<p>");
                    var labelDetails7 = $("<label>", { text: "K8s Version:" });
                    var spanDetails7 = $("<span>", { text: c.properties.kubernetesVersion });
                    pDetails7.append(labelDetails7);
                    pDetails7.append(spanDetails7);
                    divDetails7.append(pDetails7);
                    divDetails0.append(divDetails7);    
                    divLevel3.append(divDetails0);
                    var divDeployments = $("<div>", { class: "col-lg-12 details-header" });
                    var h3Deployments = $("<h4>", { text: "K8s Deployments" });
                    divDeployments.append(h3Deployments);
                    divLevel3.append(divDeployments);
                    $.each(c.deployment.items,
                        function (n, d) {
                            var divDeployments0 = $("<div>", { class: "col-lg-12 well well-sm" });
                            var divDeployments1 = $("<div>", { class: "col-lg-8" });
                            var pDeployments1 = $("<p>");
                            var labelDeployments1 = $("<label>", { text: "Name:" });
                            var spanDeployments1 = $("<span>", { text: d.metadata.name });
                            pDeployments1.append(labelDeployments1);
                            pDeployments1.append(spanDeployments1);
                            divDeployments1.append(pDeployments1);
                            divDeployments0.append(divDeployments1);
                            var divDeployments2 = $("<div>", { class: "col-lg-4" });
                            var pDeployments2 = $("<p>");
                            var labelDeployments2 = $("<label>", { text: "Deployed:" });
                            var spanDeployments2 = $("<span>", { text: dateFormatter(d.metadata.creationTimestamp) });
                            pDeployments2.append(labelDeployments2);
                            pDeployments2.append(spanDeployments2);
                            divDeployments2.append(pDeployments2);
                            divDeployments0.append(divDeployments2);
                            var divDeployments3 = $("<div>", { class: "col-lg-12" });
                            var pDeployments3 = $("<p>");
                            var labelDeployments3 = $("<label>", { text: "Chart:" });
                            var spanDeployments3 = $("<span>", { id: "copy-name-item-chart" + subsInd + "-" + clusterInd + "-" + n, text: d.metadata.labels.chart });
                            var spanCopyIconDiv3 = $("<span>",
                                {
                                    class: "glyphicon glyphicon-copy",
                                    "aria-hidden": true
                                });
                            var buttonCopyDiv3 = $("<button>",
                                {
                                    class: "btn btn-light clipcopy",
                                    "data-clipboard-target": "#copy-name-item-chart" + subsInd + "-" + clusterInd + "-" + n
                                });
                            buttonCopyDiv3.append(spanCopyIconDiv3);
                            pDeployments3.append(labelDeployments3);
                            pDeployments3.append(spanDeployments3);
                            if (d.metadata.labels.chart != null) { pDeployments3.append(buttonCopyDiv3); }
                            var chartUsage = searchClusterChartUsage(d.metadata.labels.chart);
                            if (chartUsage.length > 0) {
                                spanDeployments3.addClass("usage-chart-item");
                                divDeployments0.addClass("usage-chart-item");
                            }
                            divDeployments3.append(pDeployments3);
                            divDeployments0.append(divDeployments3);
                            $.each(d.spec.template.spec.containers,
                                function (m, i) {
                                    var divDeployments4 = $("<div>", { class: "col-lg-12" });
                                    var pDeployments4 = $("<p>");
                                    var labelDeployments4 = $("<label>");
                                    if (m === 0) labelDeployments4.append("Images:");
                                    else labelDeployments4.append("&nbsp;");
                                    var spanDeployments4 = $("<span>", { id: "copy-name-item-chart" + subsInd + "-" + clusterInd + "-" + n + "-" + m, text: i.image });
                                    var spanCopyIconDiv4 = $("<span>",
                                        {
                                            class: "glyphicon glyphicon-copy",
                                            "aria-hidden": true
                                        });
                                    var buttonCopyDiv4 = $("<button>",
                                        {
                                            class: "btn btn-light clipcopy",
                                            "data-clipboard-target": "#copy-name-item-chart" + subsInd + "-" + clusterInd + "-" + n + "-" + m
                                        });
                                    buttonCopyDiv4.append(spanCopyIconDiv4);
                                    pDeployments4.append(labelDeployments4);
                                    pDeployments4.append(spanDeployments4);
                                    if (i.image != null) { pDeployments4.append(buttonCopyDiv4); }
                                    var imageUsage = searchClusterImageUsage(i.image);
                                    if (imageUsage.length > 0) {
                                        spanDeployments4.addClass("usage-image-item");
                                        divDeployments0.addClass("usage-image-item");
                                    }
                                    divDeployments4.append(pDeployments4);
                                    divDeployments0.append(divDeployments4);
                                });
                            divLevel3.append(divDeployments0);
                        });
                    divLevel2Details.append(divLevel3);
                    divLevel1Details.append(divLevel2Details);
                    clusterInd++;
                });
            divLevel0.append(divLevel1Details);
            subsInd++;
            clusterInd = 1;
        });

    var repoCounterText = " subscription";
    if (subsInd - 1 > 1) repoCounterText = " subscriptions";
    $('.repocounter').text(subsInd - 1 + repoCounterText);
}

function countClustersInSubscription(subscription) {
    var count = _Clusters_Catalog[subscription].clusters.length;
    if (count === 0) return "Empty";
    if (count === 1) return "1 cluster";
    return count + " clusters";
}

function deriveClusterName(resourceId) {
    return resourceId.split("/")[8];
}

function deriveResourceGroup(resourceId) {
    return resourceId.split("/")[4];
}