var _Charts_Catalog = [];

async function loadCharts() {
    await loadCatalogMatrix().then(function () {
        loadChartsCatalog();
    });
}

function loadChartsCatalog() {
    clearSearch();
    showGridLoading(0, "Loading charts repository");
    var matrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix));
    if (matrix === null) {
        showGridError();
        return;
    }
    var charts = matrix.charts;
    if (charts === null) {
        showGridError();
        return;
    }
    _Charts_Catalog = charts;
    populateChartsCatalog(charts);
    showGridResults();
}

function deleteChartRepository(chartName) {
    var isApproved = false;
    var approvalPrompt = "You are deleting the entire chart repository '" + chartName + "'.\n\n";
    approvalPrompt +=
        "Please type the chart repository name into the textbox and click OK to confirm your action.";
    isApproved = prompt(approvalPrompt) === chartName;

    if (!isApproved) {
        alert("Wrong chart repository name enterd");
        return;
    }

    var repositoryDeleted = "The followig chart repository will be deleted.\n\n";
    repositoryDeleted += chartName + "\n\n";
    repositoryDeleted += "This action won't be undone. Do you want to proceed?";
    var toDelete = confirm(repositoryDeleted);
    if (!toDelete) return;

    showGridLoading(0, "Deleting chart repository '" + chartName + "'");
    $.ajax({
        async: true,
        type: "POST",
        url: "/Charts/DeleteChartRepository",
        data: { chartName: chartName },
        success: function (apiResponse) {
            if (apiResponse.result) {
                console.log("Delete '" + chartName + "' completed successfully");
                showNoGrid();
                alert("Delete '" + chartName + "' completed successfully");
                clearCatalogMatrix();
                loadCharts();
            } else {
                console.log("Delete '" + chartName + "' not completed");
                showGridResults();
                alert("Delete '" + chartName + "' not completed");
            }
        },
        error: function (ex) {
            if (ex.readyState === 0) return;
            console.log("Delete chart repository '" + chartName + "' failed with status code: " + ex.responseJSON.statusCode);
            console.log("Delete chart repository '" + chartName + "' failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Delete chart repository '" + chartName + "' failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridResults();
            alert("Error ocurred, try again...");
        }
    });
}

function deleteChartVersion(chartBlobUrl, chartInd) {
    var isApproved = false;
    if (chartInd <= 3) {
        var approvalPrompt = "You are deleting a chart version that is ";
        if (chartInd === 1) {
            approvalPrompt += "the latest version.\n\n";
        } else if (chartInd === 2) {
            approvalPrompt += "only 1 version away from the latest.\n\n";
        } else {
            approvalPrompt += "only " + (chartInd - 1) + " versions away from the latest.\n\n";
        }
        approvalPrompt += "Please type 'yes' into the textbox and click OK to confirm your action.";
        isApproved = prompt(approvalPrompt) === "yes";
    } else {
        isApproved = true;
    }

    if (!isApproved) {
        alert("Wrong confirmation entered");
        return;
    }

    var blobsSearch = searchForBlobUrl(chartBlobUrl);
    var chartsDeleted = "";
    var blobsDeleted = "";

    if (blobsSearch.length > 1) {
        chartsDeleted += "The followig chart versions will be deleted. This is because they share the same blob storage url.\n";
        $.each(blobsSearch,
            function(i, b) {
                blobsDeleted += b + "\n";
            });
    } else {
        chartsDeleted += "The followig chart version will be deleted.\n";
        blobsDeleted = blobsSearch + "\n";
    }

    chartsDeleted += "\n" + blobsDeleted + "\n";
    chartsDeleted += "This action won't be undone. Do you want to proceed?";

    var toDelete = confirm(chartsDeleted);
    if (!toDelete) return;

    showGridLoading(0, "Deleting chart version '" + chartBlobUrl + "'");
    $.ajax({
        async: true,
        type: "POST",
        url: "/Charts/DeleteChartVersion",
        data: { chartBlobUrl: chartBlobUrl },
        success: function (apiResponse) {
            if (apiResponse.result) {
                console.log("Delete '" + chartBlobUrl + "' completed successfully");
                showNoGrid();
                alert("Delete '" + chartBlobUrl + "' completed successfully");
                clearCatalogMatrix();
                loadCharts();
            } else {
                console.log("Delete '" + chartBlobUrl + "' not completed");
                showGridResults();
                alert("Delete '" + chartBlobUrl + "' not completed");
            }
        },
        error: function (ex) {
            if (ex.readyState === 0) return;
            console.log("Delete chart version '" + chartBlobUrl + "' failed with status code: " + ex.responseJSON.statusCode);
            console.log("Delete chart version '" + chartBlobUrl + "' failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Delete chart version '" + chartBlobUrl + "' failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridResults();
            alert("Error ocurred, try again...");
        }
    });
}

function populateChartsCatalog(results) {
    var repoInd = 1;
    var chartInd = 1;
    var divLevel0 = $(".list-group-root");
    divLevel0.html("");
    $.each(results,
        function (i, r) {
            var divLevel1 = $("<div>", { class: "divlevel1" });
            var aLevel1 = $("<a>",
                {
                    href: "#item-" + repoInd,
                    class: "list-group-item level1",
                    "data-toggle": "collapse"
                });
            var iLevel1 = $("<i>", { class: "glyphicon glyphicon-folder-open" });
            aLevel1.append(iLevel1);
            var spanCopyLevel1 = $("<span>", { id: "copy-item-" + repoInd, text: i });
            aLevel1.append(spanCopyLevel1);
            var spanCounter = $("<span>",
                {
                    class: "well well-sm",
                    text: countVersionsInChartsRepository(i)
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
                    "data-clipboard-target": "#copy-item-" + repoInd
                });
            buttonCopyLevel1.append(spanCopyIconLevel1);
            divLevel1.append(buttonCopyLevel1);
            var chartUsageName = searchChartUsageByName(i);
            if (chartUsageName.length > 0) {
                var spanLevel1Usage = $("<span>", { class: "usage-k8s" });
                var spanLevel1UsageCount = $("<span>", { class: "usage-k8s-count", text: chartUsageName.length });
                spanLevel1Usage.append(spanLevel1UsageCount);
                iLevel1.after(spanLevel1Usage);
                var divModalBodyLevel1 = $("<div>");
                var chartUsageNameUnique = dedupeSubscriptionClusterUsage(chartUsageName);
                $.each(chartUsageNameUnique,
                    function(i, u) {
                        var pUsage = $("<p>", { class: "usage-k8s-items" });
                        var spanSubscription = $("<span>", { text: u.subscription });
                        var spanCluster = $("<span>", { text: u.cluster });
                        pUsage.append(spanSubscription);
                        pUsage.append(spanCluster);
                        divModalBodyLevel1.append(pUsage);
                    });
                var divModalLevel1 = createModal(
                    "modal-" + repoInd,
                    i,
                    divModalBodyLevel1);
                $(".grid-modals").append(divModalLevel1);
                var btnShowDependenciesLevel1 = $("<button>",
                    {
                        type: "button",
                        class: "btn btn-info",
                        "data-toggle": "modal",
                        "data-target": "#modal-" + repoInd
                    });
                var spanDependenciesIconLevel1 = $("<span>",
                    {
                        class: "glyphicon glyphicon-wrench",
                        "aria-hidden": true
                    });
                btnShowDependenciesLevel1.append(spanDependenciesIconLevel1);
                btnShowDependenciesLevel1.append("Usage");
                divLevel1.append(btnShowDependenciesLevel1);
            }
            if (_User_Roles.includes(_Role_ArcContributor)) {
                var btnDeleteRepo = $("<button>",
                    {
                        type: "button",
                        class: "btn btn-danger",
                        onclick: "deleteChartRepository(\"" + i + "\");"
                    });
                var spanDeleteIcon = $("<span>",
                    {
                        class: "glyphicon glyphicon-remove-sign",
                        "aria-hidden": true
                    });
                btnDeleteRepo.append(spanDeleteIcon);
                btnDeleteRepo.append("Delete Repository");
                divLevel1.append(btnDeleteRepo);
            }
            divLevel0.append(divLevel1);
            var divLevel1Details = $("<div>",
                {
                    id: "item-" + repoInd,
                    class: "list-group collapse"
                });
            $.each(r,
                function (j, c) {
                    var divLevel2 = $("<div>", { class: "divlevel2" });
                    var aLevel2 = $("<a>",
                        {
                            href: "#item-" + repoInd + "-" + chartInd,
                            class: "list-group-item level2",
                            "data-toggle": "collapse"
                        });
                    var iLevel2 = $("<i>", { class: "glyphicon glyphicon-tag" });
                    aLevel2.append(iLevel2);
                    var spanCopyLevel2 = $("<span>", { id: "copy-item-" + repoInd + "-" + chartInd, text: c.version });
                    aLevel2.append(spanCopyLevel2);
                    var spanAge = $("<span>",
                        {
                            class: "well well-sm",
                            text: calculateAge(c.created)
                        });
                    divLevel2.append(aLevel2);
                    divLevel2.append(spanAge);
                    var spanCopyIconLevel2 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyLevel2 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-item-" + repoInd + "-" + chartInd
                        });
                    buttonCopyLevel2.append(spanCopyIconLevel2);
                    divLevel2.append(buttonCopyLevel2);
                    var chartUsageNameValue = searchChartUsageByNameVersion(i + "-" + c.version);
                    if (chartUsageNameValue.length > 0) {
                        var spanLevel2Usage = $("<span>", { class: "usage-k8s" });
                        var spanLevel2UsageCount = $("<span>", { class: "usage-k8s-count", text: chartUsageNameValue.length });
                        spanLevel2Usage.append(spanLevel2UsageCount);
                        iLevel2.after(spanLevel2Usage);
                        var divModalBodyLevel2 = $("<div>");
                        var chartUsageNameValueUnique = dedupeSubscriptionClusterUsage(chartUsageNameValue);
                        $.each(chartUsageNameValueUnique,
                            function (i, u) {
                                var pUsage = $("<p>", { class: "usage-k8s-items" });
                                var spanSubscription = $("<span>", { text: u.subscription });
                                var spanCluster = $("<span>", { text: u.cluster });
                                pUsage.append(spanSubscription);
                                pUsage.append(spanCluster);
                                divModalBodyLevel2.append(pUsage);
                            });
                        var divModalLevel2 = createModal(
                            "modal-" + repoInd + "-" + chartInd,
                            i + "-" + c.version,
                            divModalBodyLevel2);
                        $(".grid-modals").append(divModalLevel2);
                        var btnShowDependenciesLevel2 = $("<button>",
                            {
                                type: "button",
                                class: "btn btn-info",
                                "data-toggle": "modal",
                                "data-target": "#modal-" + repoInd + "-" + chartInd
                            });
                        var spanDependenciesIconLevel2 = $("<span>",
                            {
                                class: "glyphicon glyphicon-wrench",
                                "aria-hidden": true
                            });
                        btnShowDependenciesLevel2.append(spanDependenciesIconLevel2);
                        btnShowDependenciesLevel2.append("Usage");
                        divLevel2.append(btnShowDependenciesLevel2);
                    }
                    if (_User_Roles.includes(_Role_ArcContributor)) {
                        var btnDeleteChart = $("<button>",
                            {
                                type: "button",
                                class: "btn btn-danger",
                                onclick: "deleteChartVersion(\"" +
                                    (c.urls !== null ? c.urls[0] : "") +
                                    "\"," +
                                    chartInd +
                                    ");"
                            });
                        var spanDeleteIcon = $("<span>",
                            {
                                class: "glyphicon glyphicon-remove-sign",
                                "aria-hidden": true
                            });
                        btnDeleteChart.append(spanDeleteIcon);
                        btnDeleteChart.append("Delete");
                        divLevel2.append(btnDeleteChart);
                    }
                    divLevel1Details.append(divLevel2);
                    var divLevel2Details = $("<div>",
                        {
                            id: "item-" + repoInd + "-" + chartInd,
                            class: "list-group collapse"
                        });
                    var divLevel3 = $("<div>", { class: "list-group-item divlevel3 row" });
                    var div0 = $("<div>", { class: "col-lg-12" });
                    var p0 = $("<p>");
                    var label0 = $("<label>", { text: "Name:" });
                    var span0 = $("<span>", { id: "copy-name-item-" + repoInd + "-" + chartInd + "-" + 0, text: i + "-" + c.version });
                    p0.append(label0);
                    p0.append(span0);
                    var spanCopyIconDiv0 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyDiv0 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-name-item-" + repoInd + "-" + chartInd + "-" + 0
                        });
                    buttonCopyDiv0.append(spanCopyIconDiv0);
                    p0.append(buttonCopyDiv0);
                    div0.append(p0);
                    divLevel3.append(div0);
                    var div1 = $("<div>", { class: "col-lg-12" });
                    var p1 = $("<p>");
                    var label1 = $("<label>", { text: "Description:" });
                    var span1 = $("<span>", { text: c.description });
                    p1.append(label1);
                    p1.append(span1);
                    div1.append(p1);
                    divLevel3.append(div1);
                    var div2 = $("<div>", { class: "col-lg-12" });
                    var p2 = $("<p>");
                    var label2 = $("<label>", { text: "Created:" });
                    var span2 = $("<span>", { text: dateFormatter(c.created) });
                    p2.append(label2);
                    p2.append(span2);
                    div2.append(p2);
                    divLevel3.append(div2);
                    var div3 = $("<div>", { class: "col-lg-12" });
                    var p3 = $("<p>");
                    var label3 = $("<label>", { text: "Maintainer:" });
                    var span3 = $("<span>", { text: c.maintainers !== null ? c.maintainers[0].name : "" });
                    p3.append(label3);
                    p3.append(span3);
                    div3.append(p3);
                    divLevel3.append(div3);
                    var div4 = $("<div>", { class: "col-lg-12" });
                    var p4 = $("<p>");
                    var label4 = $("<label>", { text: "Email:" });
                    var span4 = $("<span>", { text: c.maintainers !== null ? c.maintainers[0].email : "" });
                    p4.append(label4);
                    p4.append(span4);
                    div4.append(p4);
                    divLevel3.append(div4);
                    var div5 = $("<div>", { class: "col-lg-12" });
                    var p5 = $("<p>");
                    var label5 = $("<label>", { text: "Url:" });
                    var span5 = $("<span>", { text: c.urls !== null ? c.urls[0] : "" });
                    p5.append(label5);
                    p5.append(span5);
                    div5.append(p5);
                    divLevel3.append(div5);
                    var div6 = $("<div>", { class: "col-lg-12" });
                    var p6 = $("<p>");
                    var label6 = $("<label>", { text: "Digest:" });
                    var span6 = $("<span>", { text: c.digest });
                    p6.append(label6);
                    p6.append(span6);
                    div6.append(p6);
                    divLevel3.append(div6);
                    var div7 = $("<div>", { class: "col-lg-12" });
                    var p7 = $("<p>");
                    var label7 = $("<label>", { text: "Manifest Digest:" });
                    var span7 = $("<span>", { text: c.acrMetadata !== null ? c.acrMetadata.manifestDigest : "" });
                    p7.append(label7);
                    p7.append(span7);
                    div7.append(p7);
                    divLevel3.append(div7);
                    divLevel2Details.append(divLevel3);
                    divLevel1Details.append(divLevel2Details);
                    chartInd++;
                });
            divLevel0.append(divLevel1Details);
            repoInd++;
            chartInd = 1;
        });

    var repoCounterText = " repository";
    if (repoInd - 1 > 1) repoCounterText = " repositories";
    $('.repocounter').text(repoInd - 1 + repoCounterText);
}

function searchForBlobUrl(chartBlobUrl) {
    var blobs = [];
    $.each(_Charts_Catalog,
        function(i, r) {
            $.each(r,
                function(j, c) {
                    if (c.urls !== null) {
                        if (c.urls[0] === chartBlobUrl) {
                            blobs.push(c.name + " " + c.version);
                        }
                    }
                });
        });
    return blobs;
}

function countVersionsInChartsRepository(chartName) {
    var count = _Charts_Catalog[chartName].length;
    if (count === 0) return "Empty";
    if (count === 1) return "1 version";
    return count + " versions";
}

function calculateLastUpdatedChart(chartName) {
    var latest = _Charts_Catalog[chartName][0].created;
    var start = new Date(latest);
    var end = new Date();
    var diff = new Date(end - start);
    var days = Math.round(diff / 1000 / 60 / 60 / 24);
    if (days === 0) return "Updated today";
    if (days === 1) return "Updated yesterday";
    return "Updated " + days + " days ago";
}