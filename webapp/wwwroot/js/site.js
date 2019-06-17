const _Policy_Acr = "Acr";
const _Policy_Aks = "Aks";
const _Policy_DevOps = "DevOps";
const _Role_AcrReaader = "AcrReader";
const _Role_ArcContributor = "AcrContributor";
const _Catalog_Matrix = "_Catalog_Matrix";
var _Azure_Tenant = "";
var _User_Policies = "";
var _User_Roles = "";
var _Cache_Enabled = false;
var _Cache_Duration = 0;

async function loadCommon(azureTenant, userPolicies, userRoles, cacheEnabled, cacheDuration) {
    _Azure_Tenant = azureTenant;
    _User_Policies = userPolicies;
    _User_Roles = userRoles;
    _Cache_Enabled = cacheEnabled.toLowerCase() === "true";
    _Cache_Duration = cacheDuration;

    new ClipboardJS('.clipcopy');
    
    $('#search').keyup(function () {
        var currentQuery = $('#search').val().toLowerCase();
        var searchPlaceholder = $('#search').attr("placeholder");
        var searchType = "items";
        switch (searchPlaceholder) {
        case "Search Images":
        case "Search Charts":
            searchType = "repositories";
            break;
        case "Search Subscriptions":
            searchType = "subscriptions";
            break;
        }
        if (currentQuery !== "") {
            var rcount = 0;
            $(".list-group-root.level0 > div.divlevel1").hide();
            $('.list-group.in').collapse('hide');
            $(".list-group-root.level0 > div.divlevel1").each(function () {
                var currentKeyword = $(this).text().toLowerCase();
                if (currentKeyword.indexOf(currentQuery) >= 0) {
                    $(this).show();
                    rcount++;
                }
                $('.repocounter').text(rcount + " " + searchType);
            });
        } else {
            $(".list-group-root.level0 > div.divlevel1").show();
            $('.repocounter').text($(".list-group-root.level0 > div.divlevel1").length + " " + searchType);
        }
    });

    $('.logouta').click(function () {
        clearCatalogMatrix();
    });
}

async function loadCatalogMatrix() {
    var matrix = sessionStorage.getItem(_Catalog_Matrix);
    var refresh = false;
    refresh = matrix === null;
    if (!refresh) refresh = !_Cache_Enabled;
    if (!refresh) refresh = _Cache_Enabled && matrix !== null && HasExpired(JSON.parse(matrix).expiry);

    if (!refresh) return;
    showGridLoading(30, "Loading global data sets");
    await reloadSearchMatrix().then(async function() {
        showGridResults();
    });
}

function clearCatalogMatrix() {
    sessionStorage.removeItem(_Catalog_Matrix);
}

