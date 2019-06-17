var _Images_Catalog = [];

async function loadImages() {
    await loadCatalogMatrix().then(function () {
        loadImagesCatalog();
    });
}

function loadImagesCatalog() {
    clearSearch();
    showGridLoading(0, "Loading images repository");
    var matrix = JSON.parse(sessionStorage.getItem(_Catalog_Matrix));
    if (matrix === null) {
        showGridError();
        return;
    }
    var images = matrix.images;
    if (images === null) {
        showGridError();
        return;
    }
    _Images_Catalog = images;
    populateImagesCatalog(images);
    showGridResults();
}

function loadImageDetails(repoInd, imageName) {
    var caller = $("#item-" + repoInd);
    if (!caller[0].className.includes('list-group collapse in') &&
        !_Images_Catalog[imageName].loaded) {
        showGridLoading(0, "Loading image details for '" + imageName + "'");
        var repository = _Images_Catalog[imageName];
        var imageTags = [];
        $.each(repository,
            function(r, i) {
                imageTags.push(i.tag);
            });
        $.ajax({
            async: true,
            type: "POST",
            url: "/Images/GetImageDetails",
            data: { imageName: imageName, imageTags: imageTags },
            success: function (apiResponse) {
                _Images_Catalog[imageName] = apiResponse.result[imageName];
                _Images_Catalog[imageName].loaded = true;
                populateImageDetails(apiResponse.result, repoInd);
                console.log("Image details for '" + imageName + "' loaded successfully");
                showGridResults();
                $("a[href='#item-" + repoInd + "']").focus();
            },
            error: function (ex) {
                if (ex.readyState === 0) return;
                console.log("Image details load for '" + imageName + "' failed with status code: " + ex.responseJSON.statusCode);
                console.log("Image details load for '" + imageName + "' failed with error message: " + ex.responseJSON.errorMessage);
                console.log("Image details load for '" + imageName + "' failed with result: " + ex.responseJSON.result);
                console.log(ex);
                showGridError();
            }
        });
    }
}

