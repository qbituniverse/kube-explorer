async function loadDashboard() {
    await loadCatalogMatrix().then(function () {
        showGridResults();
        if (_User_Policies.includes(_Policy_Acr)) {
            $(".images-panel").fadeIn(1000);
            $(".charts-panel").fadeIn(1000);
            loadDashboardImages();
            loadDashboardCharts();
        }
        if (_User_Policies.includes(_Policy_Aks)) {
            $(".k8s-panel").fadeIn(1000);
            loadDashboardK8s();
        }
    });
}

function loadDashboardImages() {
    showImagesPanelLoading();
    var matrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix));
    if (matrix === null) {
        showImagesPanelError();
        return;
    }
    var images = matrix.images;
    if (images === null) {
        showImagesPanelError();
        return;
    }
    populateImagesPanel(images);
    showImagesPanelResults();
}

function loadDashboardCharts() {
    showChartsPanelLoading();
    var matrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix));
    if (matrix === null) {
        showChartsPanelError();
        return;
    }
    var charts = matrix.charts;
    if (charts === null) {
        showChartsPanelError();
        return;
    }
    populateChartsPanel(charts);
    showChartsPanelResults();
}

function loadDashboardK8s() {
    showK8sPanelLoading();
    var matrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix));
    if (matrix === null) {
        showK8sPanelError();
        return;
    }
    var clusters = matrix.clusters;
    if (clusters === null) {
        showK8sPanelError();
        return;
    }
    populateK8sPanel(clusters);
    showK8sPanelResults();
}

function populateImagesPanel(result) {
    var imagesTotal = 0;
    var tagsTotal = 0;
    $.each(result,
        function(k, r) {
            imagesTotal++;
            tagsTotal += r.length;
        });
    var pLevel0 = $(".images-body > p.level0");
    var divImagesPanel = $(".images-body");
    var spanRepos = $("<span>",
        {
            class: "well well-sm text-center",
            text: imagesTotal + " images"
        });
    pLevel0.append(spanRepos);
    var spanTags = $("<span>",
        {
            class: "well well-sm text-center",
            text: tagsTotal + " tags"
        });
    pLevel0.append(spanTags);
    var pLinks = $("<p>", { class: "repo-links text-center" });
    var buttonViewRepo = $("<button>",
        {
            type: "button",
            class: "btn btn-success",
            onclick: "location.href='Images'"
        });
    var spanViewIcon = $("<span>",
        {
            class: "glyphicon glyphicon-list",
            "aria-hidden": true
        });
    buttonViewRepo.append(spanViewIcon);
    buttonViewRepo.append("Browse Images");
    pLinks.append(buttonViewRepo);
    divImagesPanel.append(pLinks);
}

function populateChartsPanel(result) {
    var chartsTotal = 0;
    var versionsTotal = 0;
    $.each(result,
        function (k, r) {
            chartsTotal++;
            versionsTotal += r.length;
        });
    var pLevel0 = $(".charts-body > p.level0");
    var divChartsPanel = $(".charts-body");
    var spanRepos = $("<span>",
        {
            class: "well well-sm text-center",
            text: chartsTotal + " charts"
        });
    pLevel0.append(spanRepos);
    var spanVersions = $("<span>",
        {
            class: "well well-sm text-center",
            text: versionsTotal + " versions"
        });
    pLevel0.append(spanVersions);
    var pLinks = $("<p>", { class: "repo-links text-center" });
    var buttonViewRepo = $("<button>",
        {
            type: "button",
            class: "btn btn-success",
            onclick: "location.href='Charts'"
        });
    var spanViewIcon = $("<span>",
        {
            class: "glyphicon glyphicon-list",
            "aria-hidden": true
        });
    buttonViewRepo.append(spanViewIcon);
    buttonViewRepo.append("Browse Charts");
    pLinks.append(buttonViewRepo);
    divChartsPanel.append(pLinks);
}

function populateK8sPanel(result) {
    var divK8sPanel = $(".k8s-body");
    $.each(result,
        function(k, r) {
            var pSubscription = $("<p>",
                {
                    class: "well well-sm text-center level0",
                    text: "Azure Subscription"
                });
            pSubscription.append("<br />");
            strongName = $("<strong>", { text: r.name });
            pSubscription.append(strongName);
            $.each(r.clusters,
                function (j, c) {
                    var spanCluster = $("<span>", { class: "well well-sm text-center" });
                    spanCluster.append("<strong>" + c.name + "</strong>");
                    spanCluster.append("<br />");
                    var spanDeployments = $("<span>",
                        {
                            class: "well well-sm text-center",
                            text: countDeploymentsInCluster(c.deployment.items)
                        });
                    spanCluster.append(spanDeployments);
                    var spanVersion = $("<span>",
                        {
                            class: "well well-sm text-center",
                            text: c.properties.kubernetesVersion
                        });
                    spanCluster.append(spanVersion);
                    var spanLocation = $("<span>",
                        {
                            class: "well well-sm text-center",
                            text: c.location
                        });
                    spanCluster.append(spanLocation);
                    pSubscription.append(spanCluster);
                });
            divK8sPanel.append(pSubscription);
        });
    var pLinks = $("<p>", { class: "repo-links text-center" });
    var buttonViewAks = $("<button>",
        {
            type: "button",
            class: "btn btn-success",
            onclick: "location.href='K8S'"
        });
    var spanViewIcon = $("<span>",
        {
            class: "glyphicon glyphicon-list",
            "aria-hidden": true
        });
    buttonViewAks.append(spanViewIcon);
    buttonViewAks.append("Browse Clusters");
    pLinks.append(buttonViewAks);
    divK8sPanel.append(pLinks);
}

function openAdmin() {
    alert('admin');
}

function showImagesPanelLoading() {
    $(".images-body").hide();
    $(".images-loader").fadeIn(1000);
    $(".images-error").hide();
}

function showImagesPanelResults() {
    $(".images-loader").hide();
    $(".images-body").fadeIn(1000);
    $(".images-error").hide();
}

function showImagesPanelError() {
    $(".images-loader").hide();
    $(".images-body").hide();
    $(".images-error").fadeIn(1000);
}

function showChartsPanelLoading() {
    $(".charts-body").hide();
    $(".charts-loader").fadeIn(1000);
    $(".charts-error").hide();
}

function showChartsPanelResults() {
    $(".charts-loader").hide();
    $(".charts-body").fadeIn(1000);
    $(".charts-error").hide();
}

function showChartsPanelError() {
    $(".charts-loader").hide();
    $(".charts-body").hide();
    $(".charts-error").fadeIn(1000);
}

function showK8sPanelLoading() {
    $(".k8s-body").hide();
    $(".k8s-loader").fadeIn(1000);
    $(".k8s-error").hide();
}

function showK8sPanelResults() {
    $(".k8s-loader").hide();
    $(".k8s-body").fadeIn(1000);
    $(".k8s-error").hide();
}

function showK8sPanelError() {
    $(".k8s-loader").hide();
    $(".k8s-body").hide();
    $(".k8s-error").fadeIn(1000);
}