async function loadSearchImagesCatalog() {
    var response = [];
    await $.ajax({
        async: true,
        type: "GET",
        url: "/Images/GetImagesCatalogWithTags",
        success: function (apiResponse) {
            console.log("Images search catalog loaded successfully");
            response = apiResponse.result;
        },
        error: function (ex) {
            if (ex.readyState === 0) return;
            console.log("Images search catalog load failed with status code: " + ex.responseJSON.statusCode);
            console.log("Images search catalog load failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Images search catalog load failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridError();
        }
    });
    return response;
}

async function loadSearchChartsCatalog() {
    var response = [];
    await $.ajax({
        async: true,
        type: "GET",
        url: "/Charts/GetChartsCatalogFull",
        success: function (apiResponse) {
            console.log("Charts search catalog loaded successfully");
            response = apiResponse.result;
        },
        error: function (ex) {
            if (ex.readyState === 0) return;
            console.log("Charts search catalog load failed with status code: " + ex.responseJSON.statusCode);
            console.log("Charts search catalog load failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Charts search catalog load failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridError();
        }
    });
    return response;
}

async function loadSearchClustersCatalog() {
    var response = [];
    await $.ajax({
        async: true,
        type: "GET",
        url: "/K8S/GetSubscriptions",
        success: function(apiResponse) {
            $.each(apiResponse.result.value,
                function(k, r) {
                    var subscription = {
                        name: r.displayName,
                        id: r.subscriptionId,
                        resourceId: r.id,
                        clusters: []
                    };
                    response.push(subscription);
                });
        },
        error: function(ex) {
            if (ex.readyState === 0) return;
            console.log("Clusters search catalog load failed with status code: " + ex.responseJSON.statusCode);
            console.log("Clusters search catalog load failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Clusters search catalog load failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridError();
        }
    }).then(async function() {
        var subscriptionIds = [];
        $.each(response,
            function(k, s) {
                subscriptionIds.push(s.id);
            });
        await $.ajax({
            async: true,
            type: "POST",
            url: "/K8S/GetClustersWithDeployment",
            data: { subscriptionIds: subscriptionIds },
            success: function(apiResponse) {
                $.each(apiResponse.result,
                    function(j, r) {
                        $.each(r.value,
                            function(k, c) {
                                response.find(x => x.id === j).clusters.push(c);
                            });
                    });
                console.log("Clusters search catalog loaded successfully");
            },
            error: function(ex) {
                if (ex.readyState === 0) return;
                console.log("Clusters search catalog load failed with status code: " + ex.responseJSON.statusCode);
                console.log("Clusters search catalog load failed with error message: " + ex.responseJSON.errorMessage);
                console.log("Clusters search catalog failed with result: " + ex.responseJSON.result);
                console.log(ex);
                showGridError();
            }
        });
    });
    var filteredResponse = [];
    $.each(response,
        function(n, b) {
            if (b.clusters.length > 0) filteredResponse.push(b);
        });
    return filteredResponse;
}

async function reloadSearchMatrix() {
    var imagesCalalog = [];
    var chartsCatalog = [];
    var clustersCatalog = [];
    if (_User_Policies.includes(_Policy_Acr)) {
        imagesCalalog = await loadSearchImagesCatalog();
        chartsCatalog = await loadSearchChartsCatalog();
    }
    if (_User_Policies.includes(_Policy_Aks)) {
        clustersCatalog = await loadSearchClustersCatalog();
    }
    sessionStorage.setItem(_Catalog_Matrix,
        JSON.stringify(
            {
                expiry: AddMinutes(new Date(), _Cache_Duration),
                images: imagesCalalog,
                charts: chartsCatalog,
                clusters: clustersCatalog,
                matrix: populateCatalogMatrix(imagesCalalog, chartsCatalog, clustersCatalog)
            }));
}

function populateCatalogMatrix(imagesCalalog, chartsCatalog, clustersCatalog) {
    var matrix = [];
    $.each(clustersCatalog,
        function(sb, subscription) {
            $.each(subscription.clusters,
                function(cl, cluster) {
                    matrix.push(
                        {
                            subscription: subscription.name,
                            cluster: cluster.name,
                            images: [],
                            charts: []
                        });
                });
        });
    $.each(clustersCatalog,
        function(sb, subscription) {
            $.each(subscription.clusters,
                function(cl, cluster) {
                    $.each(cluster.deployment.items,
                        function(it, item) {
                            $.each(chartsCatalog,
                                function(ch, chart) {
                                    $.each(chart,
                                        function(name, details) {
                                            if (item.metadata.labels.chart !== null &&
                                                item.metadata.labels.chart.includes(ch + "-" + details.version)) {
                                                matrix.find(x => x.subscription === subscription.name &&
                                                    x.cluster === cluster.name).charts.push(ch + "-" + details.version);
                                            }
                                        });
                                });
                            $.each(item.spec.template.spec.containers,
                                function(cn, container) {
                                    $.each(imagesCalalog,
                                        function(im, image) {
                                            $.each(image,
                                                function(name, details) {
                                                    if (container.image !== null &&
                                                        container.image.includes(im + ":" + details.tag)) {
                                                        matrix.find(x => x.subscription === subscription.name &&
                                                            x.cluster === cluster.name).images.push(im + ":" + details.tag);
                                                    }
                                                });
                                        });
                                });
                        });
                });
        });
    return matrix;
}

function searchChartUsageByName(chartName) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            $.each(s.charts,
                function (j, c) {
                    if (c.includes(chartName)) {
                        usages.push({ subscription: s.subscription, cluster: s.cluster });
                    }
                });
        });
    return usages;
}