function deleteImageRepository(imageName) {
    var isApproved = false;
    var approvalPrompt = "You are deleting the entire image repository '" + imageName + "'.\n\n";
    approvalPrompt +=
        "Please type the image repository name into the textbox and click OK to confirm your action.";
    isApproved = prompt(approvalPrompt) === imageName;

    if (!isApproved) {
        alert("Wrong image repository name enterd");
        return;
    }

    var repositoryDeleted = "The followig image repository will be deleted.\n\n";
    repositoryDeleted += imageName + "\n\n";
    repositoryDeleted += "This action won't be undone. Do you want to proceed?";
    var toDelete = confirm(repositoryDeleted);
    if (!toDelete) return;

    showGridLoading(0, "Deleting image repository '" + imageName + "'");
    $.ajax({
        async: true,
        type: "POST",
        url: "/Images/DeleteImageRepository",
        data: { imageName: imageName },
        success: function (apiResponse) {
            if (apiResponse.result) {
                console.log("Delete '" + imageName + "' completed successfully");
                showNoGrid();
                alert("Delete '" + imageName + "' completed successfully");
                clearCatalogMatrix();
                loadImages();
            } else {
                console.log("Delete '" + imageName + "' not completed");
                showGridResults();
                alert("Delete '" + imageName + "' not completed");
            }
        },
        error: function (ex) {
            if (ex.readyState === 0) return;
            console.log("Delete image repository '" + imageName + "' failed with status code: " + ex.responseJSON.statusCode);
            console.log("Delete image repository '" + imageName + "' failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Delete image repository '" + imageName + "' failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridResults();
            alert("Error ocurred, try again...");
        }
    });
}

function deleteImageTag(imageName, imageDigest, imageInd) {
    var isApproved = false;
    if (imageInd <= 3) {
        var approvalPrompt = "You are deleting an image version that is ";
        if (imageInd === 1) {
            approvalPrompt += "the latest version.\n\n";
        } else if (imageInd === 2) {
            approvalPrompt += "only 1 version away from the latest.\n\n";
        } else {
            approvalPrompt += "only " + (imageInd - 1) + " versions away from the latest.\n\n";
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

    var imageSearch = searchForImageDigest(imageDigest);
    var imagesDeleted = "";
    var digestsDeleted = "";

    if (imageSearch.length > 1) {
        imagesDeleted += "The followig images will be deleted. This is because they share the same digest.\n";
        $.each(imageSearch,
            function (j, i) {
                digestsDeleted += i + "\n";
            });
    } else {
        imagesDeleted += "The followig image will be deleted.\n";
        digestsDeleted = imageSearch + "\n";
    }

    imagesDeleted += "\n" + digestsDeleted + "\n";
    imagesDeleted += "This action won't be undone. Do you want to proceed?";

    var toDelete = confirm(imagesDeleted);
    if (!toDelete) return;

    showGridLoading(0, "Deleting image digest '" + imageDigest + "'");
    $.ajax({
        async: true,
        type: "POST",
        url: "/Images/DeleteImageTag",
        data: { imageName: imageName, imageDigest: imageDigest },
        success: function (apiResponse) {
            if (apiResponse.result) {
                console.log("Delete '" + imageDigest + "' completed successfully");
                showNoGrid();
                alert("Delete '" + imageDigest + "' completed successfully");
                clearCatalogMatrix();
                loadImages();
            } else {
                console.log("Delete '" + imageDigest + "' not completed");
                showGridResults();
                alert("Delete '" + imageDigest + "' not completed");
            }
        },
        error: function (ex) {
            if (ex.readyState === 0) return;
            console.log("Delete image tag '" + imageDigest + "' failed with status code: " + ex.responseJSON.statusCode);
            console.log("Delete image tag '" + imageDigest + "' failed with error message: " + ex.responseJSON.errorMessage);
            console.log("Delete image tag '" + imageDigest + "' failed with result: " + ex.responseJSON.result);
            console.log(ex);
            showGridResults();
            alert("Error ocurred, try again...");
        }
    });
}

function populateImagesCatalog(results) {
    var repoInd = 1;
    var imageInd = 1;
    var divLevel0 = $(".list-group-root");
    divLevel0.html("");
    $.each(results,
        function(k, r) {
            _Images_Catalog[k].loaded = false;
            var divLevel1 = $("<div>", { class: "divlevel1" });
            var aLevel1 = $("<a>",
                {
                    href: "#item-" + repoInd,
                    class: "list-group-item level1",
                    "data-toggle": "collapse",
                    onclick: "loadImageDetails(" + repoInd + ", \"" + k + "\");"
                });
            var iLevel1 = $("<i>", { class: "glyphicon glyphicon-folder-open" });
            aLevel1.append(iLevel1);
            var spanCopyLevel1 = $("<span>", { id: "copy-item-" + repoInd, text: k });
            aLevel1.append(spanCopyLevel1);
            var spanCounter = $("<span>",
                {
                    class: "well well-sm",
                    text: countTagsInImagesRepository(k)
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
            var imageUsageName = searchImageUsageByName(k);
            if (imageUsageName.length > 0) {
                var spanLevel1Usage = $("<span>", { class: "usage-k8s" });
                var spanLevel1UsageCount = $("<span>", { class: "usage-k8s-count", text: imageUsageName.length });
                spanLevel1Usage.append(spanLevel1UsageCount);
                iLevel1.after(spanLevel1Usage);
                var divModalBodyLevel1 = $("<div>");
                var imageUsageNameUnique = dedupeSubscriptionClusterUsage(imageUsageName);
                $.each(imageUsageNameUnique,
                    function (i, u) {
                        var pUsage = $("<p>", { class: "usage-k8s-items" });
                        var spanSubscription = $("<span>", { text: u.subscription });
                        var spanCluster = $("<span>", { text: u.cluster });
                        pUsage.append(spanSubscription);
                        pUsage.append(spanCluster);
                        divModalBodyLevel1.append(pUsage);
                    });
                var divModalLevel1 = createModal(
                    "modal-" + repoInd,
                    k,
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
                        onclick: "deleteImageRepository(\"" + k + "\");"
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
                function(j, i) {
                    var divLevel2 = $("<div>", { class: "divlevel2" });
                    var aLevel2 = $("<a>",
                        {
                            href: "#item-" + repoInd + "-" + imageInd,
                            class: "list-group-item level2",
                            "data-toggle": "collapse"
                        });
                    var iLevel2 = $("<i>", { class: "glyphicon glyphicon-tag" });
                    aLevel2.append(iLevel2);
                    var spanCopyLevel2 = $("<span>", { id: "copy-item-" + repoInd + "-" + imageInd, text: i.tag });
                    aLevel2.append(spanCopyLevel2);
                    divLevel2.append(aLevel2);
                    var spanCopyIconLevel2 = $("<span>",
                        {
                            class: "glyphicon glyphicon-copy",
                            "aria-hidden": true
                        });
                    var buttonCopyLevel2 = $("<button>",
                        {
                            class: "btn btn-light clipcopy",
                            "data-clipboard-target": "#copy-item-" + repoInd + "-" + imageInd
                        });
                    buttonCopyLevel2.append(spanCopyIconLevel2);
                    divLevel2.append(buttonCopyLevel2);
                    var imageUsageNameTag = searchImageUsageByNameTag(k + ":" + i.tag);
                    if (imageUsageNameTag.length > 0) {
                        var spanLevel2Usage = $("<span>", { class: "usage-k8s" });
                        var spanLevel2UsageCount = $("<span>", { class: "usage-k8s-count", text: imageUsageNameTag.length });
                        spanLevel2Usage.append(spanLevel2UsageCount);
                        iLevel2.after(spanLevel2Usage);
                        var divModalBodyLevel2 = $("<div>");
                        var imageUsageNameTagUnique = dedupeSubscriptionClusterUsage(imageUsageNameTag);
                        $.each(imageUsageNameTagUnique,
                            function (i, u) {
                                var pUsage = $("<p>", { class: "usage-k8s-items" });
                                var spanSubscription = $("<span>", { text: u.subscription });
                                var spanCluster = $("<span>", { text: u.cluster });
                                pUsage.append(spanSubscription);
                                pUsage.append(spanCluster);
                                divModalBodyLevel2.append(pUsage);
                            });
                        var divModalLevel2 = createModal(
                            "modal-" + repoInd + "-" + imageInd,
                            k + ":" + i.tag,
                            divModalBodyLevel2);
                        $(".grid-modals").append(divModalLevel2);
                        var btnShowDependenciesLevel2 = $("<button>",
                            {
                                type: "button",
                                class: "btn btn-info",
                                "data-toggle": "modal",
                                "data-target": "#modal-" + repoInd + "-" + imageInd
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
                    divLevel1Details.append(divLevel2);
                    var divLevel2Details = $("<div>",
                        {
                            id: "item-" + repoInd + "-" + imageInd,
                            class: "list-group collapse"
                        });
                    divLevel1Details.append(divLevel2Details);
                    imageInd++;
                });
            divLevel0.append(divLevel1Details);
            repoInd++;
            imageInd = 1;
        });

    var repoCounterText = " repository";
    if (repoInd - 1 > 1) repoCounterText = " repositories";
    $('.repocounter').text(repoInd - 1 + repoCounterText);
}

function populateImageDetails(results, repoInd) {
    var imageInd = 1;
    $.each(results,
        function(k, r) {
            $.each(r,
                function(j, i) {
                    var aLevel2 = $("a[href='#item-" + repoInd + "-" + imageInd + "']");
                    var spanAge = $("<span>",
                        {
                            class: "well well-sm",
                            text: calculateAge(i.created)
                        });
                    aLevel2.after(spanAge);
                    if (_User_Roles.includes(_Role_ArcContributor)) {
                        var btnDeleteTag = $("<button>",
                            {
                                type: "button",
                                class: "btn btn-danger",
                                onclick: "deleteImageTag(\"" +
                                    k +
                                    "\",\"" +
                                    i.digest +
                                    "\"," +
                                    imageInd +
                                    ");"
                            });
                        var spanDeleteIcon = $("<span>",
                            {
                                class: "glyphicon glyphicon-remove-sign",
                                "aria-hidden": true
                            });
                        btnDeleteTag.append(spanDeleteIcon);
                        btnDeleteTag.append("Delete");
                        aLevel2.parent().append(btnDeleteTag);
                    }
                    var divLevel2Details = $("#item-" + repoInd + "-" + imageInd);
                    var divLevel3 = $("<div>", { class: "list-group-item divlevel3 row" });
                    var div0 = $("<div>", { class: "col-lg-12" });
                    var p0 = $("<p>");
                    var label0 = $("<label>", { text: "Name:" });
                    var span0 = $("<span>", { id: "copy-name-item-" + repoInd + "-" + imageInd + "-" + 0, text: k + ":" + i.tag });
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
                            "data-clipboard-target": "#copy-name-item-" + repoInd + "-" + imageInd + "-" + 0
                        });
                    buttonCopyDiv0.append(spanCopyIconDiv0);
                    p0.append(buttonCopyDiv0);
                    div0.append(p0);
                    divLevel3.append(div0);
                    var div1 = $("<div>", { class: "col-lg-12" });
                    var p1 = $("<p>");
                    var label1 = $("<label>", { text: "Description:" });
                    var span1 = $("<span>", { text: i.labels.description });
                    p1.append(label1);
                    p1.append(span1);
                    div1.append(p1);
                    divLevel3.append(div1);
                    var div2 = $("<div>", { class: "col-lg-12" });
                    var p2 = $("<p>");
                    var label2 = $("<label>", { text: "Created:" });
                    var span2 = $("<span>", { text: dateFormatter(i.created) });
                    p2.append(label2);
                    p2.append(span2);
                    div2.append(p2);
                    divLevel3.append(div2);
                    var div3 = $("<div>", { class: "col-lg-12" });
                    var p3 = $("<p>");
                    var label3 = $("<label>", { text: "Maintainer:" });
                    var span3 = $("<span>", { text: i.labels.maintainer });
                    p3.append(label3);
                    p3.append(span3);
                    div3.append(p3);
                    divLevel3.append(div3);
                    var div4 = $("<div>", { class: "col-lg-12" });
                    var p4 = $("<p>");
                    var label4 = $("<label>", { text: "Architecture:" });
                    var span4 = $("<span>", { text: i.architecture });
                    p4.append(label4);
                    p4.append(span4);
                    div4.append(p4);
                    divLevel3.append(div4);
                    var div5 = $("<div>", { class: "col-lg-12" });
                    var p5 = $("<p>");
                    var label5 = $("<label>", { text: "Digest:" });
                    var span5 = $("<span>", { text: i.digest });
                    p5.append(label5);
                    p5.append(span5);
                    div5.append(p5);
                    divLevel3.append(div5);
                    divLevel2Details.append(divLevel3);
                    imageInd++;
                });
        });
}

function searchForImageDigest(imageDigest) {
    var digests = [];
    $.each(_Images_Catalog,
        function (k, r) {
            $.each(r,
                function (j, i) {
                    if (i.digest !== null) {
                        if (i.digest === imageDigest) {
                            digests.push(k + " " + i.tag);
                        }
                    }
                });
        });
    return digests;
}

function countTagsInImagesRepository(repository) {
    var count = _Images_Catalog[repository].length;
    if (count === 0) return "Empty";
    if (count === 1) return "1 version";
    return count + " versions";
}

function calculateLastUpdatedImage(repository) {
    var latest = _Images_Catalog[repository][0].created;
    var start = new Date(latest);
    var end = new Date();
    var diff = new Date(end - start);
    var days = Math.round(diff / 1000 / 60 / 60 / 24);
    if (days === 0) return "Updated today";
    if (days === 1) return "Updated yesterday";
    return "Updated " + days + " days ago";
}