function searchChartUsageByNameVersion(chartNameVersion) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            $.each(s.charts,
                function (j, c) {
                    if (c === chartNameVersion) {
                        usages.push({ subscription: s.subscription, cluster: s.cluster });
                    }
                });
        });
    return usages;
}

function searchImageUsageByName(imageName) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            $.each(s.images,
                function (j, i) {
                    if (i.includes(imageName)) {
                        usages.push({ subscription: s.subscription, cluster: s.cluster });
                    }
                });
        });
    return usages;
}

function searchImageUsageByNameTag(imageNameTag) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            $.each(s.images,
                function (j, i) {
                    if (i === imageNameTag) {
                        usages.push({ subscription: s.subscription, cluster: s.cluster });
                    }
                });
        });
    return usages;
}

function searchClusterImageUsage(image) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    var firstSlash = image.indexOf("/");
    var imageNameTag = image.substr(firstSlash + 1);
    $.each(catalogMatrix,
        function (h, s) {
            $.each(s.images,
                function (j, i) {
                    if (i === imageNameTag) {
                        usages.push({ subscription: s.subscription, cluster: s.cluster });
                    }
                });
        });
    return usages;
}

function searchClusterChartUsage(chart) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            $.each(s.charts,
                function (j, c) {
                    if (c === chart) {
                        usages.push({ subscription: s.subscription, cluster: s.cluster });
                    }
                });
        });
    return usages;
}

function searchClusterLevelImageUsage(cluster) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function(h, s) {
            if (s.cluster === cluster) {
                $.each(s.images,
                    function(j, i) {
                        usages.push({ cluster: s.cluster, image: i });
                    });
            }
        });
    return usages;
}

function searchClusterLevelChartUsage(cluster) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            if (s.cluster === cluster) {
                $.each(s.charts,
                    function (j, c) {
                        usages.push({ cluster: s.cluster, chart: c });
                    });
            }
        });
    return usages;
}

function searchSubscriptionLevelImageUsage(subscription) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            if (s.subscription === subscription) {
                $.each(s.images,
                    function (j, i) {
                        usages.push({ cluster: s.cluster, image: i });
                    });
            }
        });
    return usages;
}

function searchSubscriptionLevelChartUsage(subscription) {
    var catalogMatrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix)).matrix;
    var usages = [];
    $.each(catalogMatrix,
        function (h, s) {
            if (s.subscription === subscription) {
                $.each(s.charts,
                    function (j, c) {
                        usages.push({ cluster: s.cluster, chart: c });
                    });
            }
        });
    return usages;
}

function dedupeSubscriptionClusterUsage(usages) {
    var unique = [];
    $.each(usages,
        function(i, u) {
            if (unique.find(
                    c => c.subscription === u.subscription &&
                    c.cluster === u.cluster) ===
                undefined) {
                unique.push(u);
            }
        });
    return unique;
}

function dedupeImageUsage(usages) {
    var unique = [];
    $.each(usages,
        function (j, u) {
            if (unique.find(
                    c => c.image === u.image &&
                    c.cluster === u.cluster) ===
                undefined) {
                unique.push(u);
            }
        });
    return unique;
}

function dedupeChartUsage(usages) {
    var unique = [];
    $.each(usages,
        function (j, u) {
            if (unique.find(
                    c => c.chart === u.chart &&
                    c.cluster === u.cluster) ===
                undefined) {
                unique.push(u);
            }
        });
    return unique;
}

function clearSearch() {
    $('#search').val('');
}

function showGridLoading(progressSeconds, loadingText) {
    $(".grid-container").hide();
    $(".grid-loader div.text span.well span").text(loadingText);
    $(".grid-loader").fadeIn(1000);
    $(".grid-error").hide();
    $('.progress-bar').css('width', '0%');
    $('.progress-bar').text("0%");
    $('.progress').hide();

    if (progressSeconds === 0) return;

    $('.progress-bar').css('width', '0%');
    $('.progress-bar').text("0%");
    $('.progress').show();
    var ratio = Math.round(100 / progressSeconds);
    var i = ratio;
    var counterUp = setInterval(function () {
        i = i + ratio;
        if (i <= 100) {
            $('.progress-bar').css('width', i + '%');
            $('.progress-bar').text(i + "%");
        } else {
            clearInterval(counterUp);
        }
    }, 1000);
}

function showGridResults() {
    $(".grid-loader").hide();
    $(".grid-loader div.text span.well span").text("");
    $(".grid-container").fadeIn(1000);
    $(".grid-error").hide();
    $('.progress-bar').css('width', '0%');
    $('.progress-bar').text("0%");
    $('.progress').hide();
}

function showGridError() {
    $(".grid-loader").hide();
    $(".grid-loader div.text span.well span").text("");
    $(".grid-container").hide();
    $(".grid-error").fadeIn(1000);
    $('.progress-bar').css('width', '0%');
    $('.progress-bar').text("0%");
    $('.progress').hide();
}

function showNoGrid() {
    $(".grid-loader").hide();
    $(".grid-loader div.text span.well span").text("");
    $(".grid-container").hide();
    $(".grid-error").hide();
    $('.progress-bar').css('width', '0%');
    $('.progress-bar').text("0%");
    $('.progress').hide();
}

function dateFormatter(date) {
    var newDate = new Date(date);
    return newDate.toLocaleDateString('en-GB') + " " + newDate.toLocaleTimeString('en-GB');
}

function calculateAge(date) {
    var start = new Date(date);
    var end = new Date();
    var diff = new Date(end - start);
    var days = Math.round(diff / 1000 / 60 / 60 / 24);
    if (days === 0) return "Today's";
    if (days === 1) return "1 day old";
    return days + " days old";
}

function AddMinutes(date, minutes) {
    var newDate = new Date(date);
    newDate.setMinutes(date.getMinutes() + minutes);
    return newDate;
}

function HasExpired(date) {
    var expiry = new Date(date);
    var now = new Date();
    return now > expiry;
}

function countDeploymentsInCluster(deployments) {
    var count = deployments.length;
    if (count === 0) return "Empty";
    if (count === 1) return "1 deployment";
    return count + " deployments";
}

function composeAzureUrl(destination) {
    return window.open("https://portal.azure.com/#@" + _Azure_Tenant + "/resource" + destination);
}

function composeAzureDns(dnsPrefix, location) {
    return dnsPrefix + "." + location + ".cloudapp.azure.com";
}

function createModal(id, title, body) {
    var divModal = $("<div>",
        {
            id: id,
            class: "modal fade",
            "tabindex": "-1",
            "role": "dialog",
            "aria-hidden": true
        });
    var divModalDialog = $("<div>", { class: "modal-dialog", "role": "document" });
    var divModalContent = $("<div>", { class: "modal-content" });
    var divModalHeader = $("<div>", { class: "modal-header" });
    var hModalTitle = $("<h4>",
        {
            text: title,
            class: "modal-title"
        });
    divModalHeader.append(hModalTitle);
    var divModalBody = $("<div>", { class: "modal-body" });
    divModalBody.append(body);
    divModalContent.append(divModalHeader);
    divModalContent.append(divModalBody);
    var divModalFooter = $("<div>", { class: "modal-footer" });
    var btnModalFooterClose = $("<button>",
        {
            type: "button",
            class: "btn btn-primary",
            "data-dismiss": "modal",
            text: "Close"
        });
    divModalFooter.append(btnModalFooterClose);
    divModalContent.append(divModalFooter);
    divModalDialog.append(divModalContent);
    divModal.append(divModalDialog);
    return divModal;
}